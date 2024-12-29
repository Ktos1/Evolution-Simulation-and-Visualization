using MessagePack;

namespace ProjectEvolution.Utility.BinarySerialization
{
    /// <summary>
    /// Represents a data transfer object for a plant data for an one tick.
    /// </summary>
    [MessagePackObject]
    public class PlantDTO
    {
        /// <summary>
        /// The ID of the plant.
        /// </summary>
        [Key(0)]
        public uint ID { get; private set; }
        /// <summary>
        /// The position of the plant.
        /// </summary>
        [Key(1)]
        public (float x, float y)? Position { get; private set; }
        /// <summary>
        /// The number of the parts of the plant.
        /// </summary>
        [Key(2)]
        public int PartsNumber { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlantDTO"/> class.
        /// </summary>
        /// <param name="id">
        /// The ID of the plant.
        /// </param>
        /// <param name="position">
        /// The position of the plant.
        /// </param>
        /// <param name="partsNumber">
        /// The number of the parts of the plant.
        /// </param>
        public PlantDTO(uint id, (float x, float y)? position, int partsNumber)
        {
            ID = id;
            Position = position;
            PartsNumber = partsNumber;
        }
    }
}