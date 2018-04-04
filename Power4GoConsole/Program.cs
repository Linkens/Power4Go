using Power4GoCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power4Go
{
    class Program
    {
        static void Main(string[] args)
        {
            //var Agent = new P4GAgent();
            var T = new Trainer();
            T.StartTraining();
        }
    }
}
