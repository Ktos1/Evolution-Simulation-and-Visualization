using System;

namespace ProjectEvolution.CommonStuff
{
    internal abstract class Gene
    {
        protected static Random _randGen = new Random();

        public float MinValue;
        public float MaxValue;

        public float Value { get; set; }

        public Gene() { }

        public Gene(float value)
        {
            Value = value;
        }

        public Gene(float minValue, float maxValue, float value)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Value = value;
        }

        public void SetLimitations(float minValue, float maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
}
