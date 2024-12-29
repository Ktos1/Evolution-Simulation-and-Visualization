namespace ProjectEvolution.Simulation
{
    public partial class SCreature
    {
        /// <summary>
        /// Represents the died state of a creature.
        /// </summary>
        private class DiedState : DurationState
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DiedState"/> class.
            /// </summary>
            /// <param name="sCreature">
            /// <inheritdoc/>
            /// </param>
            public DiedState(SCreature sCreature) : base(sCreature)
            {
                _stateDuration = SimulationSettings.DiedStateDuration;
            }

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            public override void Process()
            {
                if (_stateDuration == 1)
                {
                    _creature._newState = new ToDeleteState(_creature);
                }
                else
                {
                    _stateDuration--;
                }
            }
        }
    }
}
