using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvolution.Utility
{
    internal static class Extensions
    {
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
