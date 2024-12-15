using MathNet.Numerics.Distributions;
using ProjectEvolution.CommonStuff;
using System;

namespace ProjectEvolution.Simulation.Algorithm
{
    public class SGene : Gene
    {
        public SGene() { }

        private SGene(float minValue, float maxValue, float value)
            : base(minValue, maxValue, value) { }

        public void Mutate()
        {
            var range = MaxValue - MinValue;
            var stddev = range * SimulationSettings.MutationStdDev * 0.01;
            var diff = (float)Normal.Sample(_randGen, 0, stddev);
            Value += diff;
            if (Value > MaxValue) Value = MaxValue;
            if (Value < MinValue) Value = MinValue;
        }

        public SGene Clone()
        {
            return new SGene(MinValue, MaxValue, Value);
        }
    }
}
