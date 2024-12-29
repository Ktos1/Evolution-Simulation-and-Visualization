namespace ProjectEvolution.Simulation
{
    public partial class SCreature
    {
        /// <summary>
        /// Represents state of a creature which has a specified duration.
        /// </summary>
        private abstract class DurationState : State
        {
            /// <summary>
            /// The duration of the state.
            /// </summary>
            protected int _stateDuration;

            /// <summary>
            /// Initializes a new instance of the <see cref="DurationState"/> class.
            /// </summary>
            /// <param name="sCreature">
            /// <inheritdoc/>
            /// </param>
            protected DurationState(SCreature sCreature) : base(sCreature) { }
        }
    }
}
