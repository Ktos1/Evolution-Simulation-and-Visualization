namespace ProjectEvolution.Simulation.Algorithm
{
    public partial class SCreature
    {
        private class ReproducingState : DurationState
        {
            public ReproducingState(SCreature sCreature) 
                : base(sCreature)
            {
                _stateDuration = SimulationSettings.ReproductionDuration;
                _creature._isGivingBirth = false;
            }

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

            private void ChooseWhatToDo()
            {
                if (_creature._energy <= _creature._chromosome.EnergyAmountToStartFoodSearchGene.Value)
                    _creature._newState = new SeekingForFoodState(_creature);
                else
                    _creature._newState = new SeekingForPartnerState(_creature);
            }
        }
    }
}
