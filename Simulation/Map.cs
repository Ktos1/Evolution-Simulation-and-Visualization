namespace ProjectEvolution.Simulation
{
    /// <summary>
    /// Represents the map in the simulation part.
    /// </summary>
    public class Map
    {
        /// <summary>
        /// Gets and privately sets the size of the map.
        /// </summary>
        public (int x, int y) Size { get; private set; }
        /// <summary>
        /// Gets and privately sets the limitations of the map.
        /// </summary>
        /// <remarks>
        /// It is set assuming that the center of the map is the origin. The x and y values
        /// represent the maximum distance from the origin in the x and y axis, respectively.
        /// </remarks>
        public (float x, float y) Limitations { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Map"/> class.
        /// </summary>
        /// <param name="xSize">
        /// The first size of the map.
        /// </param>
        /// <param name="ySize">
        /// The second size of the map.
        /// </param>
        public Map(int xSize, int ySize)
        {
            Size = new(xSize, ySize);
            Limitations = new(xSize / 2f, ySize / 2f);
        }
    }
}
