using Encog.MathUtil.Randomize;
using Encog.ML.EA.Population;
using Encog.ML.EA.Species;
using Encog.ML.Genetic;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Power4GoCore
{
    public class Trainer 
    {
        public Trainer()
        {
        }

        public void StartTraining()
        {
            var Scorer = new GameScore();
            var Trainer = new MLMethodGeneticAlgorithm(GetNetwork, Scorer, 20) { ThreadCount = 1 };
            

            //Trainer.ThreadCount = 1;
            var s = new BestStrategy();
            s.Init(Trainer);
            Trainer.Strategies.Add(s);
            for (int i = 0; i < 1000; i++)
            {
                Trainer.Iteration();
                Console.WriteLine(string.Format("Epoch {0} finished : {1} Hash : {2}",i ,Trainer.Genetic.BestGenome.Score, Trainer.Genetic.BestGenome.GetHashCode()));
                if ((i % 10) == 0) SaveTrainer(Trainer);
        }
            SaveTrainer(Trainer);
            System.IO.File.WriteAllText(@".\Result.txt", Trainer.Genetic.BestGenome.ToString());
        }

        void SaveTrainer(MLMethodGeneticAlgorithm Trainer)
        {
            var Basic = ((Trainer.Genetic.BestGenome as MLMethodGenome).Phenotype as BasicNetwork);
            Encog.Persist.EncogDirectoryPersistence.SaveObject(new System.IO.FileInfo("./Network.net"), Basic);
        }
        public class BestStrategy : IStrategy
        {
            List<Adversaries> Adversaries;
            MLMethodGeneticAlgorithm Method;
            public void Init(IMLTrain train)
            {
                Method = (MLMethodGeneticAlgorithm)train;
                var r = new RangeRandomizer(-1, 1);
                Adversaries = new List<Adversaries>();
                var l = new List<double>();
                //for (int i = 0; i < 1000; i++)
                //{
                //    l.Add((double)r.NextDouble());
                //}
                for (int i = 0; i < 6; i++)
                {
                    Adversaries.Add(new Adversaries() { PlayerStart = r.NextDouble() >= 0.5 ? 1.0 : -1.0, Player=new PlayerVertical(i)});
                    Adversaries.Add(new Adversaries() { PlayerStart = r.NextDouble() >= 0.5 ? 1.0 : -1.0, Player=new PlayerHorizontal(i)});
                }
            }

            public void PostIteration()
            {
                //Method.Genetic.ScoreFunction = new GameScore((BasicNetwork)Method.Method);
            }

            public void PreIteration()
            {
                var s = Method.Genetic.ScoreFunction as GameScore;
                var r = new RangeRandomizer(-1, 1);
                s.Adversaries = Adversaries;
                //s.Adversaries = Method.Genetic.Population.Flatten().Select(
                //    x => new Adversaries { Player = new PlayerNetwork((x as MLMethodGenome).Phenotype as BasicNetwork), PlayerStart = r.NextDouble() >= 0.5 ? 1.0 : -1.0 }).ToList();
                //s.Adversaries.AddRange(Adversaries);
            }

        }

        static BasicNetwork GetNetwork()
        {
            var Network = new BasicNetwork();
            //{ BiasActivation = 1.0 };
            var Ac = new Encog.Engine.Network.Activation.ActivationSigmoid();
            Network.Structure.Layers.Add(new BasicLayer(Ac, true,42)) ;
            Network.Structure.Layers.Add(new BasicLayer(Ac, true, 42));
            Network.Structure.Layers.Add(new BasicLayer(Ac, true, 36));
            Network.Structure.Layers.Add(new BasicLayer(Ac, true, 24));
            Network.Structure.Layers.Add(new BasicLayer(Ac, true, 1));
          
            Network.Structure.FinalizeStructure();
            Network.BiasActivation = 1.0;
            Network.Reset();
            return Network;
        }
    }
}
