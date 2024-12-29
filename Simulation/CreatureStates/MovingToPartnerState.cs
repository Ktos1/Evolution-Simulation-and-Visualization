namespace ProjectEvolution.Simulation
{
    public partial class SCreature
    {
        /// <summary>
        /// Represents the moving to partner state of a creature.
        /// </summary>
        private class MovingToPartnerState : State
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MovingToPartnerState"/> class.
            /// </summary>
            /// <param name="sCreature">
            /// <inheritdoc/>
            /// </param>
            public MovingToPartnerState(SCreature sCreature) : base(sCreature) { }

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            public override void Process()
            {
                var partner = (SCreature)_creature._focusObject;
                if (partner._state is SeekingForPartnerState ||
                    partner._focusObject == _creature &&
                    partner._state is MovingToPartnerState)
                {
                    if (_creature.GetDistanceToFocusObject() > _creature._sightRange)
                        ChooseWhatToDo();
                    else if (_creature.MoveToFocusedObject() < 0.7f)
                        _creature._newState = new ReproducingState(_creature);
                    else if (_creature._energy <= _creature._chromosome.EnrgAmntToStrtFdSrchGene.Value)
                        _creature._newState = new SeekingForFoodState(_creature);
                }
                else
                {
                    ChooseWhatToDo();
                }
            }

            /// <summary>
            /// Determines the creature's behavior when it cannot reach its partner.
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
