using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FourChessCore;

namespace FourChess.AppCode
{
    internal class UIPlayer : GamePlayer
    {
        public string PlayerName { get; set; }

        public UIPlayer(string name)
        {
            this.PlayerName = name;
        }

        public StepInfo GetNextStep(FourChessGame game)
        {
            throw new NotImplementedException();
        }
    }
}
