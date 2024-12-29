using Godot;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Visualization.ScenesStorages;
using ProjectEvolution.Visualization.ScenesElementsScripts;

/// <summary>
/// Contains classes responsible for visualization logic, not inheriting from godot classes.
/// </summary>
namespace ProjectEvolution.Visualization.LogicScripts
{
    /// <summary>
    /// Represents a creature in the visualization part.
    /// </summary>
    public class VCreature
    {
        /// <summary>
        /// The Godot node directly representing the creature object on the scene.
        /// </summary>
        private CreatureStaticBody _staticBody;
        /// <summary>
        /// The previous position of the creature, used for movement interpolation.
        /// </summary>
        private Vector2 _previousPosition;
        /// <summary>
        /// The next position of the creature, used for movement interpolation.
        /// </summary>
        private Vector2 _nextPosition;
        /// <summary>
        /// The state of the creature.
        /// </summary>
        private CreatureStates _state;
        /// <summary>
        /// Indicates whether the creature is dead.
        /// </summary>
        private bool _isDead = false;
        /// <summary>
        /// Indicates whether the creature is checked.
        /// </summary>
        private bool _isChecked;

        /// <summary>
        /// Gets or sets the flag indicating if the creature is checked.
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _staticBody.ChangeColorBasedOnSelection(value);
                _isChecked = value;
            }
        }

        /// <summary>
        /// Gets or privately sets the chromosome of the creature.
        /// </summary>
        public VChromosome Chromosome { get; private set; }

        /// <summary>
        /// Delegate for events returning a creature.
        /// </summary>
        public delegate void CreatureReturnEventHandler(VCreature creature);
        /// <summary>
        /// Event raised when the creature is clicked on.
        /// </summary>
        public event CreatureReturnEventHandler ClickedOn;
        /// <summary>
        /// Event raised when the creature is deleted.
        /// </summary>
        public event CreatureReturnEventHandler Deleted;

        /// <summary>
        /// Gets the Godot object directly representing the creature object on the scene.
        /// </summary>
        public StaticBody3D StaticBody => _staticBody;

        /// <summary>
        /// Initializes a new instance of the <see cref="VCreature"/> class.
        /// </summary>
        /// <param name="creatureData">
        /// The data of the creature.
        /// </param>
        public VCreature(CreatureTickData creatureData)
        {
            var (_, spawnPosition, state, chromosome) = creatureData;
            _state = state;
            Chromosome = chromosome;
            _previousPosition = _nextPosition = spawnPosition;
            InitializeCreatureStaticBodyNode();
            _staticBody.MoveTo(spawnPosition);
            _staticBody.InputEvent += OnInputEvent;
        }

        /// <summary>
        /// Processes the creature's movement.
        /// </summary>
        /// <param name="deltaCount">
        /// The time elapsed since the last tick.
        /// </param>
        public void Process(float deltaCount)
        {
            if (_previousPosition != _nextPosition && !_isDead)
            {
                _staticBody.MoveTo(_previousPosition.Lerp(
                    _nextPosition, deltaCount / CommonSettings.TICK_DURATION));
            }
        }

        /// <summary>
        /// Updates the creature's data for the tick.
        /// </summary>
        /// <param name="creatureData">
        /// The data of the creature for the tick.
        /// </param>
        public void Update(CreatureTickData creatureData)
        {
            var(_, _, state, _) = creatureData;
            if (state != CreatureStates.ToDelete)
            {
                if (!_isDead)
                {
                    var (_, position, _, _) = creatureData;
                    _previousPosition = _nextPosition;
                    _nextPosition = position;
                    _state = state;
                    if (_state == CreatureStates.Died)
                    {
                        Die();
                    }
                }
            }
            else
            {
                Delete();
            }
        }

        /// <summary>
        /// Deletes the creature.
        /// </summary>
        public void Delete()
        {
            _staticBody.QueueFree();
            Deleted.Invoke(this);
        }

        /// <summary>
        /// Sets the creature to the dead state.
        /// </summary>
        private void Die()
        {
            _staticBody.ChangeToDeathColor();
            _isDead = true;
        }

        /// <summary>
        /// Initializes the creature's static body node.
        /// </summary>
        /// <remarks>
        /// This method creates the creature's static body node and sets its height, 
        /// color saturation, and width based on the creature's genes.
        /// </remarks>
        private void InitializeCreatureStaticBodyNode()
        {
            _staticBody = Prefabs.Creature.Instantiate() as CreatureStaticBody;
            var maxEnergyGene = Chromosome.MaxEnergyAmountGene;
            var maxEnergyRange = maxEnergyGene.MaxValue - maxEnergyGene.MinValue;
            var heightFillness = (maxEnergyGene.Value - maxEnergyGene.MinValue) / maxEnergyRange;
            _staticBody.SetHeight(heightFillness);

            var sightGene = Chromosome.SightRangeGene;
            var saturation = sightGene.Value/ sightGene.MaxValue;
            _staticBody.SetColorSaturation(saturation);

            var angleGene = Chromosome.TurningAngleGene;
            var widthFillness = angleGene.Value / angleGene.MaxValue;
            _staticBody.SetWidth(widthFillness);
        }

        /// <summary>
        /// Handles the input event for the creature.
        /// </summary>
        /// <param name="camera">
        /// The camera node.
        /// </param>
        /// <param name="event">
        /// The input event.
        /// </param>
        /// <param name="eventPosition">
        /// The position of the event.
        /// </param>
        /// <param name="normal">
        /// The normal of the event.
        /// </param>
        /// <param name="shapeIdx">
        /// The shape index of the event.
        /// </param>
        private void OnInputEvent(Node camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, long shapeIdx)
        {
            if (@event.IsActionPressed("pick_object"))
            {
                IsChecked = true;
                ClickedOn.Invoke(this);
            }
        }
    }
}
