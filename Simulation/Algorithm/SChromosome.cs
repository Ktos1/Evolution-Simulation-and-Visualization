using ProjectEvolution.CommonStuff;
using System.Linq;

namespace ProjectEvolution.Simulation.Algorithm
{
    internal class SChromosome : Chromosome<SGene>
    {
        public SChromosome() : base()
        {
            for (int i = 0; i < _genes.Length; i++)
            {
                var gene = _genes[i];
                float range;

                if (gene == EnergyAmountToStartFoodSearchGene)
                    range = EnergyAmountToStartPartnerSearchGene.Value - gene.MinValue;
                else
                    range = gene.MaxValue - gene.MinValue;

                gene.Value = _randGen.NextSingle() * range + gene.MinValue;
            }
        }

        public SChromosome(SGene[] genes)
        {
            _genes = genes;
        }

        public SChromosome GetChildChromosome (SChromosome partnerChromosome)
        {
            SChromosome childChromosome;
            if (_randGen.NextSingle() < 0.25f)
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
        }

        private SChromosome Crossover(SChromosome partnerChromosome)
        {
            var cutPlace = _randGen.Next(1, _genes.Length - 1);
            var thisPart = Genes.Take(cutPlace);
            var partnerPart = partnerChromosome.Genes.Skip(cutPlace);

            return new SChromosome(thisPart.Concat(partnerPart).ToArray()) ;
        }
    }
}
