using MessagePack;

namespace ProjectEvolution.Utility.BinarySerialization
{
    [MessagePackObject]
    public class PlantDTO
    {
        [Key(0)]
        public uint ID { get; private set; }
        [Key(1)]
        public (float x, float y)? Position { get; private set; }
        [Key(2)]
        public int PartsNumber { get; private set; }

        public PlantDTO(uint id, (float x, float y)? position, int partsNumber)
        {
            ID = id;
            Position = position;
            PartsNumber = partsNumber;
        }
    }
}