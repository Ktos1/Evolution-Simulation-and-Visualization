namespace ProjectEvolution.Simulation
{
    public partial class SCreature
    {
        /// <summary>
        /// Represents the to delete state of a creature.
        /// </summary>
        private class ToDeleteState : State
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ToDeleteState"/> class.
            /// </summary>
            /// <param name="sCreature">
            /// <inheritdoc/>
            /// </param>
            public ToDeleteState(SCreature sCreature) : base(sCreature) { }

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            public override void Process() { }
        }
    }
}
