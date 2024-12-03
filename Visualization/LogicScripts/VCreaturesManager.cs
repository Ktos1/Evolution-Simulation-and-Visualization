using ProjectEvolution.CommonStuff;
using ProjectEvolution.Utility.BinarySerialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectEvolution.Visualization.LogicScripts
{
    internal class VCreaturesManager
    {
        private Visualization _visualization;

        private List<VCreature> _creatures = new List<VCreature>();
        private List<VCreature> _deadCreatures = new List<VCreature>();

        private float[] _averageStartGenesValues;

        public float[] AverageStartGenesValues => _averageStartGenesValues;

        public VCreaturesManager(Visualization visualization)
        {
            _visualization = visualization;
            InitializeCreatures();
        }

        public void UpdateCreatures()
        {
            var creaturesData = BinReader.GetCreaturesTickData();
            for (int i = 0; i < creaturesData.Length; i++)
            {
                if (i < _creatures.Count)
                {
                    _creatures[i].Update(creaturesData[i]);
                }
                else
                {
                    AddNewCreature(creaturesData[i]);
                }
            }
            DeleteDeadCreatures();
        }

        /// <summary>
        /// Processes creatures without loading new tick data. It uses the linear interpolation to make creature
        /// movement more smooth.
        /// </summary>
        /// <param name="timeCounterBetweenTicks">
        /// Current time counted from last tick data loading. It is used to the interpolation.
        /// </param>
        public void ProcessCreatures(double timeCounterBetweenTicks)
        {
            _creatures.ForEach((creature) => creature.Process((float)timeCounterBetweenTicks));
        }

        public void UncheckAllCreatures()
        {
            _creatures.ForEach((creature) => creature.Uncheck());
        }

        public void LoadOnTick(int tickNumber)
        {
            int tickIndex = tickNumber - 1;
            BinReader.CurrentTickNumber = tickIndex;
            var creaturesData = BinReader.GetCreaturesTickData();
            var idsWithoutChromosome = new List<uint>();
            foreach (var creatureData in creaturesData)
            {
                if (creatureData.Chromosome is null)
                {
                    idsWithoutChromosome.Add(creatureData.Id);
                }
            }
            tickIndex--;

            int missingChromosomeCounter = idsWithoutChromosome.Count;
            while (missingChromosomeCounter != 0)
            {
                BinReader.CurrentTickNumber = tickIndex;
                var tickCreaturesData = BinReader.GetCreaturesTickData();
                for (int i = tickCreaturesData.Length - 1; i >= 0; i--)
                {
                    var (id, _, _, chromosome) = tickCreaturesData[i];
                    if (chromosome is not null)
                    {
                        for (int j = 0; j < idsWithoutChromosome.Count; j++)
                        {
                            if (idsWithoutChromosome[j] == id)
                            {
                                creaturesData[j] = creaturesData[j] with { Chromosome = chromosome };
                                missingChromosomeCounter--;
                            }
                        }
                    }
                    else
                        break;
                }
                tickIndex--;
            }
            AddNewCreatures(creaturesData);
            BinReader.CurrentTickNumber = tickNumber;
            UpdateCreatures();
            ProcessCreatures(CommonSettings.TICK_DURATION);
        }

        public void Clear()
        {
            foreach (var creature in _creatures)
            {
                creature.Delete();
            }
            _creatures.Clear();
            _deadCreatures.Clear();
        }

        private void DeleteDeadCreatures()
        {
            _deadCreatures.ForEach((deadCreature) =>
            {
                _creatures.Remove(deadCreature);
            });
            _deadCreatures.Clear();
        }

        private void InitializeCreatures()
        {
            var creaturesData = BinReader.GetCreaturesTickData();
            _averageStartGenesValues = CalculateAverageGenesValues(
                creaturesData.Select((x) => x.Chromosome).ToArray()
                );
            AddNewCreatures(creaturesData);
        }

        private void AddNewCreatures(CreatureTickData[] creatures)
        {
            foreach (var creatureData in creatures)
            {
                AddNewCreature(creatureData);
            }
        }

        private void AddNewCreature(CreatureTickData creatureData)
        {
            var creature = new VCreature(creatureData, this);
            creature.ClickedOn += _visualization._genesWindow.OnClickedOnCreature;
            creature.Deleted += OnCreatureDeletion;
            _creatures.Add(creature);
            _visualization.CallDeferred("add_child", _creatures.Last().StaticBody);
        }

        private float[] CalculateAverageGenesValues(VChromosome[] chromosomes)
        {
            float sum = 0;
            int genesNumber = chromosomes[0].Genes.Length;
            var result = new float[genesNumber];

            for (int i = 0; i < genesNumber; i++)
            {
                for (int j = 0; j < chromosomes.Length; j++)
                {
                    sum += chromosomes[j].Genes[i].Value;
                }
                result[i] = sum / chromosomes.Length;
                sum = 0;
            }
            return result;
        }

        private void OnCreatureDeletion(object creature, EventArgs e)
        {
            _deadCreatures.Add(creature as VCreature);
        }
    }
}
