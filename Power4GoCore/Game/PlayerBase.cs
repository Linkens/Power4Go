using Encog.Neural.Networks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power4GoCore
{
    public abstract class PlayerBase
    {
        public double BaseScore;
        public abstract GameState RunOneStep(GameState CurrentStart, double PlayerNumber);
    }
    public class PlayerDumb : PlayerBase
    {
        protected Func<GameState, double, GameState> DumbFunc;
        public override GameState RunOneStep(GameState CurrentStart, double PlayerNumber)
        {
            return DumbFunc(CurrentStart, PlayerNumber);
        }
    }
    public class PlayerVertical : PlayerDumb
    {
        int Lane;
        public PlayerVertical(int FirstLane)
        {
            BaseScore = 1000;
            DumbFunc = VerticalStuff;
            Lane = FirstLane;
        }
        private GameState VerticalStuff(GameState State, double Player)
        {
            while (State.Available[Lane] > 5)
            {
                Lane++;
                if (Lane > 6) Lane = 0;
            }
            var s = State.PutOne(Lane, Player);
            s.ComputeState();
            return s;
        }
    }
    public class PlayerHorizontal : PlayerDumb
    {
        int Lane;
        public PlayerHorizontal(int FirstLane)
        {
            BaseScore = 1000;
            DumbFunc = VerticalStuff;
            Lane = FirstLane;
        }
        private GameState VerticalStuff(GameState State, double Player)
        {
            Lane++;
            if (Lane > 6) Lane = 0;
            while (State.Available[Lane] > 5)
            {
                Lane++;
                if (Lane > 6) Lane = 0;
            }
            var s =State.PutOne(Lane, Player);
            s.ComputeState();
            return s;
        }
    }
    public class PlayerNetwork : PlayerBase
    {
        BasicNetwork Net;
        public PlayerNetwork(BasicNetwork Network)
        {
            BaseScore = 100;
            Net = Network;
        }
        public override GameState RunOneStep(GameState CurrentStart, double PlayerNumber)
        {
            var Algo = new MinMaxAlgorithm(CurrentStart, 1, PlayerNumber);
            return Algo.GetBest(Net);
        }
    }
}
