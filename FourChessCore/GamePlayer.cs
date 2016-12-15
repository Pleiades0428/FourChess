using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChessCore
{
    public interface GamePlayer
    {
        string PlayerName { get; set; }

        StepInfo GetNextStep(FourChessGame game);
    }
}
