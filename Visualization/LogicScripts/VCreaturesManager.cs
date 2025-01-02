using ProjectEvolution.CommonStuff;
using ProjectEvolution.Utility.BinarySerialization;
using ProjectEvolution.Visualization.ScenesScripts;
using System.Collections.Generic;
using System.Linq;

namespace ProjectEvolution.Visualization.LogicScripts
{
    /// <summary>
    /// Manages the creatures in the visualization part.
    /// </summary>
    public class VCreaturesManager
    {
        /// <summary>
        /// The main node of the visualization scene.
        /// </summary>
        private VisualizationScene _visualization;
        /// <summary>
        /// The list of creatures contained by this manager.
        /// </summary>
        private List<VCreature> _creatures = new List<VCreature>();
        /// <summary>
        /// The list of creatures that will be deleted from the main 
        /// <see cref="_creatures"/> list at the end of a creatures update.
        /// </summary>
        private List<VCreature> _deadCreatures = new List<VCreature>();
        /// <summary>
        /// The average values of the genes of the creatures at the start of the simulation.
        /// </summary>
        private float[] _averageStartGenesValues;

        /// <summary>
        /// Gets the average values of the genes of the creatures at the start of the 
        /// simulation.
        /// </summary>
        public float[] AverageStartGenesValues => _averageStartGenesValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="VCreaturesManager"/> class.
        /// </summary>
        /// <param name="visualization">
        /// The visualization node.
        /// </param>
        public VCreaturesManager(VisualizationScene visualization)
        {
            _visualization = visualization;
            AddInitalCreatures();
        }

        /// <summary>
        /// Updates the creatures.
        /// </summary>
        /// <remarks>
        /// This method updates the creatures by loading the new creatures tick data from 
        /// the binary reader, updating with this data the creatures that are already
        /// in the list, and adding or deleting creatures if necessary.
        /// </remarks>
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
        /// Processes creatures without loading a new tick data. It uses the linear 
        /// interpolation to make creature movement more smooth.
        /// </summary>
        /// <param name="timeCounterBetweenTicks">
        /// Current time counted from last tick data loading. It is used to the interpolation.
        /// </param>
        public void ProcessCreatures(double timeCounterBetweenTicks)
        {
            _creatures.ForEach((creature) => creature.Process((float)timeCounterBetweenTicks));
        }

        /// <summary>
        /// Unchecks all the creatures.
        /// </summary>
        public void UncheckAllCreatures()
        {
            _creatures.ForEach((creature) => creature.IsChecked = false);
        }

        /// <summary>
        /// Loads a creatures on a specific tick.
        /// </summary>
        /// <param name="tickNumber">
        /// The tick number to load the creatures on.
        /// </param>
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
                                idsWithoutChromosome.RemoveAt(j);
                                missingChromosomeCounter--;
                                break;
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

        /// <summary>
        /// Clears all the lists of creatures.
        /// </summary>
        public void Clear()
        {
            foreach (var creature in _creatures)
            {
                creature.Delete();
            }
            _creatures.Clear();
            _deadCreatures.Clear();
        }

        /// <summary>
        /// Deletes the creatures that are in the <see cref="_deadCreatures"/> list 
        /// from the <see cref="_creatures"/> list.
        /// </summary>
        private void DeleteDeadCreatures()
        {
            _deadCreatures.ForEach((deadCreature) =>
            {
                _creatures.Remove(deadCreature);
            });
            _deadCreatures.Clear();
        }

        /// <summary>
        /// Adds the initial creatures.
        /// </summary>
        /// <remarks>
        /// This method adds the initial creatures by loading the creatures data from the 
        /// currently setted tick in binary reader and sets the average start genes values.
        /// </remarks>
        private void AddInitalCreatures()
        {
            var creaturesData = BinReader.GetCreaturesTickData();
            _averageStartGenesValues = CalculateAverageGenesValues(
                creaturesData.Select((x) => x.Chromosome).ToArray()
                );
            AddNewCreatures(creaturesData);
        }

        /// <summary>
        /// Adds a range of new creatures.
        /// </summary>
        /// <remarks>
        /// This method adds a range of new creatures by initializing a new 
        /// <see cref="VCreature"/> for each given creature data and adding it to the 
        /// <see cref="_creatures"/> list.
        /// </remarks>
        /// <param name="creatures">
        /// The tick data of the creatures to add.
        /// </param>
        private void AddNewCreatures(CreatureTickData[] creatures)
        {
            foreach (var creatureData in creatures)
            {
                AddNewCreature(creatureData);
            }
        }

        /// <summary>
        /// Adds a new creature.
        /// </summary>
        /// <remarks>
        /// This method adds a new creature by initializing a new <see cref="VCreature"/> 
        /// with the given creature data and adding it to the <see cref="_creatures"/> list.
        /// </remarks>
        /// <param name="creatureData">
        /// The tick data of the creature to add.
        /// </param>
        private void AddNewCreature(CreatureTickData creatureData)
        {
            var creature = new VCreature(creatureData);
            creature.ClickedOn += _visualization.GenesWindow.OnClickOnCreature;
            creature.Deleted += OnCreatureDeletion;
            _creatures.Add(creature);
            _visualization.CallDeferred("add_child", creature.StaticBody);
        }

        /// <summary>
        /// Calculates the average genes values of the given chromosomes.
        /// </summary>
        /// <param name="chromosomes">
        /// The chromosomes to calculate the average genes values.
        /// </param>
        /// <returns>
        /// The average genes values.
        /// </returns>
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

        /// <summary>
        /// Handles the creature deletion event.
        /// </summary>
        /// <param name="creature">
        /// The creature that is being deleted.
        /// </param>
        private void OnCreatureDeletion(VCreature creature)
        {
            _deadCreatures.Add(creature);
        }
    }
}
