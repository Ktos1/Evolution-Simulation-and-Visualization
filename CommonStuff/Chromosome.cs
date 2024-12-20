using ProjectEvolution.Simulation.Algorithm;
using ProjectEvolution.Visualization;
using System;

namespace ProjectEvolution.CommonStuff
{
    public abstract class Chromosome<T> where T: Gene
    {
        protected static Random _randGen = new Random();
        protected T[] _genes;

        public T[] Genes => _genes;

        public T TurningFrequencyGene
        {
            get => _genes[0];
            protected set => _genes[0] = value;
        }

        public T TurningAngleGene
        {
            get => _genes[1];
            protected set => _genes[1] = value;
        }

        public T EnrgAmntToStrtPrtnrSrchGene
        {
            get => _genes[2];
            protected set => _genes[2] = value;
        }

        public T EnrgAmntToStrtFdSrchGene
        {
            get => _genes[3];
            protected set => _genes[3] = value;
        }

        public T SightGene
        {
            get => _genes[4];
            protected set => _genes[4] = value;
        }

        public T SpeedGene
        {
            get => _genes[5];
            protected set => _genes[5] = value;
        }

        public T MaxEnergyAmountGene
        {
            get => _genes[6];
            protected set => _genes[6] = value;
        }

        public T AdditEnrgyForChldGene
        {
            get => _genes[7];
            protected set => _genes[7] = value;
        }

        public T LifeDurationGene
        {
            get => _genes[8];
            protected set => _genes[8] = value;
        }

        protected Chromosome()
        {
            _genes = new T[9];
            if (typeof(T) == typeof(SGene))
            {
                for (var i = 0; i < _genes.Length; i++)
                {
                    _genes[i] = new SGene() as T;
                }
            }
            else
            {
                for (var i = 0; i < _genes.Length; i++)
                {
                    _genes[i] = new VGene() as T;
                }
            }
            SetGenesLimitations();
        }

        private void SetGenesLimitations()
        {
            TurningFrequencyGene.SetLimitations(0, 100);
            TurningAngleGene.SetLimitations(0, 180);
            EnrgAmntToStrtPrtnrSrchGene.SetLimitations(0, 100);
            EnrgAmntToStrtFdSrchGene.SetLimitations(0, 100);
            SightGene.SetLimitations(0, 10);
            SpeedGene.SetLimitations(0, 6);
            MaxEnergyAmountGene.SetLimitations(SimulationSettings.ReproductionCost, 300);
            AdditEnrgyForChldGene.SetLimitations(0, 250);
            LifeDurationGene.SetLimitations(200, 2000);
        }
    }
}