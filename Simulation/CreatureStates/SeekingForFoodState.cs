namespace ProjectEvolution.Simulation
{
    public partial class SCreature
    {
        /// <summary>
        /// Represents the seeking for food state of a creature.
        /// </summary>
        private class SeekingForFoodState : State
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SeekingForFoodState"/> class.
            /// </summary>
            /// <param name="creature">
            /// <inheritdoc/>
            /// </param>
            public SeekingForFoodState(SCreature creature) : base(creature)
            {
                _creature._newFocusObject = null;
            }

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            public override void Process()
            {
                SPlant[] plantsInRange = _creature._controller.ObjectsInRange<SPlant>
                    (_creature, _creature._sightRange);

                if (plantsInRange.Length > 0)
                {
                    var nearestPlant = plantsInRange[0];
                    _creature._newFocusObject = nearestPlant;
                    _creature._newState = new MovingToFoodState(_creature);
                }
                else
                {
                    _creature.RandomMove();
                }
            }
        }
    }
}
