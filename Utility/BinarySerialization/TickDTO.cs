using MessagePack;

namespace ProjectEvolution.Utility.BinarySerialization
{
    /// <summary>
    /// Represents a data transfer object for a tick data.
    /// </summary>
    [MessagePackObject]
    public class TickDTO
    {
        /// <summary>
        /// The array of the data transfer objects for all plants in the tick.
        /// </summary>
        private PlantDTO[] _plantsData;
        /// <summary>
        /// The array of the data transfer objects for all creatures in the tick.
        /// </summary>
        private CreatureDTO[] _creaturesData;

        /// <summary>
        /// The number of the plants in the tick.
        /// </summary>
        [Key(0)]
        public int PlantsNumber { get; private set; }
        /// <summary>
        /// Gets the array of the data transfer objects for all plants in the tick.
        /// </summary>
        [Key(1)]
        public PlantDTO[] PlantsData => _plantsData;
        /// <summary>
        /// Gets the array of the data transfer objects for all creatures in the tick.
        /// </summary>
        [Key(2)]
        public CreatureDTO[] CreaturesData => _creaturesData;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickDTO"/> class.
        /// </summary>
        /// <param name="plantsNumber">
        /// The number of the plants in the tick.
        /// </param>
        /// <param name="plantsData">
        /// The array of the data transfer objects for all creatures in the tick.
        /// </param>
        /// <param name="creatureData">
        /// The array of the data transfer objects for all creatures in the tick.
        /// </param>
        public TickDTO(int plantsNumber, PlantDTO[] plantsData, CreatureDTO[] creatureData)
        {
            PlantsNumber = plantsNumber;
            _plantsData = plantsData;
            _creaturesData = creatureData;
        }
    }
}