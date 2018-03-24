using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;

namespace Power4Go
{
    public class P4GAgent
    {
        BasicNetwork Network;

        public P4GAgent()
        {
            Init();
        }

        public void Init()
        {
            Network = new BasicNetwork() { BiasActivation = 1.0 };
            Network.Structure.Layers.Add(new BasicLayer(72));
            Network.Structure.Layers.Add(new BasicLayer(48));
            Network.Structure.Layers.Add(new BasicLayer(48));
            Network.Structure.Layers.Add(new BasicLayer(48));
            Network.Structure.Layers.Add(new BasicLayer(1));
            Network.Structure.FinalizeStructure();
            Network.Reset();
        }

        public double Score(BasicMLData Input)
        {
            return (Network.Compute(Input) as BasicMLData).Data[0];
        }
    }
}
