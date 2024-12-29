using ProjectEvolution.Simulation;

namespace ProjectEvolution.CommonStuff
{
    /// <summary>
    /// Represents the states of a creature.
    /// </summary>
    /// <remarks>
    /// The values symbolized the states from the <see cref="SCreature"/>.
    /// </remarks>
    public enum CreatureStates
    {
        ToDelete,
        SeekingForPartner,
        MovingToPartner,
        Reproducing,
        SeekingForFood,
        MovingToFood,
        Eating,
        Died
    }
}
