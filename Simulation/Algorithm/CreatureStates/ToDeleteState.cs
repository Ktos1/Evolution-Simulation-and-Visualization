namespace ProjectEvolution.Simulation.Algorithm
{
    public partial class SCreature
    {
        private class ToDeleteState : State
        {
            public ToDeleteState(SCreature sCreature) : base(sCreature) { }

            public override void Process() { }
        }
    }
}
