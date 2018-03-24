using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.ML.Data.Basic;
using Encog.ML.EA.Genome;
using Encog.ML.Genetic.Genome;

namespace Power4GoCore
{
    public class GenomeFactory : Encog.ML.EA.Genome.IGenomeFactory
    {
        private int _Total;
        public GenomeFactory(int TotalNodes)
        {
            _Total = TotalNodes;
        }
        public IGenome Factor()
        {
            return new DoubleArrayGenome(_Total);
        }

        public IGenome Factor(IGenome other)
        {
            return new DoubleArrayGenome(other.Size);
        }
    }
}
