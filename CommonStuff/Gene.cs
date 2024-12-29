using System;

namespace ProjectEvolution.CommonStuff
{
    /// <summary>
    /// Represents a gene.
    /// </summary>
    public abstract class Gene
    {
        /// <summary>
        /// The random number generator shared by all instances of the 
        /// <see cref="Gene"/> class.
        /// </summary>
        protected static Random _randGen = new Random();

        /// <summary>
        /// Gets or sets the minimum value of the gene.
        /// </summary>
        public float MinValue { get; set; }
        /// <summary>
        /// Gets or sets the maximum value of the gene.
        /// </summary>
        public float MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the value of the gene.
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gene"/> class.
        /// </summary>
        public Gene() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gene"/> class.
        /// </summary>
        /// <param name="value">
        /// The value of the gene.
        /// </param>
        public Gene(float value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gene"/> class.
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
        public Gene(float minValue, float maxValue, float value) : this(value)
        {
            SetLimitations(minValue, maxValue);
        }

        /// <summary>
        /// Sets the limitations of the gene.
        /// </summary>
        /// <param name="minValue">
        /// The minimum value of the gene.
        /// </param>
        /// <param name="maxValue">
        /// The maximum value of the gene.
        /// </param>
        public void SetLimitations(float minValue, float maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        /// <summary>
        /// Translates a raw property name of a gene in english to a prettly formated
        /// polish name.
        /// </summary>
        /// <param name="englishName">
        /// The raw property name of the gene in english.
        /// </param>
        /// <returns>
        /// The prettly formated polish name of the gene.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the given english gene name is not recognized.
        /// </exception>
        public static string TranslateNameToPolish(string englishName)
        {
            switch (englishName)
            {
                case "TurningFrequencyGene":
                    return "Gen częstotliwości obracania się";
                case "TurningAngleGene":
                    return "Gen kąta obracania się";
                case "EnrgAmntToStrtPrtnrSrchGene":
                    return "Gen energii do szukania partnera";
                case "EnrgAmntToStrtFdSrchGene":
                    return "Gen energii do szukania jedzenia";
                case "SightRangeGene":
                    return "Gen zasięgu wzroku";
                case "SpeedGene":
                    return "Gen szybkości";
                case "MaxEnergyAmountGene":
                    return "Gen maksymalnej ilości energii";
                case "AdditEnrgyForChldGene":
                    return "Gen dodatkowej energii dla dziecka";
                case "LifeDurationGene":
                    return "Gen długości życia";
                default: 
                    throw new ArgumentException(
                        "The given english gene name is not recognized.");
            }
        }
    }
}
