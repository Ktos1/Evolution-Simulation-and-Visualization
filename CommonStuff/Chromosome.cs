using ProjectEvolution.Simulation;
using ProjectEvolution.Visualization.LogicScripts;
using System;
using System.Collections.Generic;

namespace ProjectEvolution.CommonStuff
{
    /// <summary>
    /// Represents a chromosome.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the gene.
    /// </typeparam>
    public abstract class Chromosome<T> where T: Gene
    {
        /// <summary>
        /// The random number generator shared by all instances of the 
        /// <see cref="Chromosome{T}"/> class.
        /// </summary>
        protected static Random _randGen = new Random();
        /// <summary>
        /// The array of genes.
        /// </summary>
        protected T[] _genes;

        /// <summary>
        /// Gets the array of genes.
        /// </summary>
        public T[] Genes => _genes;

        /// <summary>
        /// Gets the turning frequency gene.
        /// </summary>
        public T TurningFrequencyGene
        {
            get => _genes[0];
            protected set => _genes[0] = value;
        }

        /// <summary>
        /// Gets the turning angle gene.
        /// </summary>
        public T TurningAngleGene
        {
            get => _genes[1];
            protected set => _genes[1] = value;
        }

        /// <summary>
        /// Gets the energy amount to start partner search gene.
        /// </summary>
        public T EnrgAmntToStrtPrtnrSrchGene
        {
            get => _genes[2];
            protected set => _genes[2] = value;
        }

        /// <summary>
        /// Gets the energy amount to start food search gene.
        /// </summary>
        public T EnrgAmntToStrtFdSrchGene
        {
            get => _genes[3];
            protected set => _genes[3] = value;
        }

        /// <summary>
        /// Gets the sight range gene.
        /// </summary>
        public T SightRangeGene
        {
            get => _genes[4];
            protected set => _genes[4] = value;
        }

        /// <summary>
        /// Gets the speed gene.
        /// </summary>
        public T SpeedGene
        {
            get => _genes[5];
            protected set => _genes[5] = value;
        }

        /// <summary>
        /// Gets the maximum energy amount gene.
        /// </summary>
        public T MaxEnergyAmountGene
        {
            get => _genes[6];
            protected set => _genes[6] = value;
        }

        /// <summary>
        /// Gets the additional energy for child gene.
        /// </summary>
        public T AdditEnrgyForChldGene
        {
            get => _genes[7];
            protected set => _genes[7] = value;
        }

        /// <summary>
        /// Gets the life duration gene.
        /// </summary>
        public T LifeDurationGene
        {
            get => _genes[8];
            protected set => _genes[8] = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chromosome{T}"/> class.
        /// </summary>
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

        /// <summary>
        /// Sets the hardcoded limitations for the genes values.
        /// </summary>
        private void SetGenesLimitations()
        {
            TurningFrequencyGene.SetLimitations(0, 100);
            TurningAngleGene.SetLimitations(0, 180);
            EnrgAmntToStrtPrtnrSrchGene.SetLimitations(0, 100);
            EnrgAmntToStrtFdSrchGene.SetLimitations(0, 100);
            SightRangeGene.SetLimitations(0, 10);
            SpeedGene.SetLimitations(0, 6);
            MaxEnergyAmountGene.SetLimitations(SimulationSettings.ReproductionCost, 300);
            AdditEnrgyForChldGene.SetLimitations(0, 250);
            LifeDurationGene.SetLimitations(200, 2000);
        }

        /// <summary>
        /// Uses reflection to get the names of the properties representing the genes.
        /// </summary>
        /// <returns>
        /// The genes properties names.
        /// </returns>
        public static string[] GetGenesNames()
        {
            var propertiesInfo = typeof(Chromosome<SGene>).GetProperties();
            var genesNames = new List<string>();
            for (int i = 0; i < propertiesInfo.Length; i++)
            {
                if (propertiesInfo[i].PropertyType == typeof(SGene))
                    genesNames.Add(propertiesInfo[i].Name);
            }
            return genesNames.ToArray();
        }
    }
}