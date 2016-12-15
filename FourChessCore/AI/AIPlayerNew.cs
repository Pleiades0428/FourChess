using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChessCore
{
    public class AIPlayerNew : GamePlayer
    {
        public string PlayerName { get; set; }
        private AIMinMaxService service;

        public AIPlayerNew(string name)
        {
            this.PlayerName = name;
            this.service = new AIMinMaxService(Player.Counter);
        }

        public StepInfo GetNextStep(FourChessGame game)
        {
            return service.NextStep(game);
        }

        public override string ToString()
        {
            return this.PlayerName;
        }
    }
}
