using System;

namespace ProjectEvolution.CommonStuff
{
    public abstract class Gene
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

        public static string TranslateNameToPolish(string englishName)
        {
            switch (englishName)
            {
                case "TurningFrequencyGene":
                    return "Gen częstotliwości obracania się";
                case "TurningAngleGene":
                    return "Gen kątu obracania się";
                case "EnrgAmntToStrtPrtnrSrchGene":
                    return "Gen energii do szukania partnera";
                case "EnrgAmntToStrtFdSrchGene":
                    return "Gen energii do szukania jedzenia";
                case "SightGene":
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
                    return "";
            }
        }
    }
}
