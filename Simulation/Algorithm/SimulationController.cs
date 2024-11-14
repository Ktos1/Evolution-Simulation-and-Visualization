using ProjectEvolution.Utility.BinarySerialization;
using System.Collections.Generic;
using System.Linq;

namespace ProjectEvolution.Simulation.Algorithm
{
    public class SimulationController // eventually this class could be named the World
    {
        private const int YEAR_DURATION = 500; // in ticks
        public readonly Map Map;
        private List<SCreature> _creatures = new List<SCreature>();

        List<SCreature> _deadCreatures = new List<SCreature>();
        List<SCreature> _bornCreatures = new List<SCreature>();

        private BinWriter _binWriter;

        public SimulationController(Map map, int creaturesNum)
        {
            Map = map;
            for (int i = 0; i < creaturesNum; i++)
            {
                _creatures.Add(new SCreature(this));
            }
        }

        public bool StartSimulation(int years)
        {
            int totalTicksNumber = years * YEAR_DURATION;
            _binWriter = new BinWriter(totalTicksNumber + 1, new SimulationInfoDTO(Map.Size));
            SaveTickData();
            for (int i = 0; i < totalTicksNumber; i++)
            {
                foreach (var creature in _creatures)
                {
                    creature.Process();
                }
                _creatures.ForEach(creature => creature.Update());
                UpdateCreaturesList();
                SaveTickData();
            }
            _binWriter.SaveToFile();
            BinReader.LoadNewFile();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">
        /// Type of the object sought.
        /// </typeparam>
        /// <param name="seeker"></param>
        /// <param name="range"></param>
        /// <returns>
        /// An array of T-type objects sorted ascending by the distance to the <paramref name="seeker"/>
        /// </returns>
        public T[] ObjectsInRange<T> (SCreature seeker, float range) where T : MapObject
        {
            var objectsInRange = new List<(float distance, T _object)>();

            float bestDistance = range + 1;

            if (typeof(T) == typeof(SCreature))
            {
                foreach (var creature in _creatures)
                {
                    var distance = (seeker.Position - creature.Position).Length();
                    if (distance < range && distance != 0)
                    {
                        objectsInRange.Add((distance, creature as T));
                    }
                }
            }
            var sortedResult = objectsInRange.OrderBy((pair) => pair.distance).ToList();
            var resultArray = new T[sortedResult.Count()];

            for (int i = 0; i < resultArray.Length; i++)
            {
                resultArray[i] = sortedResult[i]._object;
            }

            return resultArray;
        }

        public void OnCreatureDeath(SCreature diedCreature)
        {
            _deadCreatures.Add(diedCreature);
        }

        public void OnCreatureBirth(SCreature bornCreature)
        {
            _bornCreatures.Add(bornCreature);
        }

        private void UpdateCreaturesList()
        {
            _deadCreatures.ForEach(creature => _creatures.Remove(creature));
            _bornCreatures.ForEach(creature => _creatures.Add(creature));
            _deadCreatures.Clear();
            _bornCreatures.Clear();
        }

        private void SaveTickData()
        {
            CreatureDTO[] creaturesDTOs = new CreatureDTO[_creatures.Count];
            for (int i = 0; i < _creatures.Count; i++)
            {
                SCreature creature = _creatures[i];
                var (position, state, genes) = creature.GetSavingData();
                creaturesDTOs[i] = new CreatureDTO(position, state, genes);

                // here probably should be a plant data saving to a DTO
            }
            _binWriter.AddTick(new TickDTO(new PlantDTO[0], creaturesDTOs));
        }
    }
}
