using Encog.MathUtil.Randomize;
using Encog.ML;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using System;
using System.Collections.Generic;

namespace Power4GoCore
{
    public class GameScore : ICalculateScore
    {
        public GameScore() { }

        public List<Adversaries> Adversaries;
        public bool ShouldMinimize => false;

        public bool RequireSingleThreaded => false;

        public double CalculateScore(IMLMethod network)
        {
            var Player1 = new PlayerNetwork((BasicNetwork)network);
            var Score = 0d;
            foreach (var Player2 in Adversaries)
            {
                var Game = PlayOneRound(Player1, Player2.Player, Player2.PlayerStart);
                Score += ScoreGame(Game, Player2);
            }
            return Score;
        }
        public static GameState PlayOneRound(PlayerBase P1, PlayerBase P2, double StartingPlayer)
        {
            var Game = GameState.NewGame();
            while (!Game.IsOver)
            {
                Game = PlayCycle(Game, StartingPlayer, P1, P2);
                StartingPlayer = StartingPlayer * -1;
            }
            return Game;
        }
        public static double ScoreGame(GameState Game, Adversaries Adversary)
        {
            var Score = 0.0;
            if (Game.Winner == 1) Score = Adversary.Player.BaseScore + 24 - Math.Floor(Game.Round / 2.0) * 2.0;
            else if (Game.Winner == -1) Score = -Adversary.Player.BaseScore + Game.BestChain;
            else Score += Game.BestChain;
            return Score;
        }

        private static GameState PlayCycle(GameState Game, double Player, PlayerBase P1, PlayerBase P2)
        {
            var P = Player > 0 ? P1 : P2;
            //Game.PrintDebug();
            //Game.ComputeState();
            return P.RunOneStep(Game, Player);
        }

    }
}
