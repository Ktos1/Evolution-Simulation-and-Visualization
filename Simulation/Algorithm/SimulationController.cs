using ProjectEvolution.Utility;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading;

namespace ProjectEvolution.Simulation.Algorithm
{
    public class SimulationController // eventually this class could be named the World
    {
        private const int YearDuration = 500; // in delta time
        public const float DELTA_TIME = 0.0333f; // 1/30 
        public readonly Map Map;
        private List<Creature> _creatures = new List<Creature>();

        List<Creature> _deadCreatures = new List<Creature>();
        List<Creature> _bornCreatures = new List<Creature>();

        public SimulationController(Map map)
        {
            Map = map;
            for (int i = 0; i < 15; i++)
            {
                _creatures.Add(new Creature(this));
            }
            SavePopulationToJson();
        }

        public bool StartSimulation(int years)
        {
            //years* YearDuration *DELTA_TIME   ,   += DELTA_TIME
            for (int i = 0; i < 600; i++)
            {
                foreach (var creature in _creatures)
                {
                    creature.Process();
                }
                UpdateCreatureList();
                SavePopulationToJson();
            }

            File.WriteAllText("result.json", JsonWriter.jsonString);
            return true;
        }

#nullable enable
        public T? IsItInRange<T> (Creature seeker, float range) where T : MapObject
        {
            T? bestObject = null;

            float bestDistance = range + 1;

            if (typeof(T) == typeof(Creature))
            {
                foreach (var creature in _creatures)
                {
                    var distance = (seeker.Position - creature.Position).Length();
                    if (distance < range && distance != 0)
                    {
                        if (distance < bestDistance)
                        {
                            bestObject = creature as T;
                            bestDistance = distance;
                        }
                    }
                }
            }
            return bestObject;
        }
#nullable disable
        public void OnCreatureDeath(Creature diedCreature)
        {
            _deadCreatures.Add(diedCreature);
        }

        public void OnCreatureBirth(Creature bornCreature)
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
