namespace ProjectEvolution.Simulation.Algorithm
{
    public partial class SCreature
    {
        private class DiedState : DurationState
        {
            public DiedState(SCreature sCreature) : base(sCreature)
            {
                _stateDuration = SimulationSettings.DiedStateDuration;
            }

            public override void Process()
            {
                if (_stateDuration == 1)
                {
                    _creature._newState = new ToDeleteState(_creature);
                }
                else
                {
                    _stateDuration--;
                }
            }
        }
    }
}
