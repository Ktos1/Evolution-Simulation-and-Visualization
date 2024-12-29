namespace ProjectEvolution.Simulation
{
    public partial class SCreature
    {
        /// <summary>
        /// Represents the eating state of a creature.
        /// </summary>
        private class EatingState : DurationState
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="EatingState"/> class.
            /// </summary>
            /// <param name="sCreature">
            /// <inheritdoc/>
            /// </param>
            public EatingState(SCreature sCreature) : base(sCreature)
            {
                _stateDuration = SimulationSettings.EatingDuration;
            }

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            public override void Process()
            {
                var eatenPlant = _creature._focusObject as SPlant;
                if (eatenPlant == null)
                {
                    _creature._newState = new SeekingForFoodState(_creature);
                }
                else if (_stateDuration == 0)
                {
                    eatenPlant.BeingEaten();
                    _creature._energy += SimulationSettings.EnergyFromPlantPart;
                    ChooseWhatToDo();
                }
                else _stateDuration--;
            }

            /// <summary>
            /// Chooses what creature should do after eating a plant.
            /// </summary>
            private void ChooseWhatToDo()
            {
                if (_creature._energy > _creature.GetBorderForPartnerSearch())
                    _creature._newState = new SeekingForPartnerState(_creature);
                else
                    _creature._newState = new SeekingForFoodState(_creature);
            }
        }
    }
}