using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power4GoCore
{
    public class MinMaxBranch
    {
        private GameState State;
        private int Steps;
        public double Score { get; private set; }
        public MinMaxBranch(GameState InitialStep, int Depth)
        {
            State = InitialStep;
            Steps = Depth;
        }
        public GameState GetBest()
        {
            var CurrentDeph = Steps;
            var First = GetPossible(State, 1);
            if (First.Any(x => x.Winner == 1)) return First.First(x => x.Winner == 1);
            var Current = First;
            while (CurrentDeph > 0)
            {
                var Adversary = new List<GameState>();
                foreach (var item in Current) Adversary.AddRange(GetPossible(item, -1));
                CurrentDeph--;
            }


            return First[0];

        }
        public List<GameState> GetPossible(GameState State, double Player)
        {
            var L = new List<GameState>();
            for (int i = 0; i < 6; i++)
            {
                if (State.Available[i] < 5)
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
