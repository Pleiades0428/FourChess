using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FourChessCore
{
    public class AIService
    {
        Player player;
        AIService _serviceForOne = null;
        AIService serviceForOne
        {
            get
            {
                if (_serviceForOne == null)
                {
                    _serviceForOne = new AIService(Player.One);
                }
                return _serviceForOne;
            }
        }

        public AIService(Player player)
        {
            this.player = player;
        }

        public StepInfo NextStep(FourChessGame game, AIStyle style)
        {
            CheckLegal(game);

            StepInfo step;
            step = CertainSituationService.Instance.GetNextStep(game, Player.Counter);
            if (step != null)
            {
                return step;
            }
            else
            {
                return InnerNextStep(game, style, true);
            }
        }

        public static void CheckLegal(FourChessGame game)
        {
            if (game.Pieces.Count > FourChessGame.SIZE * 2)
            {
                Debug.Fail("棋子多于8个");
            }
        }

        private StepInfo InnerNextStep(FourChessGame game, AIStyle style, bool recursive = false)
        {
            StepInfo bestStep = null;
            List<StepInfo> bestStepList = new List<StepInfo>();
            int maxScore = -100;

            List<StepInfo> allPossibleMoves = GetAllPossibleMoves(game, this.player);

            foreach (StepInfo step in allPossibleMoves)
            {
                StepResult result = game.Move(step);
                //评估分数
                int score = EvalScoreOfSituation(game, result, recursive, style);
                if (score > maxScore)
                {
                    bestStepList.Clear();
                    bestStepList.Add(step);
                    maxScore = score;
                }
                else if (score == maxScore)
                {
                    bestStepList.Add(step);
                }

                game.UnMove(step, result);
            }
            //从所有等分的移动方案中随机选择
            if (bestStepList.Count == 0)
            {
                bestStep = null;
            }
            else if ((style & AIStyle.PreferShrink) == AIStyle.PreferShrink)//优先收缩阵容
            {
                var myPieces = game.Pieces.Where(n => n.Player == this.player);
                double ax = myPieces.Average(n => n.X);
                double ay = myPieces.Average(n => n.Y);

                List<StepInfo> bestStepList2 = new List<StepInfo>();

                double minDistance = double.MaxValue;
                foreach (StepInfo step in bestStepList)
                {
                    double distance = (step.NewX - ax) * (step.NewX - ax) + (step.NewY - ay) * (step.NewY - ay);
                    if (distance < minDistance)
                    {
                        bestStepList2.Clear();
                        bestStepList2.Add(step);
                        minDistance = distance;
                    }
                    else if (distance == minDistance)
                    {
                        bestStepList2.Add(step);
                    }
                }
                int i = new Random().Next(bestStepList2.Count);
                bestStep = bestStepList2[i];
            }
            else if ((style & AIStyle.PreferFar) == AIStyle.PreferFar)//优先扩散阵容
            {
                var myPieces = game.Pieces.Where(n => n.Player == this.player);
                double ax = myPieces.Average(n => n.X);
                double ay = myPieces.Average(n => n.Y);

                List<StepInfo> bestStepList2 = new List<StepInfo>();

                double maxDistance = double.MinValue;
                foreach (StepInfo step in bestStepList)
                {
                    double distance = (step.NewX - ax) * (step.NewX - ax) + (step.NewY - ay) * (step.NewY - ay);
                    if (distance > maxDistance)
                    {
                        bestStepList2.Clear();
                        bestStepList2.Add(step);
                        maxDistance = distance;
                    }
                    else if (distance == maxDistance)
                    {
                        bestStepList2.Add(step);
                    }
                }
                int i = new Random().Next(bestStepList2.Count);
                bestStep = bestStepList2[i];
            }
            else
            {
                int i = new Random().Next(bestStepList.Count);
                bestStep = bestStepList[i];
            }

            if (bestStep != null)
            {
                Debug.Assert(bestStep.OriginalX == bestStep.Piece.X && bestStep.OriginalY == bestStep.Piece.Y);
            }
            DelayTime(style);
            return bestStep;
        }

        private void DelayTime(AIStyle style)
        {
            if ((style & AIStyle.ThinkFast) == AIStyle.ThinkFast)
            {
                Thread.Sleep(100);
            }
            else if ((style & AIStyle.ThinkSlow) == AIStyle.ThinkSlow)
            {
                Thread.Sleep(200);
            }
            else
            {

            }
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

        private int EvalScoreOfSituation(FourChessGame game, StepResult result, bool recursive, AIStyle style)
        {
            int score = 0;
            int deadCounterPieces = result.DeadPieces.Where(n => n.Player != this.player).Count();
            score += deadCounterPieces * 10;

            if (recursive)
            {
                AIStyle oneStyle = AIStyle.PreferRandom | AIStyle.ThinkImmediately;
                bool isProfessional = (style & AIStyle.Professional) == AIStyle.Professional;
                if (isProfessional)
                {
                    oneStyle = oneStyle | AIStyle.Professional;
                }

                StepInfo step2 = serviceForOne.InnerNextStep(game, oneStyle, false);
                //FourChessGame game2 = game.DeepCopy();
                StepResult result2 = game.Move(step2);
                {
                    score -= result2.DeadPieces.Count() * 9;
                    var loseGroup = game.GetLosePlayer();
                    if (loseGroup != null && loseGroup.First().Player != this.player)
                    {
                        score += 10000;
                    }

                    //专业选手再多看两步
                    if (isProfessional)
                    {
                        StepInfo step3 = this.InnerNextStep(game, style, false);
                        if (step3 != null)
                        {
                            //FourChessGame game3 = game2.DeepCopy();
                            StepResult result3 = game.Move(step3);
                            {
                                score += result3.DeadPieces.Count() * 8;

                                StepInfo step4 = serviceForOne.InnerNextStep(game, oneStyle, false);
                                if (step4 != null)
                                {
                                    //FourChessGame game4 = game.DeepCopy();
                                    StepResult result4 = game.Move(step4);
                                    score -= result4.DeadPieces.Count() * 7;
                                    game.UnMove(step4, result4);
                                }

                                game.UnMove(step3, result3);
                            }
                        }
                    }

                    game.UnMove(step2, result2);
                }
            }
            return score;
        }
    }
}
