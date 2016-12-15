using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FourChess.Network;
using FourChess.Properties;
using FourChessCore;

namespace FourChess
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Consts
        int marginTop = 25;
        int marginLeft = 25;
        int pieceSize = 50;
        int blockSize = 70;
        Color SELECTED_COLOR = Color.Goldenrod;
        #endregion

        #region Variables
        FourChessGame game;
        OperStatus status;
        bool isSinglePlay;//是否是一个棋盘上自己跟自己玩
        private GamePlayer counterPlayer;
        private Player currentPlayer;

        PieceInfo selectedPiece = null;
        Button selectedButton = null;
        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Initialize();

            status = OperStatus.WaitToStart;
        }

        private void Initialize()
        {
            this.isSinglePlay = true;
            this.gamePanel.Width = this.gamePanel.Height = blockSize * (FourChessGame.SIZE - 1) + this.pieceSize;

            Image bg = Resources.bg;

            this.gamePanel.BackgroundImage = bg;
            this.gamePanel.BackgroundImageLayout = ImageLayout.Stretch;
        }

        #region GameClick
        private void HandleSinglePlayClick(object sender)
        {
            //select piece
            ClearColoredButtons();

            Button btn = sender as Button;
            BoardPosition pos = btn.Tag as BoardPosition;
            switch (status)
            {
                case OperStatus.WaitToStart:
                    break;
                case OperStatus.Idle:
                    if (pos.Piece != null)
                    {
                        selectedButton = btn;
                        selectedPiece = pos.Piece;
                        btn.BackColor = SELECTED_COLOR;
                        status = OperStatus.Selected;
                    }
                    break;
                case OperStatus.Selected:
                    //perform move
                    int dx = pos.X - selectedPiece.X;
                    int dy = pos.Y - selectedPiece.Y;

                    if (!(Math.Abs(dx) + Math.Abs(dy) == 1))
                    {
                        break;
                    }

                    Direction dir = GetDirectionFromDxDy(dx, dy);

                    StepInfo step = new StepInfo()
                    {
                        Direction = dir,
                        Piece = selectedPiece,
                        OriginalX = selectedPiece.X,
                        OriginalY = selectedPiece.Y
                    };

                    StepResult result = game.Move(step);

                    if (result.IsValid)
                    {
                        //move button
                        Button btnFrom = FindButton(step.OriginalX, step.OriginalY);
                        btnFrom.Text = string.Empty;
                        btnFrom.BackColor = Color.Transparent;

                        Button btnTarget = FindButton(step.NewX, step.NewY);
                        btnTarget.Text = step.Piece.Player.ToString();
                        btnTarget.BackColor = SELECTED_COLOR;
                        selectedButton = btnTarget;

                        WriteLog(string.Format("玩家{0}移动{1}-{2}至{3}-{4}", step.Piece.Player, step.OriginalX, step.OriginalY, step.NewX, step.NewY));

                        //remove button
                        foreach (PieceInfo p in result.DeadPieces)
                        {
                            RemoveButton(p);
                            WriteLog(string.Format("玩家{0}棋子被吃掉：{1}-{2}", p.Player, p.X, p.Y));
                        }

                        status = OperStatus.Idle;
                    }
                    break;
                case OperStatus.Target:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 联网点击
        /// </summary>
        /// <param name="sender"></param>
        private void HandleCounterPlayClick(object sender)
        {
            //select piece
            ClearColoredButtons();

            Button btn = sender as Button;
            BoardPosition pos = btn.Tag as BoardPosition;
            switch (status)
            {
                case OperStatus.WaitToStart:
                    break;
                case OperStatus.Idle:
                    if (pos.Piece != null)
                    {
                        selectedButton = btn;
                        selectedPiece = pos.Piece;
                        btn.BackColor = SELECTED_COLOR;
                        this.selectedButton = btn;
                        status = OperStatus.Selected;
                    }
                    break;
                case OperStatus.Selected:
                    //perform move
                    int dx = pos.X - selectedPiece.X;
                    int dy = pos.Y - selectedPiece.Y;

                    if (!(Math.Abs(dx) + Math.Abs(dy) == 1))
                    {
                        this.status = OperStatus.Idle;
                        break;
                    }

                    Direction dir = GetDirectionFromDxDy(dx, dy);

                    StepInfo step = new StepInfo()
                    {
                        Direction = dir,
                        Piece = selectedPiece,
                        OriginalX = selectedPiece.X,
                        OriginalY = selectedPiece.Y
                    };

                    StepResult result = game.Move(step);

                    if (result.IsValid)
                    {
                        LogStep(step);

                        MoveUIButton(step);

                        RemoveDeadButtons(result);

                        if (result.WinPlayer != null)
                        {
                            MessageBox.Show(string.Format("玩家{0}胜利！", result.WinPlayer.ToString()));
                            status = OperStatus.WaitToStart;
                            break;
                        }
                        status = OperStatus.WaitCounter;
                    }

                    //counter todo:
                    BeginGetCounterNextStep();
                    break;
                case OperStatus.Target:
                    break;
                case OperStatus.WaitCounter:
                    break;
                default:
                    break;
            }
        }

        private void BeginGetCounterNextStep()
        {
            Task.Factory.StartNew(() =>
            {
                StepInfo counterStep = this.counterPlayer.GetNextStep(this.game);

                //Thread.Sleep(2000);

                if (counterStep != null)
                {
                    this.Invoke(new Action(() =>
                    {
                        StepResult result = this.game.Move(counterStep);

                        LogStep(counterStep);

                        MoveUIButton(counterStep);

                        RemoveDeadButtons(result);

                        if (result.WinPlayer != null)
                        {
                            MessageBox.Show(string.Format("玩家{0}胜利！", result.WinPlayer.ToString()));
                            status = OperStatus.WaitToStart;
                        }
                    }));
                    status = OperStatus.Idle;
                }
            });
        }

        private void btn_Click(object sender, EventArgs e)
        {
            if (this.isSinglePlay)
            {
                HandleSinglePlayClick(sender);
            }
            else
            {
                HandleCounterPlayClick(sender);
            }
        }
        #endregion

        #region UI
        private void ClearUI()
        {
            this.gamePanel.Controls.Clear();
        }

        private void SetupUI()
        {
            for (int i = 0; i < FourChessGame.SIZE; i++)
            {
                for (int j = 0; j < FourChessGame.SIZE; j++)
                {
                    BoardPosition position = game.Board.Positions[i, j];
                    PieceInfo p = position.Piece;

                    int left = marginLeft + i * blockSize - pieceSize / 2;
                    int top = marginTop + (FourChessGame.SIZE - j - 1) * blockSize - pieceSize / 2;

                    Button btn = new Button();
                    btn.Left = left;
                    btn.Top = top;
                    btn.Width = pieceSize;
                    btn.Height = pieceSize;
                    btn.Click += btn_Click;

                    if (p == null)
                    {
                        btn.Text = string.Empty;
                    }
                    else
                    {
                        btn.Text = p.Player.ToString();
                        SetButtonTextAndColor(p, btn);
                    }
                    btn.Tag = position;

                    this.gamePanel.Controls.Add(btn);
                }
            }
        }

        private static void SetButtonTextAndColor(PieceInfo p, Button btn)
        {
            if (p == null)
            {
                btn.Text = string.Empty;
                btn.ForeColor = Color.Black;
            }
            else
            {
                btn.Text = p.Player.ToString();
                btn.ForeColor = p.Player == Player.One ? Color.Blue : Color.Red;
            }
        }

        private void RemoveDeadButtons(StepResult result)
        {
            //remove button
            foreach (PieceInfo p in result.DeadPieces)
            {
                RemoveButton(p);
                WriteLog(string.Format("玩家{0}棋子被吃掉：{1}-{2}", p.Player, p.X, p.Y));
            }
        }

        private void MoveUIButton(StepInfo step)
        {
            ClearColoredButtons();

            //move button
            Button btnFrom = FindButton(step.OriginalX, step.OriginalY);
            //btnFrom.BackColor = Color.Transparent;
            SetButtonTextAndColor(null, btnFrom);

            Button btnTarget = FindButton(step.NewX, step.NewY);
            btnTarget.BackColor = SELECTED_COLOR;
            SetButtonTextAndColor(step.Piece, btnTarget);
            selectedButton = btnTarget;
        }

        private void ClearColoredButtons()
        {
            if (selectedButton != null)
            {
                selectedButton.BackColor = Color.Transparent;
            }
        }
        #endregion

        #region Helpers
        private Direction GetDirectionFromDxDy(int dx, int dy)
        {
            Direction dir = Direction.Up;
            if (dx == 1)
            {
                dir = Direction.Right;
            }
            else if (dx == -1)
            {
                dir = Direction.Left;
            }
            else if (dy == 1)
            {
                dir = Direction.Down;
            }
            else if (dx == -1)
            {
                dir = Direction.Up;
            }
            return dir;
        }

        private Button FindButton(PieceInfo p)
        {
            return FindButton(p.X, p.Y);
        }

        private Button FindButton(int x, int y)
        {
            foreach (Control ctrl in this.gamePanel.Controls)
            {
                if (ctrl is Button)
                {
                    BoardPosition pos = ctrl.Tag as BoardPosition;
                    if (pos.X == x && pos.Y == y)
                    {
                        return ctrl as Button;
                    }
                }
            }
            return null;
        }

        private void RemoveButton(PieceInfo p)
        {
            Button btn = FindButton(p);
            if (btn == null)
            {
                throw new Exception("未找到棋子");
            }
            btn.Text = string.Empty;
        }

        private void LogStep(StepInfo step)
        {
            WriteLog(string.Format("玩家{0}移动{1}-{2}至{3}-{4}", step.Piece.Player, step.OriginalX, step.OriginalY, step.NewX, step.NewY));
        }

        private void WriteLog(string msg)
        {
            if (tbLog.InvokeRequired)
            {
                tbLog.Invoke(new Action(() =>
                {
                    tbLog.AppendText(msg + "\r\n");
                }));
            }
            else
            {
                tbLog.AppendText(msg + "\r\n");
            }
        }
        #endregion

        #region Menu
        private void NewAIGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChooseAI chooseAIForm = new ChooseAI();
            if (chooseAIForm.ShowDialog() == DialogResult.OK)
            {
                this.counterPlayer = chooseAIForm.Player;

                //this.counterPlayer = new AIPlayer("AI");
                lbCounterName.Text = this.counterPlayer.PlayerName;
                currentPlayer = Player.One;//todo:
                this.isSinglePlay = false;

                ClearUI();

                this.game = new FourChessGame();
                this.game.InitBoard();
                //this.game.InitTestBoard();

                //TODO:
                SetupUI();

                this.status = OperStatus.Idle;
            }
        }
        #endregion

        #region Test
        
        #endregion
    }

    enum OperStatus
    {
        WaitToStart,
        Idle,
        Selected,
        Target,
        WaitCounter
    }
}
