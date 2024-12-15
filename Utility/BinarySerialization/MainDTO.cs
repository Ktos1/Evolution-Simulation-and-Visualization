using MessagePack;

namespace ProjectEvolution.Utility.BinarySerialization
{
    [MessagePackObject]
    public class MainDTO
    {
        [Key(0)]
        public SimulationInfoDTO SimulationInfo { get; private set; }
        [Key(1)]
        public TickDTO[] TicksData { get; private set; }

        public MainDTO(SimulationInfoDTO simulationInfo, TickDTO[] ticksData)
        {
            SimulationInfo = simulationInfo;
            TicksData = ticksData;
        }
    }
}
