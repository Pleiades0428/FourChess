using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChessCore
{
    class CertainSituationService
    {
        Dictionary<string, StepInfo> dic;

        public static readonly CertainSituationService Instance = new CertainSituationService();

        private CertainSituationService()
        {
            dic = new Dictionary<string, StepInfo>();
            dic.Add("2222000001001011", new StepInfo()
            {
                Piece = new PieceInfo() { X = 3, Y = 3, Player = Player.Counter },
                OriginalX = 3,
                OriginalY = 3,
                Direction = Direction.Up
            });

            dic = new Dictionary<string, StepInfo>();
            dic.Add("2222000000101101", new StepInfo()
            {
                Piece = new PieceInfo() { X = 0, Y = 3, Player = Player.Counter },
                OriginalX = 0,
                OriginalY = 3,
                Direction = Direction.Up
            });
        }

        public StepInfo GetNextStep(FourChessGame game, Player nextPlayer)
        {
            string pattern = game.Board.ShowText().Replace("\r\n", "").Replace('+', '0');
            StepInfo step;
            dic.TryGetValue(pattern, out step);
            if (step != null)
                step.Piece = step.Piece.Copy();
            return step;
        }
    }
}
