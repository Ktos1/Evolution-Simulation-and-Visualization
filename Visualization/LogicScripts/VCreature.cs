using Godot;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Visualization.LogicScripts;

namespace ProjectEvolution.Visualization
{
    public class VCreature
    {
        private VCreaturesManager _creaturesManager;
        private CreatureStaticBody _staticBody;
        private uint _id;
        private Vector2 _previousPosition;
        private Vector2 _nextPosition;
        private CreatureStates _state;
        private bool _isDead = false;
        private bool _isChecked;

        private bool IsChecked
        {
            get => _isChecked;
            set
            {
                _staticBody.ChangeColorBasedOnSelection(value);
                _isChecked = value;
            }
        }

        public VChromosome Chromosome { get; private set; }

        public delegate void CreatureReturnEventHandler(VCreature creature);
        public event CreatureReturnEventHandler ClickedOn;
        public event CreatureReturnEventHandler Deleted;

        public StaticBody3D StaticBody => _staticBody;

        public VCreature(CreatureTickData creatureData, VCreaturesManager vCreaturesManager)
        {
            var (id, spawnPosition, state, chromosome) = creatureData;
            _id = id;
            _state = state;
            Chromosome = chromosome;
            _previousPosition = _nextPosition = spawnPosition;
            InitializeStaticBodyNode();
            _staticBody.MoveTo(spawnPosition);
            _staticBody.InputEvent += OnInputEvent;
            _creaturesManager = vCreaturesManager;
        }

        public void Process(float deltaCount)
        {
            if (_previousPosition != _nextPosition && !_isDead)
            {
                _staticBody.MoveTo(_previousPosition.Lerp(
                    _nextPosition, deltaCount / CommonSettings.TICK_DURATION));
            }
        }

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

        public void Uncheck()
        {
            IsChecked = false;
        }

        public void Delete()
        {
            _staticBody.QueueFree();
            Deleted.Invoke(this);
        }

        private void Die()
        {
            _staticBody.ChangeToDeathColor();
            _isDead = true;
        }

        private void InitializeStaticBodyNode()
        {
            _staticBody = Prefabs.Creature.Instantiate() as CreatureStaticBody;
            var maxEnergyGene = Chromosome.MaxEnergyAmountGene;
            var maxEnergyRange = maxEnergyGene.MaxValue - maxEnergyGene.MinValue;
            var heightFillness = (maxEnergyGene.Value - maxEnergyGene.MinValue) / maxEnergyRange;
            _staticBody.SetHeight(heightFillness);

            var sightGene = Chromosome.SightGene;
            var saturation = sightGene.Value/ sightGene.MaxValue;
            _staticBody.SetColorSaturation(saturation);
        }

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
