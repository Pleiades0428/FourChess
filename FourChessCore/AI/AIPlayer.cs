using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChessCore
{
    public class AIPlayer : GamePlayer
    {
        public string PlayerName { get; set; }
        private AIService service;
        AIStyle style;

        public AIPlayer(string name, AIStyle style)
        {
            this.PlayerName = name;
            this.style = style;
            this.service = new AIService(Player.Counter);
        }

        public StepInfo GetNextStep(FourChessGame game)
        {
            return service.NextStep(game, style);
        }

        public override string ToString()
        {
            return this.PlayerName;
        }
    }
}
