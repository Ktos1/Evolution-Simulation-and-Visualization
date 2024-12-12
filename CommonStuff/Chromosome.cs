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

        public T EnergyAmountToStartPartnerSearchGene
        {
            get => _genes[2];
            protected set => _genes[2] = value;
        }

        public T EnergyAmountToStartFoodSearchGene
        {
            get => _genes[3];
            protected set => _genes[3] = value;
        }

        public Chromosome()
        {
            _genes = new T[4];
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
            EnergyAmountToStartPartnerSearchGene.SetLimitations(0, 100);
            EnergyAmountToStartFoodSearchGene.SetLimitations(0, 100);
        }
    }
}