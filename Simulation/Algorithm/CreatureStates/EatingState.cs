namespace ProjectEvolution.Simulation.Algorithm
{
    public partial class SCreature
    {
        private class EatingState : DurationState
        {
            public EatingState(SCreature sCreature) : base(sCreature)
            {
                _stateDuration = SimulationSettings.EatingDuration;
            }

            public override void Process()
            {
                var eatenPlant = _creature._focusObject as SPlant;
                if (eatenPlant == null)
                {
                    _creature._newState = new SeekingForFoodState(_creature);
                }
                else if (_stateDuration == 0)
                {
                    eatenPlant.BeingEaten();
                    _creature._energy += SimulationSettings.EnergyFromPlantPart;
                    ChooseWhatToDo();
                }
                else _stateDuration--;
            }

            private void ChooseWhatToDo()
            {
                if (_creature._energy >= _creature._chromosome.EnrgAmntToStrtPrtnrSrchGene.Value)
                    _creature._newState = new SeekingForPartnerState(_creature);
                else
                    _creature._newState = new SeekingForFoodState(_creature);
            }
        }
    }
}