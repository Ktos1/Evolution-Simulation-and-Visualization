namespace ProjectEvolution.Simulation
{
    public partial class SCreature
    {
        /// <summary>
        /// Represents the moving to food state of a creature.
        /// </summary>
        private class MovingToFoodState : State
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MovingToFoodState"/> class.
            /// </summary>
            /// <param name="sCreature">
            /// <inheritdoc/>
            /// </param>
            public MovingToFoodState(SCreature sCreature) : base(sCreature) { }

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            public override void Process()
            {
                if (_creature.GetDistanceToFocusObject() < 0.5 ||
                    _creature.MoveToFocusedObject() < 0.5f)
                {
                    _creature._newState = new EatingState(_creature);
                }
            }
        }
    }
}
