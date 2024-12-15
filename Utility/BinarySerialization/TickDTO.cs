using MessagePack;

namespace ProjectEvolution.Utility.BinarySerialization
{
    [MessagePackObject]
    public class TickDTO
    {
        [Key(0)]
        public int PlantsNumber { get; private set; }
        [Key(1)]
        public PlantDTO[] PlantsData { get; private set; }
        [Key(2)]
        public CreatureDTO[] CreaturesData { get; private set; }

        public TickDTO(int plantsNumber, PlantDTO[] plantsData, CreatureDTO[] creatureData)
        {
            PlantsNumber = plantsNumber;
            PlantsData = plantsData;
            CreaturesData = creatureData;
        }
    }
}