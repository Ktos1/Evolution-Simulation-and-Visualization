using MathNet.Numerics.Distributions;
using ProjectEvolution.CommonStuff;

namespace ProjectEvolution.Simulation
{
    /// <summary>
    /// Represents a gene in the simulation part.
    /// </summary>
    public class SGene : Gene
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SGene"/> class.
        /// </summary>
        public SGene() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SGene"/> class.
        /// </summary>
        /// <param name="minValue">
        /// The minimum value of the gene.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value of the gene.
        /// </param>
        /// <param name="value">
        /// The value of the gene.
        /// </param>
        private SGene(float minValue, float maxValue, float value)
            : base(minValue, maxValue, value) { }

        /// <summary>
        /// Mutates the gene.
        /// </summary>
        /// <remarks>
        /// The gene is mutated by adding a random value to the current value. 
        /// The random value is generated from a normal distribution with 
        /// a standard deviation defined as a percentage of the possible range
        /// of the gene specified by the <see cref="SimulationSettings.MutationStdDev"/>.
        /// The new value is then checked to be in the range of the gene. If it is not, 
        /// it is set to the minimum or maximum value of the gene.
        /// </remarks>
        public void Mutate()
        {
            var range = MaxValue - MinValue;
            var stddev = range * SimulationSettings.MutationStdDev;
            var diff = (float)Normal.Sample(_randGen, 0, stddev);
            Value += diff;
            if (Value > MaxValue) Value = MaxValue;
            if (Value < MinValue) Value = MinValue;
        }

        /// <summary>
        /// Clones the gene.
        /// </summary>
        /// <returns>
        /// The cloned gene.
        /// </returns>
        public SGene Clone()
        {
            return new SGene(MinValue, MaxValue, Value);
        }
    }
}
