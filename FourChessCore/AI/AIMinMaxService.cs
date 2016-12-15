using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FourChessCore
{
    class AIMinMaxService
    {
        Player player;

        public AIMinMaxService(Player player)
        {
            this.player = player;
        }

        public StepInfo NextStep(FourChessGame game)
        {
            StepInfo step;
            NegaAlphaBeta(game, this.player, 5, -int.MaxValue, int.MaxValue, out step);
            return step;
        }

        private int NegaAlphaBeta(FourChessGame game, Player player, int depth, int alpha, int beta, out StepInfo bestStep)
        {
            LogManager.WriteLog(string.Format("[NegaAlphaBeta]Player:{0}, depth:{1}, alpha:{2}, beta:{3}\r\n{4}\r\n", player, depth, alpha, beta, game.Board.ShowText()));

            int value;
            bestStep = null;
            if (depth <= 0 || GameOver(game))
            {
                return Evaluation(game);
            }

            List<StepInfo> allPossibleMoves = GetAllPossibleMoves(game, player);//得到所有可能的移动

            foreach (StepInfo step in allPossibleMoves)
            {
                StepResult result = game.Move(step);//改变局面

                StepInfo tmp;
                int bestScore = -int.MaxValue;
                Player nextPlayer = (player == Player.One) ? Player.Counter : Player.One;
                value = -NegaAlphaBeta(game, nextPlayer, depth - 1, -beta, -alpha, out tmp);//搜索子节点

                LogManager.WriteLog(string.Format("[StepNegaAlphaBeta]value:{0}\r\n{1}\r\n", value, game.Board.ShowText()));

                game.UnMove(step, result);//恢复局面

                if (value > bestScore)
                {
                    bestScore = value;
                    bestStep = step;
                }
                if (bestScore > alpha)
                    alpha = bestScore;
                if (bestScore >= beta)
                    break;
                //if (value >= alpha)
                //{
                //    //取最大值
                //    alpha = value;
                //    bestStep = step;
                //    if (alpha >= beta)
                //    {
                //        break;//剪枝
                //    }
                //}
            }

            return alpha;
        }

        private List<StepInfo> GetAllPossibleMoves(FourChessGame game, Player player)
        {
            List<StepInfo> stepList = new List<StepInfo>();
            foreach (PieceInfo piece in game.Pieces)
            {
                //找到己方的棋子
                if (piece.Player == player)
                {
                    //尝试所有可能的移动方向
                    foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                    {
                        StepInfo step = new StepInfo()
                        {
                            Direction = dir,
                            OriginalX = piece.X,
                            OriginalY = piece.Y,
                            Piece = piece//copy!
                        };
                        if (game.Board.CheckIsValid(step))
                        {
                            stepList.Add(step);
                        }
                    }
                }
            }
            return stepList;
        }

        private bool GameOver(FourChessGame game)
        {
            return game.GetLosePlayer() != null;
        }

        /// <summary>
        /// 估值函数
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private int Evaluation(FourChessGame game)
        {
            //棋子数量对比
            var myPieces = game.Pieces.Where(n => n.Player == this.player);
            var counterPieces = game.Pieces.Where(n => n.Player != this.player);
            int piecesCountEval = myPieces.Count() - counterPieces.Count();

            //占据中间的位置
            int centerPiecesEval = myPieces.Count(n => n.X != 0 && n.X != FourChessGame.SIZE - 1 && n.Y != 0 && n.Y != FourChessGame.SIZE - 1);

            //特定局面
            int certainSituationEval = GetCertainSituationEval(game);

            return -(piecesCountEval * 10 + centerPiecesEval * 6 + certainSituationEval);
        }

        private int GetCertainSituationEval(FourChessGame game)
        {
            return 0;
        }
    }
}
