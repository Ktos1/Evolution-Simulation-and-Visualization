using ProjectEvolution.CommonStuff;

namespace ProjectEvolution.Visualization.LogicScripts
{
    /// <summary>
    /// Represents a chromosome in the visualization part.
    /// </summary>
    public class VChromosome : Chromosome<VGene>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VChromosome"/> class.
        /// </summary>
        /// <param name="genesValues">
        /// The values of the genes to initialize the chromosome with.
        /// </param>
        public VChromosome(float[] genesValues) : base()
        {
            for (int i = 0; i < _genes.Length; i++)
            {
                _genes[i].Value = genesValues[i];
            }
        }
    }
}
