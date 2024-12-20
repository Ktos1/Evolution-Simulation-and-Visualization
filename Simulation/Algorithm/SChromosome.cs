using ProjectEvolution.CommonStuff;
using System.Linq;

namespace ProjectEvolution.Simulation.Algorithm
{
    public class SChromosome : Chromosome<SGene>
    {
        public SChromosome() : base()
        {
            for (int i = 0; i < _genes.Length; i++)
            {
                var gene = _genes[i];
                float range;

                if (gene == EnrgAmntToStrtFdSrchGene)
                    range = EnrgAmntToStrtPrtnrSrchGene.Value - gene.MinValue;
                else
                    range = gene.MaxValue - gene.MinValue;

                gene.Value = _randGen.NextSingle() * range + gene.MinValue;
            }
            CheckDependencies();
        }

        private SChromosome(SGene[] genes)
        {
            _genes = new SGene[genes.Length];
            for (int i = 0; i < genes.Length; i++)
            {
                _genes[i] = genes[i].Clone();
            }
        }

        public SChromosome GetChildChromosome (SChromosome partnerChromosome)
        {
            SChromosome childChromosome;
            if (_randGen.NextSingle() < SimulationSettings.CrossoverChance)
            {
                childChromosome = Crossover(partnerChromosome);
            }
            else
            {
                childChromosome = new SChromosome(Genes);
            }
            childChromosome.Mutate();
            return childChromosome;
        }

        public float[] GetGenesValues()
        {
            return _genes.Select((gene) => gene.Value).ToArray();
        }

        private void Mutate()
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                if (_randGen.NextSingle() <= SimulationSettings.MutationChance)
                {
                    _genes[i].Mutate();
                }
            }
            CheckDependencies();
        }

        private SChromosome Crossover(SChromosome partnerChromosome)
        {
            var cutPlace = _randGen.Next(1, _genes.Length - 1);
            var thisPart = Genes.Take(cutPlace);
            var partnerPart = partnerChromosome.Genes.Skip(cutPlace);

            return new SChromosome(thisPart.Concat(partnerPart).ToArray()) ;
        }

        private void CheckDependencies()
        {
            var toStartFoodSearch = EnrgAmntToStrtFdSrchGene.Value * 0.01;
            var toStartPartnerSearch = EnrgAmntToStrtPrtnrSrchGene.Value * 0.01;
            var energyAmount = MaxEnergyAmountGene.Value;

            if (toStartFoodSearch * energyAmount < SimulationSettings.ReproductionCost)
            {
                EnrgAmntToStrtFdSrchGene.Value = 
                    SimulationSettings.ReproductionCost / energyAmount * 100;
            }

            if (toStartPartnerSearch * energyAmount < SimulationSettings.ReproductionCost)
            {
                EnrgAmntToStrtPrtnrSrchGene.Value =
                    SimulationSettings.ReproductionCost / energyAmount * 100;
            }
        }
    }
}
