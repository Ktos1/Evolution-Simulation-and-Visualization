using MessagePack;
using ProjectEvolution.CommonStuff;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace ProjectEvolution.Utility.BinarySerialization
{
    /// <summary>
    /// Represents the binary writer used to save the simulation data to the file.
    /// </summary>
    internal class BinWriter
    {
        /// <summary>
        /// The simulation info data transfer object.
        /// </summary>
        private SimulationInfoDTO _simulationInfo;
        /// <summary>
        /// The array of the ticks data transfer objects.
        /// </summary>
        /// <remarks>
        /// The array is used to store the tick data transfer objects before saving 
        /// them to the file.
        /// </remarks>
        private TickDTO[] _ticksData;

        /// <summary>
        /// The current index of the tick data array.
        /// </summary>
        private int _index = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinWriter"/> class.
        /// </summary>
        /// <param name="ticksNumber">
        /// The total number of the ticks.
        /// </param>
        /// <param name="simulationInfo">
        /// The simulation info data transfer object.
        /// </param>
        public BinWriter(int ticksNumber, SimulationInfoDTO simulationInfo)
        {
            _simulationInfo = simulationInfo;
            _ticksData = new TickDTO[ticksNumber];
        }

        /// <summary>
        /// Adds the tick data transfer object to save to the file.
        /// </summary>
        /// <param name="tick">
        /// The tick data transfer object to add.
        /// </param>
        public void AddTick(TickDTO tick)
        {
            _ticksData[_index++] = tick;
        }

        /// <summary>
        /// Saves the simulation data to the file.
        /// </summary>
        /// <param name="cancelToken">
        /// The token used to safely abort the simulation which is being processed 
        /// on separate thread.
        /// </param>
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
        }
    }
}
