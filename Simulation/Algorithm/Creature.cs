using System;
using Godot;
using ProjectEvolution.CommonStuff;

namespace ProjectEvolution.Simulation.Algorithm
{
    public class Creature : MapObject
    {
        private Random rand = new Random();
        private Vector2 _movementDirection;
        private float _speed;
        private float _sightRange;
        private int _lifeDuration;
        private int _lifeTime = 0;
        private bool _newBorn;

        private SimulationController _controller;

        private Tuple<float, float> _movementLimitations;
        private CreatureStates _state;

        private MutableChromosome _chromosome;

        private int _waitingTicks;

#nullable enable
        private MapObject? _focusObject;
#nullable disable

        public CreatureStates State => _state;

#nullable enable
        public MapObject? FocusObject => _focusObject;
#nullable disable

        public Creature(SimulationController controller)
        {
            _position = new Vector2(rand.NextSingle() * 10 - 5, rand.NextSingle() * 10 - 5);
            _movementDirection = (new Vector2(rand.NextSingle() * 2 - 1, rand.NextSingle() * 2 - 1)).Normalized();
            _speed = 0.5f;
            _controller = controller;
            _sightRange = 2;
            _chromosome = new MutableChromosome();
            _lifeDuration = rand.Next(40, 60);
            _newBorn = true;

            var tiles = _controller.Map.Tiles;

            _movementLimitations = new Tuple<float, float>(tiles.GetLength(0)/2f, tiles.GetLength(1)/2f);
            _state = CreatureStates.SeekingForPartner;
        }

        public Creature(Vector2 position, SimulationController controller) : this(controller)
        {
            _position = position;
        }

        public void Process()
        {
            if (_lifeTime >= _lifeDuration)
            {
                Die();
            }
            else if (_waitingTicks == 0)
            {
                switch (_state)
                {
                    case CreatureStates.RandomMoves:
                        RandomMove();
                        break;

                    case CreatureStates.Stop:
                        break;
#nullable enable
                    case CreatureStates.SeekingForPartner:
                        Creature? creatureInRange = _controller.IsItInRange<Creature>(this, _sightRange);

                        if (creatureInRange is not null &&
                            (creatureInRange.State == CreatureStates.SeekingForPartner ||
                            creatureInRange.FocusObject == this &&
                            creatureInRange.State == CreatureStates.MovingToPartner
                            ))
                        {
                            creatureInRange.SetMovingToPartnerState(this);
                            creatureInRange.Wait(1);
                            _focusObject = creatureInRange;
                            _state = CreatureStates.MovingToPartner;
                        }
                        else
                        {
                            RandomMove();
                        }
                        break;
#nullable disable
                    case CreatureStates.MovingToPartner:
                        if (MoveToFocusedObject() < 0.3f)
                        {
                            var focusCreature = (Creature)_focusObject;
                            _state = CreatureStates.Stop;
                            focusCreature.Reproduce();
                            focusCreature.AddWaitingTime(1);
                            Reproduce();
                        }
                        break;

                    case CreatureStates.Reproducing:
                        var partner = (Creature)_focusObject;
                        partner.ChooseWhatToDo();
                        GiveBirth(partner);
                        ChooseWhatToDo();
                        break;

                    case CreatureStates.Died:
                        _controller.OnCreatureDeath(this);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                _waitingTicks--;
            }
            _lifeTime++;
        }

        public void SetMovingToPartnerState(Creature partner)
        {
            if (_state == CreatureStates.SeekingForPartner)
            {
                _state = CreatureStates.MovingToPartner;
                _focusObject = partner;
            }
        }

        // TODO: In a future this should be a method which pick the specified state of creature
        // basing on the creature data for example the energy
        public void ChooseWhatToDo()
        {
            _state = CreatureStates.Stop;
            _focusObject = null;
        }

        public void Reproduce()
        {
            _state = CreatureStates.Reproducing;
            Wait(SimulationSettings.ReproductionTime);
        }

        public (Vector2, CreatureStates, float[]) GetSavingData()
        {
            if (_newBorn)
            {
                _newBorn = false;
                return (_position, _state, _chromosome.Genes);
            }
            else
            {
                return (_position, _state, null);
            }
        }

        public void Wait(int timeInTicks)
        {
            _waitingTicks = timeInTicks;
        }

        public void AddWaitingTime(int timeInTicks)
        {
            _waitingTicks += timeInTicks;
        }

        private void GiveBirth(Creature partner)
        {
            var childPosition = (partner.Position - _position) / 2 + _position;
            _controller.OnCreatureBirth(new Creature(childPosition, _controller));
        }

        private void Die()
        {
            _state = CreatureStates.Died;
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
            var rotateAngle = (rand.NextSingle() * 10 - 5) * (MathF.PI / 180);
            _movementDirection.Rotated(rotateAngle);
            Move();
        }

        private void Move()
        {
            var moveDist = SimulationController.DELTA_TIME * _speed;
            _position += _movementDirection * moveDist;

            if (_position.X > _movementLimitations.Item1 || _position.X < -_movementLimitations.Item1)
            {
                _position -= _movementDirection * moveDist;
                _movementDirection.X = -_movementDirection.X;
                _position += _movementDirection * moveDist;
            }
            if (_position.Y > _movementLimitations.Item2 || _position.Y < -_movementLimitations.Item2)
            {
                _position -= _movementDirection * moveDist;
                _movementDirection.Y = -_movementDirection.Y;
                _position += _movementDirection * moveDist;
            }
        }
    }
}