using Godot;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Visualization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ProjectEvolution.Utility
{
    internal static class JsonReader
    {
        private static int _currentTickNumber = -1;
        private static (Vector2 position, CreatureStates state, VChromosome chromosome)[][] _ticks;

        public static int TotalTicksNumber { get; private set; }
        public static int CurrentTickNumber
        {
            get { return  _currentTickNumber; }
            set { _currentTickNumber = (value < 0) ? -1 : value - 1; }
        }

        static JsonReader()
        {
            var jsonString = File.ReadAllText("result.json");
            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                JsonElement root = document.RootElement;
                var rawTicks = root.GetProperty("ticks").EnumerateArray();
                TotalTicksNumber = rawTicks.Count();

                _ticks = new(Vector2 position, CreatureStates state, VChromosome chromosome)[TotalTicksNumber][];
                int i = 0, j = 0;
                foreach (var tick in rawTicks)
                {
                    var tickArray = tick.EnumerateArray();
                    var creaturesTickData = new (Vector2 position, CreatureStates state, VChromosome chromosome)[tickArray.Count()];

                    j = 0;
                    foreach (var creature in tickArray)
                    {
                        var x = creature.GetProperty("X").GetSingle();
                        var y = creature.GetProperty("Y").GetSingle();
                        var position = new Vector2(x, y);

                        var state = (CreatureStates)creature.GetProperty("S").GetInt32();

                        JsonElement genesJSON;
                        float[] genes = null;
                        if (creature.TryGetProperty("C", out genesJSON))
                        {
                            List<float> genesList = new List<float>();
                            foreach (var gene in genesJSON.EnumerateArray())
                            {
                                genesList.Add(gene.GetSingle());
                            }
                            genes = genesList.ToArray();

                            creaturesTickData[j] = new(position, state, new VChromosome(genes));
                        }
                        else
                        {
                            creaturesTickData[j] = new(position, state, null);
                        }
                        j++;
                    }
                    _ticks[i] = creaturesTickData;
                    i++;
                }
            }
        }

        public static (Vector2 position, CreatureStates state, VChromosome chromosome)[] NextTick()
        {
            _currentTickNumber++;
            if (_currentTickNumber >= _ticks.Length) return null;
            return _ticks[CurrentTickNumber];
        }
    }
}
