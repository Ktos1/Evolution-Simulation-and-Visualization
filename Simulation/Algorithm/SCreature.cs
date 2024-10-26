using System;
using Godot;
using MathNet.Numerics.Distributions;
using ProjectEvolution.CommonStuff;

namespace ProjectEvolution.Simulation.Algorithm
{
    public class SCreature : MapObject
    {
        private static Random _randGen = new Random();
        private SimulationController _controller;

        private CreatureStates _state;
        private MapObject _focusObject;
        private SChromosome _chromosome;

        private CreatureStates _newState;
        private MapObject _newFocusObject;
        private Vector2 _newPosition;

        private Vector2 _movementDirection;
        private float _speed;
        private Tuple<float, float> _movementLimitations;

        private float _sightRange;

        private int _stateDuration;
        private int _lifeDuration;
        private int _lifeTime = 0;
        private bool _newBorn;
        private bool _isGivingBirth;

        public CreatureStates State => _state;

        public MapObject FocusObject => _focusObject;

        public bool IsGivingBirth { get; set; }

        public SCreature(SimulationController controller)
        {
            _position = _newPosition = new Vector2(_randGen.NextSingle() * 10 - 5, _randGen.NextSingle() * 10 - 5);
            _movementDirection = (new Vector2(_randGen.NextSingle() * 2 - 1, _randGen.NextSingle() * 2 - 1)).Normalized();
            _speed = 2f;
            _controller = controller;
            _sightRange = 1;
            _chromosome = new SChromosome();
            _lifeDuration = _randGen.Next(250, 350);
            _newBorn = true;

            var tiles = _controller.Map.Tiles;

            _movementLimitations = new Tuple<float, float>(tiles.GetLength(0)/2f, tiles.GetLength(1)/2f);
            _state = _newState = CreatureStates.SeekingForPartner;
        }

        public SCreature(Vector2 position, SimulationController controller) : this(controller)
        {
            _position = _newPosition = position;
        }

        public void Process()
        {
            if (_lifeTime >= _lifeDuration)
            {
                Die();
            }
            else
            {
                switch (_state)
                {
                    case CreatureStates.RandomMoves:
                        RandomMove();
                        break;

                    case CreatureStates.Stop:
                        break;

                    case CreatureStates.SeekingForPartner:
                    {
                        SCreature[] creaturesInRange = _controller.ObjectsInRange<SCreature>(this, _sightRange);
                        var isPartnerFounded = false;
                        if (creaturesInRange is not null)
                        {
                            foreach (var creatureInRange in creaturesInRange)
                            {
                                if (creatureInRange.State == CreatureStates.SeekingForPartner ||
                                creatureInRange.FocusObject == this &&
                                creatureInRange.State == CreatureStates.MovingToPartner)
                                {
                                    _newFocusObject = creatureInRange;
                                    _newState = CreatureStates.MovingToPartner;
                                    isPartnerFounded = true;
                                    break;
                                }
                            }
                        }
                        if (!isPartnerFounded)
                        {
                            RandomMove();
                        }
                        break;
                    }

                    case CreatureStates.MovingToPartner:
                        {
                            var partner = (SCreature)_focusObject;
                            if (partner.State == CreatureStates.SeekingForPartner ||
                                partner.FocusObject == this &&
                                partner.State == CreatureStates.MovingToPartner)
                            {
                                if (MoveToFocusedObject() < 0.3f)
                                {
                                    Reproduce();
                                }
                            }
                            else
                            {
                                SeekForPartner();
                            }
                            break;
                        }

                    case CreatureStates.Reproducing:
                        {
                            var partner = (SCreature)_focusObject;
                            if (partner.State != CreatureStates.Reproducing)
                            {
                                ChooseWhatToDo();
                            }
                            else if (_stateDuration == 0)
                            {
                                if (!partner.IsGivingBirth)
                                {
                                    IsGivingBirth = true;
                                    GiveBirth(partner);
                                    ChooseWhatToDo();
                                }
                                else
                                {
                                    partner.IsGivingBirth = false;
                                    ChooseWhatToDo();
                                }
                                Die();
                            }
                            else
                            {
                                _stateDuration--;
                            }
                            break;
                        }

                    case CreatureStates.Died:
                        _controller.OnCreatureDeath(this);
                        break;

                    default:
                        break;
                }
            }
            _lifeTime++;
        }

        public void Update()
        {
            _position = _newPosition;
            _state = _newState;
            _focusObject = _newFocusObject;
        }

        // TODO: In a future this should be a method which pick the specified state of creature
        // basing on the creature data for example the energy
        public void ChooseWhatToDo()
        {
            SeekForPartner();
        }

        private void Reproduce()
        {
            _newState = CreatureStates.Reproducing;
            _stateDuration = SimulationSettings.ReproductionTime;
            IsGivingBirth = false;
        }

        private void SeekForPartner()
        {
            _newFocusObject = null;
            _newState = CreatureStates.SeekingForPartner;
        }

        public (Vector2, CreatureStates, float[]) GetSavingData()
        {
            if (_newBorn)
            {
                _newBorn = false;
                return (_position, _state, _chromosome.GetGenesValues());
            }
            else
            {
                return (_position, _state, null);
            }
        }

        private void GiveBirth(SCreature partner)
        {
            var childPosition = (partner.Position - _position) / 2 + _position;
            _controller.OnCreatureBirth(new SCreature(childPosition, _controller));
        }

        private void Die()
        {
            _newState = CreatureStates.Died;
            _lifeTime -= 2;
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
                var sign = _randGen.Next(2);
                if (_randGen.Next(2) == 0)
                {
                    sign = -1;
                }
                else
                {
                    sign = 1;
                }
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