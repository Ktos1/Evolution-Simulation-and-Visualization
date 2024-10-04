using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvolution.Simulation.Algorithm
{
    public class Map
    {
        // in the future type of Tiles should be a 2D-array of the custom type Tile
        public readonly int[,] Tiles;
        public Map(int xSize, int ySize)
        {
            Tiles = new int[xSize, ySize];
        }
    }
}
