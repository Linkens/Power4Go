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
        //List<DoubleArrayGenome> CurrentPop;

        //GenomeFactory Factory;
        //RangeRandomizer Randomizer;
        //Splice Splicer;
        //MutatePerturb Mutator;

        public Trainer()
        {
            //Factory = new GenomeFactory(12);
            //Splicer = new Splice(15);
            //Randomizer= new RangeRandomizer(-1, 1);
            //Mutator = new MutatePerturb(0.2f);
            //var Trainer = new MLMethodGeneticAlgorithm(GetNetwork,,500);
        }

        BasicNetwork GetNetwork()
        {
            var Network = new BasicNetwork() { BiasActivation = 1.0 };
            Network.Structure.Layers.Add(new BasicLayer(72));
            Network.Structure.Layers.Add(new BasicLayer(48));
            Network.Structure.Layers.Add(new BasicLayer(48));
            Network.Structure.Layers.Add(new BasicLayer(48));
            Network.Structure.Layers.Add(new BasicLayer(1));
            Network.Structure.FinalizeStructure();
            Network.Reset();
            return Network;
        }
        public class GameScore : ICalculateScore
        {
            public bool ShouldMinimize => false;

            public bool RequireSingleThreaded => false;

            public double CalculateScore(IMLMethod network)
            {
                var Network = (BasicNetwork)network;
                var Round = 0;
                var Game = GameState.NewGame();
                while (!Game.IsOver)
                {
                    Round++;
                    PlayOnce(Network, Game);

                }
                var Score=0.0;
                if (Game.Winner == 1) Score = 100 + 36 - Round;
                else if (Game.Winner == -1) Score = -100 + Game.BestChain;
                else Score += Game.BestChain;
                return Score;
            }
            private GameState PlayCycle(BasicNetwork Network, GameState Game)
            {

            }

            private GameState PlayOnce(BasicNetwork Network, GameState Game)
            {
                var g= GetMax(Network,Game);
                g=GetMax(Network,g);
                g=GetMax(Network,g);
                return g;
            }

            private GameState GetMax(BasicNetwork Network, GameState game)
            {
                List<GameState> L = OneStep(game, Network, 1, out List<double> Scores);
                GameState Max = L[0];
                var MaxValue = double.MinValue;
                for (int i = 0; i < Scores.Count; i++)
                {
                    if (MaxValue < Scores[i])
                    {
                        MaxValue = Scores[i];
                        Max = L[i];
                    }
                }
                return Max;
            }
            private GameState GetMin(BasicNetwork Network, GameState game)
            {
                List<GameState> L = OneStep(game, Network, -1, out List<double> Scores);
                GameState Min = L[0];
                var MinValue = double.MaxValue;
                for (int i = 0; i < Scores.Count; i++)
                {
                    if (MinValue > Scores[i])
                    {
                        MinValue = Scores[i];
                        Min = L[i];
                    }
                }
                return Min;
            }
            private List<GameState> OneStep(GameState Game, BasicNetwork Network, double Player, out List<double> Score)
            {
                var L = new List<GameState>();
                var S = new List<double>();
                for (int i = 0; i < 6; i++)
                {
                    if (Game.Available[i] < 5) {
                        var NewGame = Game.PutOne(i, Player);
                        L.Add(NewGame);
                        S.Add(Network.Compute(NewGame.GetData())[0]);
                    }
                }
                Score = S;
                return L;
            }
        }

    }
}
