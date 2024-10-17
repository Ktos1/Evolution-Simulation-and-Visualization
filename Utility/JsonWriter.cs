using System.Linq;
using Godot;
using System.Text.Json;
using System.Text.Json.Nodes;
using ProjectEvolution.CommonStuff;

namespace ProjectEvolution.Utility
{
    internal static class JsonWriter
    {
        private static JsonObject _jsonData;
        private static JsonArray _ticksArray;
        private static JsonArray _tickArray;

        //private static JsonSerializerOptions serializerOptions = new JsonSerializerOptions { WriteIndented = true };

        // TODO: in the future change this ToString() to the Serialize() on a JsonSerializer class
        // in order to make a json file without white characters. You can use the serializerOptions above.
        public static string jsonString => _jsonData.ToString();

        static JsonWriter()
        {
            _jsonData = new JsonObject
            {
                ["ticks"] = new JsonArray()
            };
            _ticksArray = _jsonData!["ticks"].AsArray();
            NextTick();
        }

        public static void NextTick()
        {
            _ticksArray.Add(new JsonArray());
            _tickArray = (JsonArray)_ticksArray.Last();
        }

        public static void WriteCreature(Vector2 position, CreatureStates currentState)
        {
            _tickArray.Add(GetCreatureJSONObject(position, currentState));
        }

        public static void WriteNewCreature (Vector2 position, CreatureStates currentState, float[] genes)
        {
            JsonArray genesJson = new JsonArray();
            foreach (var gene in genes)
            {
                genesJson.Add(gene);
            }
            var creatureJson = GetCreatureJSONObject(position, currentState);
            creatureJson["C"] = genesJson;

            _tickArray.Add(creatureJson);
        }

        private static JsonObject GetCreatureJSONObject(Vector2 position, CreatureStates currentState)
        {
            JsonObject creatureJson = new JsonObject
            {
                ["X"] = position.X,
                ["Y"] = position.Y,
                ["S"] = (int)currentState
            };
            return creatureJson;
        }
    }
}
