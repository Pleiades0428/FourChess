using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChessCore
{
    public class StepResult
    {
        public bool IsValid { get; set; }

        public StepInfo Step { get; set; }

        public List<PieceInfo> DeadPieces { get; set; }

        public Player? WinPlayer { get; set; }
    }
}
