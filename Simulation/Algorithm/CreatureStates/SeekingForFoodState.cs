namespace ProjectEvolution.Simulation.Algorithm
{
    public partial class SCreature
    {
        private class SeekingForFoodState : State
        {
            public SeekingForFoodState(SCreature creature) : base(creature)
            {
                _creature._newFocusObject = null;
            }

            public override void Process()
            {
                SPlant[] plantsInRange = _creature._controller.ObjectsInRange<SPlant>
                    (_creature, _creature._sightRange);

                if (plantsInRange.Length > 0)
                {
                    var nearestPlant = plantsInRange[0];
                    _creature._newFocusObject = nearestPlant;
                    _creature._newState = new MovingToFoodState(_creature);
                }
                else
                {
                    _creature.RandomMove();
                }
            }
        }
    }
}
