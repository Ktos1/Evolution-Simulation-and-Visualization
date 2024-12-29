namespace ProjectEvolution.Simulation
{
    public partial class SCreature
    {
        /// <summary>
        /// Represents the reproducing state of a creature.
        /// </summary>
        private class ReproducingState : DurationState
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ReproducingState"/> class.
            /// </summary>
            /// <param name="sCreature">
            /// <inheritdoc/>
            /// </param>
            public ReproducingState(SCreature sCreature) 
                : base(sCreature)
            {
                _stateDuration = SimulationSettings.ReproductionDuration;
                _creature._isGivingBirth = false;
            }

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            public override void Process()
            {
                var partner = (SCreature)_creature._focusObject;
                if (partner._state is not ReproducingState)
                {
                    ChooseWhatToDo();
                }
                else if (_stateDuration == 0)
                {
                    if (!partner._isGivingBirth)
                    {
                        _creature._isGivingBirth = true;
                        _creature.GiveBirth(partner);
                    }
                    else
                    {
                        partner._isGivingBirth = false;
                    }
                    _creature._energy -= SimulationSettings.ReproductionCost;
                    ChooseWhatToDo();
                }
                else
                {
                    _stateDuration--;
                }
            }

            /// <summary>
            /// Determines the creature's behavior after reproduction or in situation when
            /// reproduction cannot be led to the end.
            /// </summary>
            private void ChooseWhatToDo()
            {
                if (_creature._energy < _creature.GetBorderForFoodSearch())
                    _creature._newState = new SeekingForFoodState(_creature);
                else
                    _creature._newState = new SeekingForPartnerState(_creature);
            }
        }
    }
}
