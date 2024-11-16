namespace ProjectEvolution.Simulation.Algorithm
{
    public partial class SCreature
    {
        private abstract class DurationState : State
        {
            protected int _stateDuration;

            protected DurationState(SCreature sCreature) : base(sCreature) { }
        }
    }
}
