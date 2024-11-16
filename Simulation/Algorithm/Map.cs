namespace ProjectEvolution.Simulation.Algorithm
{
    public class Map
    {
        // in the future type of Tiles should be a 2D-array of the custom type Tile
        public readonly int[,] Tiles;

        public (int x, int y) Size { get; private set; }

        public Map(int xSize, int ySize)
        {
            Tiles = new int[xSize, ySize];
            Size = new(xSize, ySize);
        }
    }
}
