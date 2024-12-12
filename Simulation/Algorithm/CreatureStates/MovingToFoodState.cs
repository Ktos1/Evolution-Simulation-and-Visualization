namespace ProjectEvolution.Simulation.Algorithm
{
    public partial class SCreature
    {
        private class MovingToFoodState : State
        {
            public MovingToFoodState(SCreature sCreature) : base(sCreature) { }

            public override void Process()
            {
                if (_creature.GetDistanceToFocusObject() < 0.3 ||
                    _creature.MoveToFocusedObject() < 0.3f)
                {
                    _creature._newState = new EatingState(_creature);
                }
            }
        }
    }
}
