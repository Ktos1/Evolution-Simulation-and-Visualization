using Godot;
using ProjectEvolution.CommonStuff;
using System;

namespace ProjectEvolution.Visualization
{
    internal class VCreature
    {
        private StaticBody3D _staticBody;
        private Vector2 _previousPosition;
        private Vector2 _nextPosition;
        private CreatureStates _state;

        public readonly VChromosome Chromosome;

        public event EventHandler ClickedOn;

        public StaticBody3D StaticBody => _staticBody;

        public VCreature(Vector2 spawnPosition, CreatureStates state, VChromosome chromosome)
        {
            _state = state;
            Chromosome = chromosome;
            _previousPosition = _nextPosition = spawnPosition;
            InitializeStaticBodyNode();
            MoveTo(spawnPosition);
            _staticBody.InputEvent += OnInputEvent;
        }

        public void Process(float deltaCount)
        {
            if (_previousPosition != _nextPosition)
            {
                MoveTo(_previousPosition.Lerp(_nextPosition, deltaCount / CommonSettings.TICK_DURATION));
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

        public void Delete()
        {
            _staticBody.QueueFree();
        }

        private void ChangeColor(Color color)
        {
            var material = new StandardMaterial3D();
            material.AlbedoColor = color;
            _staticBody.GetNode<MeshInstance3D>("MeshInstance3D").SetSurfaceOverrideMaterial(0, material);
        }

        public void Uncheck()
        {
            ChangeColor(new Color("#de4040"));
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
            
        private async void Die()
        {
            ChangeColor(new Color(0.5f, 0.5f, 0.5f));
            var timer = new Timer();
            _staticBody.AddChild(timer);
            timer.WaitTime = CommonSettings.TICK_DURATION * 60;
            timer.OneShot = true;
            timer.Start();
            await _staticBody.ToSignal(timer, "timeout");

            _staticBody.QueueFree();
        }

        private void OnInputEvent(Node camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, long shapeIdx)
        {
            if (@event.IsActionPressed("pick_object"))
            {
                ChangeColor(new Color("#ff828c"));
                ClickedOn(this, null);
            }
        }
    }
}
