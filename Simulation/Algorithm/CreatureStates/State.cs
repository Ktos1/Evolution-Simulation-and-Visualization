using ProjectEvolution.CommonStuff;
using System;

namespace ProjectEvolution.Simulation.Algorithm
{
    public partial class SCreature
    {
        private abstract class State
        {
            protected SCreature _creature;

            protected State(SCreature sCreature)
            {
                _creature = sCreature;
            }

            public abstract void Process();

            public CreatureStates ConvertToEnum()
            {
                if (this is DiedState) return CreatureStates.Died;
                else if (this is MovingToPartnerState) return CreatureStates.MovingToPartner;
                else if (this is ReproducingState) return CreatureStates.Reproducing;
                else if (this is SeekingForPartnerState) return CreatureStates.SeekingForPartner;
                else if (this is ToDeleteState) return CreatureStates.ToDelete;
                else if (this is MovingToFoodState) return CreatureStates.MovingToFood;
                else if (this is SeekingForFoodState) return CreatureStates.SeekingForFood;
                else if (this is EatingState) return CreatureStates.Eating;
                throw new Exception("Occurred a creature state which is not handled.");
            }
        }
    }
}
