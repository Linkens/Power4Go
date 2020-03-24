using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power4GoCore
{
    public class MinMaxAlgorithm
    {
        private GameState State;
        private int Steps;
        private double Player;
        public double Score { get; private set; }
        public MinMaxAlgorithm(GameState InitialStep, int Depth,double InitialPlayer)
        {
            Player = InitialPlayer;
            State = InitialStep;
            Steps = Depth;
        }
        public GameState GetBest(BasicNetwork Network)
        {
            var CurrentDepth = Steps;
            var First = GetPossible(State, Player);
            if (First.Any(x => x.Winner == 1)) return First.First(x => x.Winner == 1);
            if (First.Count == 1) return First.First();
            var Current = First.Select(x=>new MinMaxBranch(x,Player*-1)).ToList();
            List<MinMaxBranch> Next;
            var FirstBool = true;
            while (CurrentDepth > 0)
            {
                if (!FirstBool)
                {
                    Next = new List<MinMaxBranch>();
                    foreach (var item in Current)
                    {
                        Next.AddRange(item.Leafs);
                        foreach (var Leaf in item.Leafs)Leaf.Generate();
                    }
                    Current = Next;
                }
                foreach (var item in Current) item.Generate();
                FirstBool = false;
                CurrentDepth--;
                if (Current.Any(x => x.Head.IsOver)) CurrentDepth = 0;
            }
            var BestState = GetMinMax(Current, Network);
            return BestState;
        }

        private GameState GetMinMax(List<MinMaxBranch> Branches,BasicNetwork Network)
        {
            foreach (var b in Branches) {
                foreach (var l in b.Leafs)
                {
                    var Compute = ((BasicMLData)Network.Compute(l.Head.GetData()));
                l.Score = Compute.Data[0];
                }
                    }
            return Branches.OrderBy(x => x.GetScore()).First().GetFirstChoice();
        }


        public static List<GameState> GetPossible(GameState State, double Player)
        {
            var L = new List<GameState>();
            for (int i = 0; i < 7; i++)
            {
                if (State.Available[i] < 6)
                {
                    var NewGame = State.PutOne(i, Player);
                    NewGame.ComputeState();
                    L.Add(NewGame);
                }
            }
            return L;
        }
    }
}
