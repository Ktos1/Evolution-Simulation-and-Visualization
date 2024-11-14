using Godot;

namespace ProjectEvolution.Utility
{
    internal static class Extensions
    {
        // TODO: Delete this
        public static Vector2[] Lerp(this Vector2[] vectors, Vector2[] to, float weight)
        {
            Vector2[] result = new Vector2[vectors.Length];

            for (int i = 0; i < vectors.Length; i++)
            {
                result[i] = vectors[i].Lerp(to[i], weight);
            }
            return result;
        }
    }
}
