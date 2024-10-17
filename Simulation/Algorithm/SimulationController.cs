using ProjectEvolution.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProjectEvolution.Simulation.Algorithm
{
    public class SimulationController // eventually this class could be named the World
    {
        private const int YearDuration = 500; // in delta time
        public const float DELTA_TIME = 0.0333f; // 1/30 
        public readonly Map Map;
        private List<SCreature> _creatures = new List<SCreature>();

        List<SCreature> _deadCreatures = new List<SCreature>();
        List<SCreature> _bornCreatures = new List<SCreature>();

        public SimulationController(Map map)
        {
            Map = map;
            for (int i = 0; i < 30; i++)
            {
                _creatures.Add(new SCreature(this));
            }
            SavePopulationToJson();
        }

        public bool StartSimulation(int years)
        {
            //years* YearDuration *DELTA_TIME   ,   += DELTA_TIME
            for (int i = 0; i < 1000; i++)
            {
                foreach (var creature in _creatures)
                {
                    creature.Process();
                }
                _creatures.ForEach(creature => creature.Update());
                UpdateCreatureList();
                SavePopulationToJson();
            }

            File.WriteAllText("result.json", JsonWriter.jsonString);
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

        private void UpdateCreatureList()
        {
            _deadCreatures.ForEach(creature => _creatures.Remove(creature));
            _bornCreatures.ForEach(creature => _creatures.Add(creature));
            _deadCreatures.Clear();
            _bornCreatures.Clear();
        }

        private void SavePopulationToJson()
        {
            foreach (var creature in _creatures)
            {
                var (position, state, genes) = creature.GetSavingData();
                if (genes == null)
                {
                    JsonWriter.WriteCreature(position, state);
                }
                else
                {
                    JsonWriter.WriteNewCreature(position, state, genes);
                }
            }
            JsonWriter.NextTick();
        }
    }
}
