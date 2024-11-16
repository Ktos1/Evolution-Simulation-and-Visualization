namespace ProjectEvolution.Simulation.Algorithm
{
    public partial class SCreature
    {
        private class ReproducingState : DurationState
        {
            public ReproducingState(SCreature sCreature) 
                : base(sCreature)
            {
                _stateDuration = SimulationSettings.ReproductionTime;
                _creature._isGivingBirth = false;
            }

            public override void Process()
            {
                var partner = (SCreature)_creature._focusObject;
                if (partner._state is not ReproducingState)
                {
                    _creature.ChooseWhatToDo();
                }
                else if (_stateDuration == 0)
                {
                    if (!partner._isGivingBirth)
                    {
                        _creature._isGivingBirth = true;
                        _creature.GiveBirth(partner);
                        _creature.ChooseWhatToDo();
                    }
                    else
                    {
                        partner._isGivingBirth = false;
                        _creature.ChooseWhatToDo();
                    }
                    _creature._newState = new DiedState(_creature);
                }
                else
                {
                    _stateDuration--;
                }
            }
        }
    }
}
