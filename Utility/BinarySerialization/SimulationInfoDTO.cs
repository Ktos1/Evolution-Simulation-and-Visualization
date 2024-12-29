using MessagePack;

namespace ProjectEvolution.Utility.BinarySerialization
{
    /// <summary>
    /// Represents a data transfer object for the simulation info.
    /// </summary>
    /// <remarks>
    /// Contains a information about the simulation which are needed to save only once 
    /// at start in the save file.
    /// </remarks>
    [MessagePackObject]
    public class SimulationInfoDTO
    {
        /// <summary>
        /// The size of the map.
        /// </summary>
        [Key(0)]
        public (int x, int y) MapSize { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulationInfoDTO"/> class.
        /// </summary>
        /// <param name="mapSize">
        /// The size of the map.
        /// </param>
        public SimulationInfoDTO((int, int) mapSize)
        {
            MapSize = mapSize;
        }
    }
}