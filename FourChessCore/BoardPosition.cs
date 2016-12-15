using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChessCore
{
    public class BoardPosition
    {
        public int X { get; set; }

        public int Y { get; set; }

        public PieceInfo Piece { get; set; }

        public bool IsVacant
        {
            get
            {
                return this.Piece == null;
            }
        }
    }
}
