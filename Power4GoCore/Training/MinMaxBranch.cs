using Encog.Neural.Networks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power4GoCore
{
    public class MinMaxBranch
    {
        private MinMaxBranch Master;
        public GameState Head { get; private set; }
        public List<MinMaxBranch> Leafs { get; private set; }
        public double Player;
        public double Score;

        public MinMaxBranch(GameState head, double PlayerNumber, MinMaxBranch CurrentMaster = null)
        {
            Master = CurrentMaster;
            Player = PlayerNumber;
            Head = head;
            Leafs = new List<MinMaxBranch>();
        }

        public void Generate()
        {
            Leafs = MinMaxAlgorithm.GetPossible(Head, Player).Select(x => new MinMaxBranch(x, Player * -1, this)).ToList();
            foreach (var item in Leafs) item.Head.ComputeState();
        }

        public GameState GetFirstChoice()
        {
            var Current = this;
            while (Current.Master != null) Current = Current.Master;
            return Current.Head;
        }

        public double GetScore()
        {
            Score = Leafs.Any() ? Leafs.Min(x => x.Score) : 0;
            return Score;
        }

    }
}
