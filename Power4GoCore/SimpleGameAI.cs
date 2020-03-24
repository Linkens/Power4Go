using Encog.Neural.Networks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power4GoCore
{
    public class SimpleGameAI
    {
        public GameState State { get; protected set; }
        BasicNetwork _Brain;
        public SimpleGameAI(BasicNetwork Brain)
        {
            _Brain = Brain;
            State = GameState.NewGame();
        }

        public bool IsUserInputValid(string Input, out int Value)
        {
            var b=int.TryParse(Input, out int Result);
            if (!b || Result < 0 || Result > 6 || State.Available[Result] == 6) {
                Value = -1;
                return false;
            }
            Value = Result;
            return true;
        }

        public bool UserTurn(string Input)
        {
            if (IsUserInputValid(Input, out int Value)) { State = State.PutOne(Value, 1); State.ComputeState(); return true; }
            else { Console.WriteLine("Utiliser un nombre correct"); return false; }
        }

        public void ComputerTurn()
        {
            var Algo = new MinMaxAlgorithm(State, 2, -1);
            State = Algo.GetBest(_Brain);
        }



    }
}
