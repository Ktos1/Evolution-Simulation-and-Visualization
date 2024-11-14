using MessagePack;
using System.IO;

namespace ProjectEvolution.Utility.BinarySerialization
{
    internal class BinWriter
    {
        private SimulationInfoDTO _simulationInfo;
        private TickDTO[] _ticksData;

        private int _index = 0;

        public BinWriter(int ticksNumber, SimulationInfoDTO simulationInfo)
        {
            _simulationInfo = simulationInfo;
            _ticksData = new TickDTO[ticksNumber];
        }

        public void AddTick(TickDTO tick)
        {
            _ticksData[_index++] = tick;
        }

        public void SaveToFile()
        {
            var main = new MainDTO(_simulationInfo, _ticksData);
            byte[] binaryResults = MessagePackSerializer.Serialize(main);
            File.WriteAllBytes("result.bin", binaryResults);
        }
    }
}
