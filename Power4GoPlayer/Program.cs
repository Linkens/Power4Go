using Power4GoCore;
using System;
using System.Collections.Generic;

namespace Power4GoPlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var g =Tools.GetAI();
                var Player = (new Random()).NextDouble() > 0.5 ? 1 : -1;
                while (!g.State.IsOver)
                {
                    if(Player > 0)
                    {
                        Console.WriteLine("Your turn");
                        Console.WriteLine("0123456");
                        g.State.PrintDebug();

                        int Val;
                        string Input; 
                        do
                        {
                            Input = "" + Console.ReadKey().KeyChar;
                        } while (!g.IsUserInputValid(Input, out Val));
                        Console.WriteLine();

                        g.UserTurn(Input);
                    }
                    else
                    {
                        g.ComputerTurn();
                    }
                    Player *= -1;
                }
                    g.State.PrintDebug();
                Console.WriteLine("Winner is player : " + g.State.Winner + "Press key to start a new one");
                Console.ReadKey();
            }
        }
    }
}
