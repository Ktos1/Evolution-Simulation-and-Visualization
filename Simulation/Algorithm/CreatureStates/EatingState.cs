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
                    _creature.ChooseWhatToDo();
                }
                else if (_stateDuration == 0)
                {
                    eatenPlant.BeingEaten();
                    _creature._energy += SimulationSettings.EnergyFromPlantPart;
                    _creature.ChooseWhatToDo();
                }
                else _stateDuration--;
            }
        }
    }
}