using Encog.MathUtil.Randomize;
using Encog.ML;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using System;
using System.Collections.Generic;

namespace Power4GoCore
{
    public static class Tools
    {
        public static SimpleGameAI GetAI()
        {
            var Net = Encog.Persist.EncogDirectoryPersistence.LoadObject(new System.IO.FileInfo("./Network.net")) as BasicNetwork;
            return new SimpleGameAI(Net);
        }
    }
}
