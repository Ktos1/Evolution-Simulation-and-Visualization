using Godot;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Visualization.LogicScripts;

namespace ProjectEvolution.Visualization
{
    public class VCreature
    {
        private VCreaturesManager _creaturesManager;
        private StaticBody3D _staticBody;
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
                if (value)
                {
                    if (_isDead) ChangeColor(new Color(0.75f, 0.75f, 0.75f));
                    else ChangeColor(new Color("#ff828c"));
                }
                else
                {
                    if (_isDead) ChangeColor(new Color(0.5f, 0.5f, 0.5f));
                    else ChangeColor(new Color("#de4040"));
                }
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
            MoveTo(spawnPosition);
            _staticBody.InputEvent += OnInputEvent;
            _creaturesManager = vCreaturesManager;
        }

        public void Process(float deltaCount)
        {
            if (_previousPosition != _nextPosition && !_isDead)
            {
                MoveTo(_previousPosition.Lerp(_nextPosition, deltaCount / CommonSettings.TICK_DURATION));
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
            if (IsChecked) ChangeColor(new Color(0.75f, 0.75f, 0.75f));
            else ChangeColor(new Color(0.5f, 0.5f, 0.5f));
            _isDead = true;
        }

        private void ChangeColor(Color color)
        {
            var material = new StandardMaterial3D();
            material.AlbedoColor = color;
            _staticBody.GetNode<MeshInstance3D>("MeshInstance3D").SetSurfaceOverrideMaterial(0, material);
        }

        private void MoveTo (Vector2 newPosition)
        {
            var oldPosition3D = _staticBody.Position;
            var oldPosition2D = new Vector2(oldPosition3D.X, oldPosition3D.Z);
            var movementVector = newPosition - oldPosition2D;

            Rotate(movementVector.Angle());
            _staticBody.Position = new Vector3(newPosition.X, 0.75f, newPosition.Y);
        }

        private void InitializeStaticBodyNode()
        {
            _staticBody = Prefabs.Creature.Instantiate() as StaticBody3D;
        }

        private void Rotate(float angle)
        {
            _staticBody.Rotation = new Vector3(0, -angle, 0);
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
