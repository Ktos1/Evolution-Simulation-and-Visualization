using MessagePack;

namespace ProjectEvolution.Utility.BinarySerialization
{
    [MessagePackObject]
    public class TickDTO
    {
        [Key(0)]
        public PlantDTO[] PlantsData { get; private set; }
        [Key(1)]
        public CreatureDTO[] CreaturesData { get; private set; }

        public TickDTO(PlantDTO[] plantsData, CreatureDTO[] creatureData)
        {
            PlantsData = plantsData;
            CreaturesData = creatureData;
        }
    }
}