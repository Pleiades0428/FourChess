using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChessCore
{
    [Serializable]
    public class StepInfo
    {
        public PieceInfo Piece { get; set; }

        public Direction Direction { get; set; }

        public int NewX
        {
            get
            {
                int x;
                switch (this.Direction)
                {
                    case Direction.Left:
                        x = this.OriginalX - 1;
                        break;
                    case Direction.Right:
                        x = this.OriginalX + 1;
                        break;
                    default:
                        x = this.OriginalX;
                        break;
                }
                return x;
            }
        }

        public int NewY
        {
            get
            {
                int y;
                switch (this.Direction)
                {
                    case Direction.Up:
                        y = this.OriginalY - 1;
                        break;
                    case Direction.Down:
                        y = this.OriginalY + 1;
                        break;
                    default:
                        y = this.OriginalY;
                        break;
                }
                return y;
            }
        }

        public int OriginalX { get; set; }
        public int OriginalY { get; set; }
    }
}
