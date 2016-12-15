using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChessCore
{
    [Serializable]
    public class FourChessBoard
    {
        public BoardPosition[,] Positions;

        public FourChessBoard() { }

        public FourChessBoard(int size)
        {
            Positions = new BoardPosition[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    this.Positions[i, j] = new BoardPosition()
                    {
                        Piece = null,
                        X = i,
                        Y = j
                    };
                }
            }
        }

        public void Add(PieceInfo piece)
        {
            Positions[piece.X, piece.Y].Piece = piece;
        }

        internal StepResult Move(StepInfo step, bool ignoreDead = false)
        {
            bool isValid = CheckIsValid(step);
            if (!isValid)
            {
                return new StepResult()
                {
                    IsValid = false,
                    Step = step
                };
            }
            else
            {
                Remove(step.Piece);

                Positions[step.NewX, step.NewY].Piece = step.Piece;
                step.Piece.X = step.NewX;
                step.Piece.Y = step.NewY;

                if (ignoreDead)
                {
                    return null;
                }
                else
                {
                    StepResult result = HandleResult(step);
                    result.Step = step;
                    return result;
                }
            }
        }

        public bool CheckIsValid(StepInfo step)
        {
            if (step.NewX < 0 || step.NewX >= FourChessGame.SIZE || step.NewY < 0 || step.NewY >= FourChessGame.SIZE)
            {
                return false;
            }
            else if (this.Positions[step.NewX, step.NewY].IsVacant == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private StepResult HandleResult(StepInfo step)
        {
            StepResult result = new StepResult()
            {
                DeadPieces = new List<PieceInfo>(),
                IsValid = true
            };

            //horizental
            //for (int i = 0; i < FourChessGame.SIZE; i++)
            //{
            int i = step.NewX;
            string array = string.Empty;
            for (int j = 0; j < FourChessGame.SIZE; j++)
            {
                PieceInfo p = this.Positions[i, j].Piece;
                if (p == null)
                {
                    array += "0";
                }
                else
                {
                    array += (int)p.Player;
                }
            }
            int dead = MatchPattern(array);
            if (dead >= 0 && !(i == step.NewX && dead == step.NewY))
            {
                result.DeadPieces.Add(Positions[i, dead].Piece);
            }
            //}
            //vertical
            //for (int i = 0; i < FourChessGame.SIZE; i++)
            //{
            array = string.Empty;
            i = step.NewY;
            for (int j = 0; j < FourChessGame.SIZE; j++)
            {
                PieceInfo p = this.Positions[j, i].Piece;
                if (p == null)
                {
                    array += "0";
                }
                else
                {
                    array += (int)p.Player;
                }
            }
            dead = MatchPattern(array);
            if (dead >= 0 && !(dead == step.NewX && i == step.NewY))
            {
                result.DeadPieces.Add(Positions[dead, i].Piece);
            }
            //}
            foreach (PieceInfo p in result.DeadPieces)
            {
                Remove(p);
            }

            return result;
        }

        private int MatchPattern(string array)
        {
            int result = -1;
            //1120,0112,0211,2110;2210,0221,0122,1220
            if (array == "1120" || array == "2210")
            {
                result = 2;
            }
            else if (array == "0112" || array == "0221")
            {
                result = 3;
            }
            else if (array == "0211" || array == "0122")
            {
                result = 1;
            }
            else if (array == "2110" || array == "1220")
            {
                result = 0;
            }
            return result;
        }

        internal void Remove(PieceInfo piece)
        {
            Positions[piece.X, piece.Y].Piece = null;
        }

        public FourChessBoard DeepCopy()
        {
            FourChessBoard newBoard = new FourChessBoard(FourChessGame.SIZE);
            for (int i = 0; i < FourChessGame.SIZE; i++)
            {
                for (int j = 0; j < FourChessGame.SIZE; j++)
                {
                    if (!this.Positions[i, j].IsVacant)
                    {
                        PieceInfo p = this.Positions[i, j].Piece.Copy();
                        newBoard.Add(p);
                    }
                }
            }
            return newBoard;
        }

        public string ShowText()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = FourChessGame.SIZE - 1; i >= 0; i--)
            {
                for (int j = 0; j < FourChessGame.SIZE; j++)
                {
                    BoardPosition pos = this.Positions[j, i];
                    sb.Append(pos.IsVacant ? "+" : ((int)pos.Piece.Player).ToString());
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        internal void UnMove(StepInfo step, StepResult result)
        {
            throw new NotImplementedException();
        }
    }
}
