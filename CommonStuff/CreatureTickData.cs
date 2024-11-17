using Godot;
using ProjectEvolution.Visualization;

namespace ProjectEvolution.CommonStuff
{
    public record class CreatureTickData
        (uint Id, Vector2 Position, CreatureStates State, VChromosome Chromosome);
}
