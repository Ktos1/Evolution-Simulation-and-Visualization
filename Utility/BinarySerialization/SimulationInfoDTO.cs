
using MessagePack;
using System.Numerics;

namespace ProjectEvolution.Utility.BinarySerialization
{
    [MessagePackObject]
    public class SimulationInfoDTO
    {
        [Key(0)]
        public (int x, int y) MapSize { get; private set; }

        public SimulationInfoDTO((int, int) mapSize)
        {
            MapSize = mapSize;
        }
    }
}