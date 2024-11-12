using ProjectEvolution.CommonStuff;

namespace ProjectEvolution.Visualization
{
    internal class VGene : Gene
    {
        public VGene() { }

        public VGene(float value) : base(value) { }

        public VGene(float minValue, float maxValue, float value)
            : base(minValue, maxValue, value) { }
    }
}
