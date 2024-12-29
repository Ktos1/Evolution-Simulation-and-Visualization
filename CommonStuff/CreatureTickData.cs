using Godot;
using ProjectEvolution.Visualization.LogicScripts;

namespace ProjectEvolution.CommonStuff
{
    /// <summary>
    /// Contains the data of a creature in a tick.
    /// </summary>
    /// <param name="Id">
    /// The id of the creature.
    /// </param>
    /// <param name="Position">
    /// The position of the creature.
    /// </param>
    /// <param name="State">
    /// The state of the creature.
    /// </param>
    /// <param name="Chromosome">
    /// The chromosome of the creature.
    /// </param>
    public record class CreatureTickData
        (uint Id, Vector2 Position, CreatureStates State, VChromosome Chromosome);
}
