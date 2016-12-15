using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChessCore
{
    [Serializable]
    public class FourChessGame
    {
        public const int SIZE = 4;

        public List<PieceInfo> Pieces;

        public FourChessBoard Board;

        public FourChessGame()
        {
            this.Pieces = new List<PieceInfo>(SIZE);
            this.Board = new FourChessBoard(SIZE);
        }

        public void InitBoard()
        {
            for (int i = 0; i < SIZE; i++)
            {
                PieceInfo p = new PieceInfo()
                {
                    Player = Player.One,
                    X = i,
                    Y = 0
                };
                Pieces.Add(p);
                this.Board.Add(p);

                p = new PieceInfo()
                {
                    Player = Player.Counter,
                    X = i,
                    Y = SIZE - 1
                };
                Pieces.Add(p);
                this.Board.Add(p);
            }
        }

        public void InitTestBoard()
        {
            PieceInfo p = new PieceInfo()
            {
                Player = Player.One,
                X = 0,
                Y = 0
            };
            Pieces.Add(p);
            this.Board.Add(p);

            p = new PieceInfo()
            {
                Player = Player.One,
                X = 2,
                Y = 0
            };
            Pieces.Add(p);
            this.Board.Add(p);

            p = new PieceInfo()
            {
                Player = Player.One,
                X = 3,
                Y = 0
            };
            Pieces.Add(p);
            this.Board.Add(p);

            p = new PieceInfo()
            {
                Player = Player.One,
                X = 2,
                Y = 1
            };
            Pieces.Add(p);
            this.Board.Add(p);

            //Counter
            p = new PieceInfo()
            {
                Player = Player.Counter,
                X = 0,
                Y = 3
            };
            Pieces.Add(p);
            this.Board.Add(p);

            p = new PieceInfo()
            {
                Player = Player.Counter,
                X = 1,
                Y = 3
            };
            Pieces.Add(p);
            this.Board.Add(p);

            p = new PieceInfo()
            {
                Player = Player.Counter,
                X = 2,
                Y = 3
            };
            Pieces.Add(p);
            this.Board.Add(p);

            p = new PieceInfo()
            {
                Player = Player.Counter,
                X = 3,
                Y = 2
            };
            Pieces.Add(p);
            this.Board.Add(p);
        }

        public StepResult Move(StepInfo step)
        {
            StepResult result = this.Board.Move(step);
            UpdatePieces();

            //check iswin
            var lose = GetLosePlayer();
            if (lose != null)
            {
                result.WinPlayer = (lose.Key == Player.One ? Player.Counter : Player.One);
            }

            AIService.CheckLegal(this);
            return result;
        }

        public IGrouping<Player, PieceInfo> GetLosePlayer()
        {
            return this.Pieces.GroupBy(n => n.Player).FirstOrDefault(n => n.Count() <= 1);
        }

        public void Remove(PieceInfo piece)
        {
            this.Board.Remove(piece);
            UpdatePieces();
        }

        private void UpdatePieces()
        {
            Pieces.Clear();
            for (int i = 0; i < FourChessGame.SIZE; i++)
            {
                for (int j = 0; j < FourChessGame.SIZE; j++)
                {
                    BoardPosition pos = this.Board.Positions[j, i];
                    Debug.Assert(pos.IsVacant == (pos.Piece == null));
                    if (!pos.IsVacant)
                    {
                        this.Pieces.Add(pos.Piece);
                    }
                }
            }
        }

        public FourChessGame DeepCopy()
        {
            FourChessGame game = new FourChessGame();
            game.Board = this.Board.DeepCopy();
            game.UpdatePieces();
            return game;
        }

        internal void UnMove(StepInfo step, StepResult result)
        {
            StepInfo reverseStep = new StepInfo()
            {
                Piece = step.Piece,
                Direction = (Direction)(((int)step.Direction + 2) % 4),
                OriginalX = step.NewX,
                OriginalY = step.NewY
            };
            this.Board.Move(reverseStep, true);
            foreach (PieceInfo p in result.DeadPieces)
            {
                this.Board.Positions[p.X, p.Y].Piece = p;
            }
            UpdatePieces();
        }
    }
}
