using System;
using Godot;
using MathNet.Numerics.Distributions;
using ProjectEvolution.CommonStuff;

namespace ProjectEvolution.Simulation.Algorithm
{
    public partial class SCreature : MapObject
    {
        private static Random _randGen = new Random();
        private static uint _idCounter = 0;
        private uint _id = _idCounter++;
        private SimulationController _controller;

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
        private float _energyCostPerTick;
        private float _movementEnergyCostPerUnit;

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

            _lifeDuration = _randGen.Next(1000, 1250);
            _energy = 30;
            
            _chromosome = new SChromosome();
            CalculateEnergyCosts();
            _sightRange = _chromosome.SightGene.Value;
            _speed = _chromosome.SpeedGene.Value;

            var isSearchingForFood = _energy <= _chromosome.EnrgAmntToStrtFdSrchGene.Value;
            _state = _newState = isSearchingForFood ? new SeekingForFoodState(this) : new SeekingForPartnerState(this);

            _newBorn = true;
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

            if (_state is not DiedState)
            {
                if (_lifeTime >= _lifeDuration || _energy <= 0)
                    _newState = new DiedState(this);
            }
            _state.Process();
            _energy -= _energyCostPerTick;
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

        public static void ResetIds()
        {
            _idCounter = 0;
        }

        private void CalculateEnergyCosts()
        {
            var sightCost = Mathf.Pow(_chromosome.SightGene.Value, 2) * (1 / 800f);
            _energyCostPerTick += sightCost;

            _movementEnergyCostPerUnit = Mathf.Pow(_chromosome.SpeedGene.Value, 2) * (1 / 95f);
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
            return GetDistanceToFocusObject();
        }

        private float GetDistanceToFocusObject()
        {
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
            _energy -= _movementEnergyCostPerUnit * moveDist;
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