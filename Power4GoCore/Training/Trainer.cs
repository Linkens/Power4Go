using System;
using System.Collections.Generic;
using System.Linq;
using Encog.ML.Genetic.Crossover;
using Encog.ML.Genetic.Mutate;
using System.Threading.Tasks;
using Encog.ML.Genetic;
using Encog.ML.Genetic.Genome;
using Encog.MathUtil.Randomize;
using Encog.ML.Train;
using Encog.ML;
using Encog.ML.Data;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks.Training.Propagation;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.ML.Data.Basic;

namespace Power4GoCore
{
    public class Trainer 
    {
        public Trainer()
        {
        }

        public void StartTraining()
        {
            var Scorer = new GameScore(GetNetwork());
            var Trainer = new MLMethodGeneticAlgorithm(GetNetwork, Scorer, 100);
            //Trainer.ThreadCount = 1;
            var s = new BestStrategy();
            s.Init(Trainer);
            Trainer.Strategies.Add(s);
            for (int i = 0; i < 1000; i++)
            {
                Trainer.Iteration();
            }
            System.IO.File.WriteAllText(@".\Result.txt", Trainer.Genetic.BestGenome.ToString());
        }
        public class BestStrategy : IStrategy
        {
            MLMethodGeneticAlgorithm Method;
            public void Init(IMLTrain train)
            {
                Method = (MLMethodGeneticAlgorithm)train;
            }

            public void PostIteration()
            {
                Method.Genetic.ScoreFunction = new GameScore((BasicNetwork)Method.Method);
            }

            public void PreIteration()
            {
            }
        }

        BasicNetwork GetNetwork()
        {
            var Network = new BasicNetwork();
            //{ BiasActivation = 1.0 };
            Network.Structure.Layers.Add(new BasicLayer(42));
            Network.Structure.Layers.Add(new BasicLayer(42));
            Network.Structure.Layers.Add(new BasicLayer(36));
            Network.Structure.Layers.Add(new BasicLayer(24));
            Network.Structure.Layers.Add(new BasicLayer(1));
            Network.Structure.FinalizeStructure();
            Network.BiasActivation = 1.0;
            Network.Reset();
            return Network;
        }
        public class GameScore : ICalculateScore
        {
            public GameScore ( BasicNetwork Adversary) { Player2 = Adversary; }
            BasicNetwork Player2;
            BasicNetwork Player1;
            public bool ShouldMinimize => false;

            public bool RequireSingleThreaded => false;

            public double CalculateScore(IMLMethod network)
            {
                Player1 = (BasicNetwork)network;
                var Round = 0;
                var CurrentPlayer = new RangeRandomizer(-1, 1).NextDouble() >= 0 ? 1.0 : -1.0;
                var Game = GameState.NewGame();
                while (!Game.IsOver)
                {
                    Round++;
                    Game = PlayCycle(Game, CurrentPlayer);
                    CurrentPlayer = CurrentPlayer * -1;
                }
                var Score=0.0;
                if (Game.Winner == 1) Score = 100 + 36 - Math.Floor( Round/2.0)*2.0;
                else if (Game.Winner == -1) Score = -100 + Game.BestChain;
                else Score += Game.BestChain;
                return Score;
            }
            private GameState PlayCycle(GameState Game, double Player) {
                var Network = Player > 0 ? Player1 : Player2;
                var Algo = new MinMaxAlgorithm(Game, 2,Player);
                return Algo.GetBest(Network);
            }
        }

            //private GameState PlayOnce(BasicNetwork Network, GameState Game)
            //{
            //    var g= GetMax(Network,Game);
            //    g=GetMax(Network,g);
            //    g=GetMax(Network,g);
            //    return g;
            //}

            //private GameState GetMax(BasicNetwork Network, GameState game)
            //{
            //    List<GameState> L = OneStep(game, Network, 1, out List<double> Scores);
            //    GameState Max = L[0];
            //    var MaxValue = double.MinValue;
            //    for (int i = 0; i < Scores.Count; i++)
            //    {
            //        if (MaxValue < Scores[i])
            //        {
            //            MaxValue = Scores[i];
            //            Max = L[i];
            //        }
            //    }
            //    return Max;
            //}
            //private GameState GetMin(BasicNetwork Network, GameState game)
            //{
            //    List<GameState> L = OneStep(game, Network, -1, out List<double> Scores);
            //    GameState Min = L[0];
            //    var MinValue = double.MaxValue;
            //    for (int i = 0; i < Scores.Count; i++)
            //    {
            //        if (MinValue > Scores[i])
            //        {
            //            MinValue = Scores[i];
            //            Min = L[i];
            //        }
            //    }
            //    return Min;
            //}
            //private List<GameState> OneStep(GameState Game, BasicNetwork Network, double Player, out List<double> Score)
            //{
            //    var L = new List<GameState>();
            //    var S = new List<double>();
            //    for (int i = 0; i < 6; i++)
            //    {
            //        if (Game.Available[i] < 5) {
            //            var NewGame = Game.PutOne(i, Player);
            //            L.Add(NewGame);
            //            S.Add(Network.Compute(NewGame.GetData())[0]);
            //        }
            //    }
            //    Score = S;
            //    return L;
            //}
        

    }
}
