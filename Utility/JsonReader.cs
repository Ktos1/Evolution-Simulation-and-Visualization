using Godot;
using ProjectEvolution.CommonStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ProjectEvolution.Utility
{
    internal static class JsonReader
    {
        private static IEnumerator _ticksEnumerator;

        static JsonReader()
        {
            var jsonString = File.ReadAllText("result.json");
            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                JsonElement root = document.RootElement;
                var ticks = root.GetProperty("ticks").EnumerateArray();
                var ticksLength = ticks.Count();

                var result = new(Vector2 position, CreatureStates state, Chromosome chromosome)[ticksLength][];
                int i = 0, j = 0;
                foreach (var tick in ticks)
                {
                    var tickArray = tick.EnumerateArray();
                    var creaturesTickData = new (Vector2 position, CreatureStates state, Chromosome chromosome)[tickArray.Count()];

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

                            creaturesTickData[j] = new(position, state, new Chromosome(genes));
                        }
                        else
                        {
                            creaturesTickData[j] = new(position, state, null);
                        }
                        j++;
                    }
                    result[i] = creaturesTickData;
                    i++;
                }
                _ticksEnumerator = result.GetEnumerator();
            }
        }

        public static (Vector2 position, CreatureStates state, Chromosome chromosome)[] NextTick()
        {
            _ticksEnumerator.MoveNext();
            return (ValueTuple<Vector2, CreatureStates, Chromosome>[])_ticksEnumerator.Current;
        }
    }
}
