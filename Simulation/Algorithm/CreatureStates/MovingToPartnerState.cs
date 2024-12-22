namespace ProjectEvolution.Simulation.Algorithm
{
    public partial class SCreature
    {
        private class MovingToPartnerState : State
        {
            public MovingToPartnerState(SCreature sCreature) : base(sCreature) { }

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
