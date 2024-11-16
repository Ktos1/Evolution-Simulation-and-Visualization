using ProjectEvolution.CommonStuff;
using System;
using System.Diagnostics;

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
                throw new Exception("Occurred a creature state which is not handled.");
            }
        }
    }
}
