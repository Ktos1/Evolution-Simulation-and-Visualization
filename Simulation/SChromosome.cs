using ProjectEvolution.CommonStuff;
using System.Linq;

namespace ProjectEvolution.Simulation
{
    /// <summary>
    /// Represents a chromosome in the simulation part.
    /// </summary>
    public class SChromosome : Chromosome<SGene>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SChromosome"/> class with random
        /// genes values.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SChromosome"/> class with 
        /// the given genes.
        /// </summary>
        /// <param name="genes">
        /// The genes to initialize the chromosome with.
        /// </param>
        private SChromosome(SGene[] genes)
        {
            _genes = new SGene[genes.Length];
            for (int i = 0; i < genes.Length; i++)
            {
                _genes[i] = genes[i].Clone();
            }
        }

        /// <summary>
        /// Creates a new chromosome with using a genetic operators.
        /// </summary>
        /// <remarks>
        /// The new chromosome is created by crossing over the creature chromosome and the 
        /// partner chromosome. If the crossover does not happen, the new chromosome is a
        /// clone of the creature chromosome. The new chromosome is then mutated.
        /// </remarks>
        /// <param name="partnerChromosome">
        /// The chromosome to cross over with.
        /// </param>
        /// <returns>
        /// The new created child chromosome.
        /// </returns>
        public SChromosome GetChildChromosome(SChromosome partnerChromosome)
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

        /// <summary>
        /// Gets the values of the genes in the chromosome.
        /// </summary>
        /// <returns>
        /// The values of the genes in the chromosome.
        /// </returns>
        public float[] GetGenesValues()
        {
            return _genes.Select((gene) => gene.Value).ToArray();
        }

        /// <summary>
        /// Mutates the chromosome.
        /// </summary>
        /// <remarks>
        /// The mutation is done by iterating over the genes in the chromosome and mutating
        /// them with a probability of <see cref="SimulationSettings.MutationChance"/>.
        /// </remarks>
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

        /// <summary>
        /// Crosses over the chromosome with the given partner chromosome.
        /// </summary>
        /// <remarks>
        /// The crossover is done by taking the first part of the creature chromosome and
        /// the second part of the partner chromosome. The cut place is chosen randomly.
        /// </remarks>
        /// <param name="partnerChromosome">
        /// The chromosome to cross over with.
        /// </param>
        /// <returns>
        /// The new created chromosome from the crossover.
        /// </returns>
        private SChromosome Crossover(SChromosome partnerChromosome)
        {
            var cutPlace = _randGen.Next(1, _genes.Length - 1);
            var thisPart = Genes.Take(cutPlace);
            var partnerPart = partnerChromosome.Genes.Skip(cutPlace);

            return new SChromosome(thisPart.Concat(partnerPart).ToArray());
        }

        /// <summary>
        /// Checks and  the dependencies between the genes in the chromosome.
        /// </summary>
        /// <remarks>
        /// If some dependencies are not met, the genes are adjusted to meet them.
        /// </remarks>
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
