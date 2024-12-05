namespace ProjectEvolution.Simulation.Algorithm
{
    public class Map
    {
        public (int x, int y) Size { get; private set; }

        public Map(int xSize, int ySize)
        {
            Size = new(xSize, ySize);
        }
    }
}
