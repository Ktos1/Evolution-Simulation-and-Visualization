namespace ProjectEvolution.Simulation
{
    public partial class SCreature
    {
        /// <summary>
        /// Represents the seeking for partner state of a creature.
        /// </summary>
        private class SeekingForPartnerState : State
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SeekingForPartnerState"/> class.
            /// </summary>
            /// <param name="sCreature">
            /// <inheritdoc/>
            /// </param>
            public SeekingForPartnerState(SCreature sCreature) : base(sCreature)
            {
                _creature._newFocusObject = null;
            }

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            public override void Process()
            {
                SCreature[] creaturesInRange = _creature._controller.ObjectsInRange<SCreature>
                    (_creature, _creature._sightRange);
                var isPartnerFounded = false;
                foreach (var creatureInRange in creaturesInRange)
                {
                    if (creatureInRange._state is SeekingForPartnerState ||
                    creatureInRange._focusObject == _creature &&
                    creatureInRange._state is MovingToPartnerState)
                    {
                        _creature._newFocusObject = creatureInRange;
                        _creature._newState = new MovingToPartnerState(_creature);
                        isPartnerFounded = true;
                        break;
                    }
                }
                if (!isPartnerFounded)
                {
                    _creature.RandomMove();
                }
                ChooseWhatToDo();
            }

            /// <summary>
            /// Defines what creature should do in next tick.
            /// </summary>
            private void ChooseWhatToDo()
            {
                if (_creature._energy < _creature.GetBorderForFoodSearch())
                {
                    _creature._newState = new SeekingForFoodState(_creature);
                }
            }
        }
    }
}
