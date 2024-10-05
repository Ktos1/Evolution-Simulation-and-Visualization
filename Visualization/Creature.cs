using Godot;
using ProjectEvolution.CommonStuff;

namespace ProjectEvolution.Visualization
{
    internal class Creature
    {
        private StaticBody3D _staticBody;

        private Vector2 _previousPosition;
        private Vector2 _nextPosition;
        private CreatureStates _state;
        private Chromosome _chromosome;

        public StaticBody3D StaticBody => _staticBody;

        public Creature(Vector2 spawnPosition, CreatureStates state, Chromosome chromosome)
        {
            _state = state;
            _chromosome = chromosome;
            _previousPosition = _nextPosition = spawnPosition;
            InitializeStaticBodyNode();
            MoveTo(spawnPosition);
        }

        public void Process(float deltaCount)
        {
            if (_previousPosition != _nextPosition)
            {
                MoveTo(_previousPosition.Lerp(_nextPosition, deltaCount / VisualizationSettings.SecondsBetweenTicks));
            }
        }

        public void Update(Vector2 position, CreatureStates state)
        {
            _previousPosition = _nextPosition;
            _nextPosition = position;
            _state = state;
            if (_state == CreatureStates.Died)
            {
                Die();
            }
        }

        private void MoveTo (Vector2 newPosition)
        {
            var oldPosition3D = _staticBody.Position;
            var oldPosition2D = new Vector2(oldPosition3D.X, oldPosition3D.Z);
            var movementVector = newPosition - oldPosition2D;

            Rotate(movementVector.Angle());
            _staticBody.Position = new Vector3(newPosition.X, 0, newPosition.Y);
        }

        private void InitializeStaticBodyNode()
        {
            _staticBody = Prefabs.Creature.Instantiate() as StaticBody3D;
        }

        private void Rotate(float angle)
        {
            _staticBody.Rotation = new Vector3(0, -angle, 0);
        }
            
        private async void Die()
        {
            var material = new StandardMaterial3D();
            material.AlbedoColor = new Color(0.5f, 0.5f, 0.5f);
            _staticBody.GetNode<MeshInstance3D>("MeshInstance3D").SetSurfaceOverrideMaterial(0, material);

            var timer = new Timer();
            _staticBody.AddChild(timer);
            timer.WaitTime = VisualizationSettings.SecondsBetweenTicks * 60;
            timer.OneShot = true;
            timer.Start();
            await _staticBody.ToSignal(timer, "timeout");

            _staticBody.QueueFree();
        }
    }
}
