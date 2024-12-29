using MessagePack;
using ProjectEvolution.CommonStuff;

namespace ProjectEvolution.Utility.BinarySerialization
{
    /// <summary>
    /// Represents a data transfer object for a creature data for an one tick.
    /// </summary>
    [MessagePackObject]
    public class CreatureDTO
    {
        /// <summary>
        /// The ID of the creature.
        /// </summary>
        [Key(0)]
        public uint ID { get; private set; }
        /// <summary>
        /// The position of the creature.
        /// </summary>
        [Key(1)]
        public (float x, float y) Position { get; private set; }
        /// <summary>
        /// The state of the creature.
        /// </summary>
        [Key(2)]
        public CreatureStates State { get; private set; }
        /// <summary>
        /// The genes of the creature.
        /// </summary>
        [Key(3)]
        public float[] Genes { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureDTO"/> class.
        /// </summary>
        /// <param name="id">
        /// The ID of the creature.
        /// </param>
        /// <param name="position">
        /// The position of the creature.
        /// </param>
        /// <param name="currentState">
        /// The current state of the creature.
        /// </param>
        /// <param name="genes">
        /// The genes of the creature.
        /// </param>
        public CreatureDTO(uint id, (float x, float y) position, CreatureStates currentState, float[] genes)
        {
            ID = id;
            Position = position;
            State = currentState;
            Genes = genes;
        }
    }
}