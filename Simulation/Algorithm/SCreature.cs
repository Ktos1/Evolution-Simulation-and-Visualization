using System;
using Godot;
using MathNet.Numerics.Distributions;
using ProjectEvolution.CommonStuff;

namespace ProjectEvolution.Simulation.Algorithm
{
    public partial class SCreature : MapObject
    {
        private static Random _randGen = new Random();
        private static uint IDCounter = 0;
        private SimulationController _controller;
        private uint _id = IDCounter++;

        private State _state;
        private MapObject _focusObject;
        private SChromosome _chromosome;

        private State _newState;
        private MapObject _newFocusObject;
        private Vector2 _newPosition;

        private Vector2 _movementDirection;
        private float _speed;
        private Tuple<float, float> _movementLimitations;

        private float _energy;
        private float _sightRange;

        private int _lifeDuration;
        private int _lifeTime = 0;
        private bool _newBorn;
        private bool _isGivingBirth;
         
        public SCreature(SimulationController controller)
        {
            _controller = controller;
            var(mapSizeX, mapSizeY) = controller.Map.Size;

            _position = _newPosition = new Vector2(
                _randGen.NextSingle() * mapSizeX - mapSizeX / 2f,
                _randGen.NextSingle() * mapSizeY - mapSizeY / 2f
                );
            _movementDirection = new Vector2(_randGen.NextSingle() * 2 - 1, _randGen.NextSingle() * 2 - 1).Normalized();
            _movementLimitations = new Tuple<float, float>(mapSizeX / 2f, mapSizeY / 2f);
            _speed = 2f;

            _sightRange = 1;
            _energy = 50;
            _chromosome = new SChromosome();
            _lifeDuration = _randGen.Next(250, 350);
            _newBorn = true;
            _state = _newState = new SeekingForPartnerState(this);
        }

        public SCreature(Vector2 position, SimulationController controller) : this(controller)
        {
            _position = _newPosition = position;
        }

        public void Process()
        {
            if (_state is ToDeleteState)
            {
                _controller.OnCreatureDeath(this);
                return;
            }  

            if (_lifeTime >= _lifeDuration && _state is not DiedState)
            {
                _newState = new DiedState(this);
            }
            _state.Process();
            _energy -= 0.01f;
            _lifeTime++;
        }

        public void Update()
        {
            _position = _newPosition;
            _state = _newState;
            _focusObject = _newFocusObject;
        }

        public (uint, (float x, float y), CreatureStates, float[]) GetSavingData()
        {
            (float x, float y) = _position;
            var state = _state.ConvertToEnum();
            if (_newBorn)
            {
                _newBorn = false;
                return (_id, (x, y), state, _chromosome.GetGenesValues());
            }
            else
            {
                return (_id, (x, y), state, null);
            }
        }

        private void ChooseWhatToDo()
        {
            if (_energy < 50)
            {
                _newState = new SeekingForFoodState(this);
            }
            else
            {
                _newState = new SeekingForPartnerState(this);
            }
        }

        private void GiveBirth(SCreature partner)
        {
            var childPosition = (partner.Position - _position) / 2 + _position;
            _controller.OnCreatureBirth(new SCreature(childPosition, _controller));
        }

        private float MoveToFocusedObject()
        {
            _movementDirection = (_focusObject.Position - _position).Normalized();
            Move();
            return (_focusObject.Position - _position).Length();
        }

        private void RandomMove()
        {
            if (_randGen.NextSingle() < _chromosome.TurningFrequencyGene.Value * 0.001)
            {
                var sign = (_randGen.Next(2) == 0) ? -1 : 1;
                var rotateAngle = sign * (float)Normal.Sample(_chromosome.TurningAngleGene.Value, 2.5) * (MathF.PI / 180);
                _movementDirection = _movementDirection.Rotated(rotateAngle);
            }
            Move();
        }

        private void Move()
        {
            var moveDist = CommonSettings.TICK_DURATION * _speed;
            _newPosition += _movementDirection * moveDist;

            if (_newPosition.X > _movementLimitations.Item1 || _newPosition.X < -_movementLimitations.Item1)
            {
                _newPosition -= _movementDirection * moveDist;
                _movementDirection.X = -_movementDirection.X;
                _newPosition += _movementDirection * moveDist;
            }
            if (_newPosition.Y > _movementLimitations.Item2 || _newPosition.Y < -_movementLimitations.Item2)
            {
                _newPosition -= _movementDirection * moveDist;
                _movementDirection.Y = -_movementDirection.Y;
                _newPosition += _movementDirection * moveDist;
            }
        }
    }
}