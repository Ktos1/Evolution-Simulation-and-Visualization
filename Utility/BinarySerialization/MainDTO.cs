using MessagePack;

namespace ProjectEvolution.Utility.BinarySerialization
{
    /// <summary>
    /// Represents a root of the save file.
    /// </summary>
    /// <remarks>
    /// To save data to a file an object of this class is created and serialized to the file.
    /// Every saving data is stored directly or not in the properties of this class.
    /// </remarks>
    [MessagePackObject]
    public class MainDTO
    {
        /// <summary>
        /// The simulation info data transfer object.
        /// </summary>
        [Key(0)]
        public SimulationInfoDTO SimulationInfo { get; private set; }
        /// <summary>
        /// The array of the ticks data transfer objects.
        /// </summary>
        [Key(1)]
        public TickDTO[] TicksData { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainDTO"/> class.
        /// </summary>
        /// <param name="simulationInfo">
        /// The simulation info data transfer object.
        /// </param>
        /// <param name="ticksData">
        /// The array of the ticks data transfer objects.
        /// </param>
        public MainDTO(SimulationInfoDTO simulationInfo, TickDTO[] ticksData)
        {
            SimulationInfo = simulationInfo;
            TicksData = ticksData;
        }
    }
}
