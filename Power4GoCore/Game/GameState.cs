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
        public byte BestChain { get; private set; }
        public string Winning { get; private set; }
        public bool IsOver { get; private set; }
        public bool IsDraw { get; private set; }
        public int Round { get; set; }

        public static GameState NewGame()
        {
            var g = new GameState();
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
            g.Available = Available.ToArray();
            g.Grid = new List<List<double>>();
            for (int i = 0; i < 7; i++)
            {
                g.Grid.Add(Grid[i].ToList());
            }
            g.Grid[Index][Available[Index]]= Value;
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
            var Player1 = new List<GamePosition>();
            var Player2 = new List<GamePosition>();
            for (byte i = 0; i < 7; i++)
            {
                for (byte j = 0; j < 6; j++)
                {
                    var Value = Grid[i][j];
                    if (Value > 0) Player1.Add(new GamePosition(j, i));
                    else if (Value < 0) Player2.Add(new GamePosition(j, i));
                }
            }
            var Chain1 = ChainOfFour(Player1);
            BestChain = (Chain1 != null) ? (byte)Chain1.Chain.Count : (byte)0;
            var Chain2 = ChainOfFour(Player2);
            if(Chain1 !=null && Chain1.Chain.Count >3) { Winner = 1; Winning = Chain1.ToString(); IsOver = true; IsDraw = false; }
            else if( Chain2 != null && Chain2.Chain.Count > 3) { Winner = -1; Winning = Chain2.ToString(); IsOver = true; IsDraw = false; }
            else if (Grid.Sum(x => x.Where(y=>y!=0.0).Count()) >= 42) { IsOver = true; IsDraw = true; }
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
         
        }

        private FourChain ChainOfFour(List<GamePosition> List)
        {
            List<FourChain> Chains = new List<FourChain>();
            foreach (var item in List)
            {
                var Bool = Chains.Where(x => x.IsContituent(item)).Any();
                if (!Bool)
                {
                    var Near = List.Where( point => !point.Equals(item) &&  Math.Abs(item.X - point.X) < 2 && Math.Abs(item.Y - point.Y) < 2).ToList();
                    foreach (var p in Near)
                    {
                        var c = new FourChain(item, p);
                        if (!Chains.Any(x => x.IsSame(c))) Chains.Add(c);
                    }
                }
            }
            if (Chains.Any())
            {
                var m = Chains.OrderBy(x => x.Chain.Count).Last();
                return m;
            }
            else return null;
        }
        public double GetScore(BasicNetwork NN)
        {
            return ((BasicMLData)NN.Compute(GetData()))[0];
        }
    }
}
