using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power4GoCore
{
    public class GameState
    {
        public int[] Available { get; private set; }
        private List<List<double>> Grid;
        public int Winner { get; private set; }
        public int BestChain { get; private set; }
        public string Winning { get; private set; }
        public bool IsOver { get; private set; }
        public bool IsDraw { get; private set; }
        public int Round { get; set; }
        private PlayerState Player1;
        private PlayerState Player2;

        public static GameState NewGame()
        {
            var g = new GameState();
            g.Player1 = new PlayerState();
            g.Player1.Init(); 
            g.Player2 = new PlayerState();
            g.Player2.Init();
            g.Round = 0;
            g.Available = new int[] { 0,0,0,0,0,0,0};
            g.Grid = new List<List<double>>();
            for (int i = 1; i <= 7; i++)
            {
                g.Grid.Add(new List<double> { 0, 0, 0, 0, 0, 0 });
            }
            return g;
        }

        public void PrintDebug()
        {
            var s = new StringBuilder();
            for (int j = 5; j >= 0; j--)
            {
                for (int i = 0; i < 7; i++)
                {
                    var val = Grid[i][j];
                    if (val > 0) s.Append("O");
                    else if (val < 0) s.Append("X");
                    else s.Append("_");

                    //Console.WriteLine(string.Join(" ",Grid[i].Select(x=>x.ToString()).ToArray()));
                }
                s.AppendLine();
            }
            Console.Write(s.ToString());
        }

        public GameState PutOne(int Index, double Value)
        {
            
            var g = (GameState)MemberwiseClone();
            g.Player1 = Player1.Clone();
            g.Player2 = Player2.Clone();
            g.Available = Available.ToArray();
            g.Grid = new List<List<double>>();
            for (int i = 0; i < 7; i++)
            {
                g.Grid.Add(Grid[i].ToList());
            }
            g.Grid[Index][Available[Index]]= Value;
            var p = new GamePosition((byte)Index, (byte)Available[Index]);
            if (Value > 0)
            {
                ChainOfFour(g.Player1.Positions, g.Player1.Chains,p);
                g.Player1.Positions.Add(p);
            }
            else
            {
                ChainOfFour(g.Player2.Positions, g.Player2.Chains,p);
                g.Player2.Positions.Add(p);
            }
            g.BestChain = g.Player1.Chains.Any() ? g.Player1.Chains.Max(x => x.Chain.Count): 0;
            g.Available[Index]++;
            g.Round++;
            return g;
        }

        public BasicMLData GetData()
        {
            return new BasicMLData(GetDouble(), false);
        }

        private double[] GetDouble()
        {
            var l = new List<double>();
            foreach (var Vec in Grid)
            {
                foreach (var Ind in Vec)
                {
                    l.Add(Ind);
                }
            }
            return l.ToArray();
        }

        public void ComputeState()
        {
            var Chain1 = Player1.Chains.Any() ? Player1.Chains.OrderBy(x => x.Chain.Count).Last(): null;
            var Chain2 = Player2.Chains.Any() ? Player2.Chains.OrderBy(x => x.Chain.Count).Last() : null;
            if (Chain1 != null && Chain1.Chain.Count >3) { Winner = 1; Winning = Chain1.ToString(); IsOver = true; IsDraw = false; }
            else if(Chain2 != null && Chain2.Chain.Count > 3) { Winner = -1; Winning = Chain2.ToString(); IsOver = true; IsDraw = false; }
            else if (Grid.Sum(x => x.Where(y=>y!=0.0).Count()) >= 42) { IsOver = true; IsDraw = true; }
        }
        private class PlayerState
        {
            public List<FourChain> Chains;
            public List<GamePosition> Positions;
            public void Init()
            {
                Chains = new List<FourChain>();
                Positions = new List<GamePosition>();
            }
            
            public PlayerState Clone()
            {
                var p =(PlayerState) MemberwiseClone();
                p.Chains = Chains.Select(x=>x.Clone()).ToList();
                p.Positions = Positions.ToList();
                return p;
            }
        }
        private class GamePosition
        {
            public byte X { get; private set; }
            public byte Y { get; private set; }
            public GamePosition(byte XValue, byte YValue)
            {
                X = XValue;
                Y = YValue;
            }
            public bool Equals(GamePosition obj)
            {
                return obj.X==X && obj.Y==Y;
            }
            public override string ToString()
            {
                return string.Format("{0}:{1}",X,Y);
            }
        }
        private enum FourDirection
        {
            Vertical,Horizontal,Slash,AntiSlash
        }
        private class FourChain
        {
            public FourDirection Direction { get; private set; }
            public List<GamePosition> Chain { get; private set; }
            public FourChain(GamePosition p1, GamePosition p2)
            {
                if (p2.Y < p1.Y) (p1, p2) = (p2, p1);
                Chain = new List<GamePosition>() { p1, p2 };
                if (p1.X == p2.X) Direction = FourDirection.Vertical;
                else if (p1.Y == p2.Y) Direction = FourDirection.Horizontal;
                else if (p1.X > p2.X) Direction = FourDirection.AntiSlash;
                else Direction = FourDirection.Slash;
            }
            public bool IsContituent( GamePosition pNew)
            {
                if (Chain.Any(x => x.Equals(pNew))) return false;
                if (Direction == FourDirection.Vertical)
                    if (Chain.Any(p => pNew.X.Equals(p.X) && Math.Abs(pNew.Y-p.Y) == 1)) { Chain.Add(pNew); return true; }
                if (Direction == FourDirection.Horizontal)
                    if (Chain.Any(p => pNew.Y.Equals(p.Y) && Math.Abs(pNew.X - p.X) == 1) ) { Chain.Add(pNew); return true; }
                if (Direction == FourDirection.Slash)
                    if (Chain.Any(p => (pNew.Y - p.Y) == 1 && pNew.X - p.X == 1)) { Chain.Add(pNew); return true; }
                if (Direction == FourDirection.AntiSlash)
                    if (Chain.Any(p => (pNew.Y - p.Y) == 1 && pNew.X - p.X == -1)) { Chain.Add(pNew); return true; }
                return false;
            }
            public override string ToString()
            {
                return Direction.ToString() + " " + string.Join(",",Chain.Select(x=>x.ToString()).ToArray());
            }
            public bool IsSame(FourChain c)
            {
                return (c.Chain.All(x => Chain.Any(y=>y.Equals(x))));
            }

            public FourChain Clone()
            {
                var f = (FourChain)MemberwiseClone();
                f.Chain = Chain.ToList();
                return f;
            }
         
        }

        private void ChainOfFour(List<GamePosition> List, List<FourChain> Chains,GamePosition NewPoint )
        {
                var Bool = Chains.Where(x => x.IsContituent(NewPoint)).Any();
                if (!Bool)
                {
                    var Near = List.Where( point => !point.Equals(NewPoint) &&  Math.Abs(NewPoint.X - point.X) < 2 && Math.Abs(NewPoint.Y - point.Y) < 2).ToList();
                    foreach (var p in Near)
                    {
                        var c = new FourChain(NewPoint, p);
                        if (!Chains.Any(x => x.IsSame(c))) Chains.Add(c);
                    }
                }
        }
        public double GetScore(BasicNetwork NN)
        {
            return ((BasicMLData)NN.Compute(GetData()))[0];
        }
    }
}
