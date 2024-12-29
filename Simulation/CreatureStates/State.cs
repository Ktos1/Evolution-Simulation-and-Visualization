using ProjectEvolution.CommonStuff;
using System;

namespace ProjectEvolution.Simulation
{
    public partial class SCreature
    {
        /// <summary>
        /// Represents state of a creature.
        /// </summary>
        private abstract class State
        {
            /// <summary>
            /// A <see cref="SCreature"/> object serves as the context in the State design pattern.
            /// </summary>
            protected SCreature _creature;

            /// <summary>
            /// Initializes a new instance of the <see cref="State"/> class.
            /// </summary>
            /// <param name="sCreature">
            /// A <see cref="SCreature"/> object serves as the context in the State design pattern.
            /// </param>
            protected State(SCreature sCreature)
            {
                _creature = sCreature;
            }

            /// <summary>
            /// Processes logic of the state of a creature for one tick.
            /// </summary>
            public abstract void Process();

            /// <summary>
            /// Converts a state instance to an <see cref="CreatureStates"/> 
            /// value which symbolizes it.
            /// </summary>
            /// <returns>
            /// A converted state to a <see cref="CreatureStates"/> value.
            /// </returns>
            /// <exception cref="NullReferenceException">
            /// Thrown when the state instance does not have a corresponding value in 
            /// the <see cref="CreatureStates"/>
            /// </exception>
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
                throw new NullReferenceException("Occurred a creature state which is not handled.");
            }
        }
    }
}
