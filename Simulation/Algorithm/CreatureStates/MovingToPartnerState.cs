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
                    if (_creature.MoveToFocusedObject() < 0.3f)
                    {
                        _creature._newState = new ReproducingState(_creature);
                    }
                }
                else
                {
                    _creature._newState = new SeekingForPartnerState(_creature);
                }
            }
        }
    }
}
