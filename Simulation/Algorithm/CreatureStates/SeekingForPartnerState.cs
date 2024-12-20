namespace ProjectEvolution.Simulation.Algorithm
{
    public partial class SCreature
    {
        private class SeekingForPartnerState : State
        {
            public SeekingForPartnerState(SCreature sCreature) : base(sCreature)
            {
                _creature._newFocusObject = null;
            }

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
