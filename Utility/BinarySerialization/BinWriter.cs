using MessagePack;
using ProjectEvolution.CommonStuff;
using System.IO;
using System.Text.Json;
using System.Threading;

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

        public void SaveToFile(CancellationToken cancelToken)
        {
            var main = new MainDTO(_simulationInfo, _ticksData);
            byte[] binaryResults = MessagePackSerializer.Serialize(main, cancellationToken: cancelToken);

            if (CommonSettings.GenerateJSON)
            {
                var json = MessagePackSerializer.ConvertToJson(binaryResults);
                var jsonDocument = JsonDocument.Parse(json);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var prettyJson = JsonSerializer.Serialize(jsonDocument.RootElement, options);
                File.WriteAllText("binaryResultJSON.json", prettyJson);
            }
            
            File.WriteAllBytes("result.bin", binaryResults);
            PlotsCreater.CreatePlots(_ticksData);
        }
    }
}
