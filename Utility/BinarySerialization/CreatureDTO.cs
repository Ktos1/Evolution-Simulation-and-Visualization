using MessagePack;
using ProjectEvolution.CommonStuff;

namespace ProjectEvolution.Utility.BinarySerialization
{
    [MessagePackObject]
    public class CreatureDTO
    {
        [Key(0)]
        public (float x, float y) Position { get; private set; }
        [Key(1)]
        public CreatureStates CurrentState { get; private set; }
        [Key(2)]
        public float[] Genes { get; private set; }

        public CreatureDTO((float x, float y) position, CreatureStates currentState, float[] genes)
        {
            Position = position;
            CurrentState = currentState;
            Genes = genes;
        }
    }
}