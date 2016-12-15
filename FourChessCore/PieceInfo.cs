using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChessCore
{
    [Serializable]
    public class PieceInfo
    {
        public Player Player { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public PieceInfo Copy()
        {
            return new PieceInfo()
            {
                Player = this.Player,
                X = this.X,
                Y = this.Y
            };
        }
    }
}
