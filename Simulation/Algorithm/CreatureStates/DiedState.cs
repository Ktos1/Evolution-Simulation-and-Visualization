namespace ProjectEvolution.Simulation.Algorithm
{
    public partial class SCreature
    {
        private class DiedState : State
        {
            public DiedState(SCreature sCreature) : base(sCreature) { }

            public override void Process()
            {
                _creature._controller.OnCreatureDeath(_creature);
            }
        }
    }
}
