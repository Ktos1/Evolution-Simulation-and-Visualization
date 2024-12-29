using System;
using Godot;
using MathNet.Numerics.Distributions;
using ProjectEvolution.CommonStuff;

namespace ProjectEvolution.Simulation
{
    /// <summary>
    /// Represents a creature in the simulation part.
    /// </summary>
    public partial class SCreature : MapObject
    {
        /// <summary>
        /// The random number generator shared by all instances of the 
        /// <see cref="SCreature"/> class.
        /// </summary>
        private static Random _randGen = new Random();
        /// <summary>
        /// The counter used to assign unique identifiers to the creatures.
        /// </summary>
        private static uint _idCounter = 0;
        /// <summary>
        /// The unique identifier of the creature.
        /// </summary>
        private uint _id = _idCounter++;
        /// <summary>
        /// The controller of the simulation.
        /// </summary>
        private SimulationController _controller;
        /// <summary>
        /// The creatures manager containing the creature.
        /// </summary>
        private SCreaturesManager _creaturesManager;
        /// <summary>
        /// The state of the creature.
        /// </summary>
        private State _state;
        /// <summary>
        /// The focus object of the creature.
        /// </summary>
        private MapObject _focusObject;
        /// <summary>
        /// The chromosome of the creature.
        /// </summary>
        private SChromosome _chromosome;

        /// <summary>
        /// The new state of the creature.
        /// </summary>
        /// <remarks>
        /// It is used to change the right state of the creature after the current 
        /// tick is processed.
        /// </remarks>
        private State _newState;
        /// <summary>
        /// The new focus object of the creature.
        /// </summary>
        /// <remarks>
        /// It is used to change the right focus object of the creature after the current
        /// tick is processed.
        /// </remarks>
        private MapObject _newFocusObject;
        /// <summary>
        /// The new position of the creature.
        /// </summary>
        /// <remarks>
        /// It is used to change the right position of the creature after the current
        /// tick is processed.
        /// </remarks>
        private Vector2 _newPosition;

        /// <summary>
        /// The movement direction of the creature.
        /// </summary>
        private Vector2 _movementDirection;
        /// <summary>
        /// The speed of the creature.
        /// </summary>
        private float _speed;
        /// <summary>
        /// The limitations of the movement of the creature on the map.
        /// </summary>
        private Tuple<float, float> _movementLimitations;

        /// <summary>
        /// The energy of the creature.
        /// </summary>
        private float _energy;
        /// <summary>
        /// The sight range of the creature.
        /// </summary>
        private float _sightRange;
        /// <summary>
        /// The energy cost per tick of the creature.
        /// </summary>
        private float _energyCostPerTick;
        /// <summary>
        /// The life time cost of the creature.
        /// </summary>
        /// <remarks>
        /// It is calculated on every tick as the square function of the life time 
        /// of the creature. It is incurred in every tick.
        /// </remarks>
        private float _lifeTimeCost;
        /// <summary>
        /// The movement energy cost per unit of the creature.
        /// </summary>
        private float _movementEnergyCostPerUnit;

        /// <summary>
        /// The life duration of the creature.
        /// </summary>
        private int _lifeDuration;
        /// <summary>
        /// The life time of the creature.
        /// </summary>
        private int _lifeTime = 0;

        /// <summary>
        /// Indicates whether the creature is a new born.
        /// </summary>
        private bool _newBorn;
        /// <summary>
        /// Indicates whether the creature is giving birth.
        /// </summary>
        /// <remarks>
        /// It is used to indicate which partner is giving birth.
        /// </remarks>
        private bool _isGivingBirth;

        /// <summary>
        /// Initializes a new instance of the <see cref="SCreature"/> class.
        /// </summary>
        /// <remarks>
        /// The new creature is initialized with random position on the map and
        /// random values for its genes.
        /// </remarks>
        /// <param name="controller">
        /// The controller of the simulation.
        /// </param>
        public SCreature(SCreaturesManager creaturesManager, SimulationController controller)
        {
            Initialize(null, null, controller, creaturesManager);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SCreature"/> class.
        /// </summary>
        /// <param name="position">
        /// The position of the creature on the map.
        /// </param>
        /// <param name="chromosome">
        /// The chromosome of the creature.
        /// </param>
        /// <param name="controller">
        /// The controller of the simulation.
        /// </param>
        public SCreature(
            Vector2 position,
            SChromosome chromosome,
            SimulationController controller,
            SCreaturesManager creaturesManager)
        {
            Initialize(position, chromosome, controller, creaturesManager);
        }

        /// <summary>
        /// Processes the creature logic for one tick.
        /// </summary>
        public void Process()
        {
            if (_state is ToDeleteState)
            {
                _creaturesManager.OnCreatureDeletion(this);
                return;
            }

            if (_state is not DiedState)
            {
                if (_lifeTime >= _lifeDuration || _energy <= 0)
                    _newState = new DiedState(this);
            }
            _state.Process();
            CalculateLifeTimeCost();
            _energy -= _energyCostPerTick + _lifeTimeCost;
            _lifeTime++;
        }

        /// <summary>
        /// Updates the creature state, position and focus object.
        /// </summary>
        /// <remarks>
        /// It is called after the current tick is processed.
        /// </remarks>
        public void Update()
        {
            _position = _newPosition;
            _state = _newState;
            _focusObject = _newFocusObject;
        }

        /// <summary>
        /// Gets the data of the creature to save it in the file.
        /// </summary>
        /// <returns>
        /// A tuple containing the id, position, state and genes values of the creature.
        /// </returns>
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

        /// <summary>
        /// Resets the ids counter of the creatures to 0.
        /// </summary>
        public static void ResetIds()
        {
            _idCounter = 0;
        }

        /// <summary>
        /// Gives birth to a new creature.
        /// </summary>
        /// <param name="partner">
        /// The second parent of the new creature.
        /// </param>
        private void GiveBirth(SCreature partner)
        {
            var childPosition = (partner.Position - _position) / 2 + _position;
            var childChromosome = _chromosome.GetChildChromosome(partner._chromosome);
            var child = new SCreature(childPosition, childChromosome, _controller, _creaturesManager);
            child._energy += GiveAdditEnergyForChild() + partner.GiveAdditEnergyForChild();
            _creaturesManager.OnCreatureBirth(child);
        }

        /// <summary>
        /// Moves the creature to the focused object.
        /// </summary>
        /// <remarks>
        /// The creature moves a distance corresponding to its speed.
        /// </remarks>
        /// <returns>
        /// The distance to the focused object.
        /// </returns>
        private float MoveToFocusedObject()
        {
            _movementDirection = (_focusObject.Position - _position).Normalized();
            Move();
            return GetDistanceToFocusObject();
        }

        /// <summary>
        /// Gets the distance to the focused object.
        /// </summary>
        /// <returns>
        /// The distance to the focused object.
        /// </returns>
        private float GetDistanceToFocusObject()
        {
            return (_focusObject.Position - _position).Length();
        }

        /// <summary>
        /// Moves the creature according to the random move genes.
        /// </summary>
        /// <remarks>
        /// The creature change its direction with a probability corresponding to its
        /// turning frequency gene. The turning angle is chosen from a normal distribution
        /// with mean equal to the turning angle gene value and standard deviation
        /// equal to 2.5.
        /// </remarks>
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

        /// <summary>
        /// Moves the creature.
        /// </summary>
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

        /// <summary>
        /// Initializes the creature.
        /// </summary>
        /// <param name="position">
        /// The position of the creature on the map. If it is null, the position is 
        /// chosen randomly.
        /// </param>
        /// <param name="chromosome">
        /// The chromosome of the creature. If it is null, the chromosome is created
        /// with random genes values.
        /// </param>
        /// <param name="controller">
        /// The controller of the simulation.
        /// </param>
        private void Initialize(
            Vector2? position,
            SChromosome chromosome,
            SimulationController controller,
            SCreaturesManager creaturesManager)
        {
            _controller = controller;
            _creaturesManager = creaturesManager;
            var (mapSizeX, mapSizeY) = controller.Map.Size;

            _energy = 20;

            if (chromosome != null) _chromosome = chromosome;
            else _chromosome = new SChromosome();
            CalculateEnergyCosts();
            _lifeDuration = Mathf.RoundToInt(_chromosome.LifeDurationGene.Value);
            _sightRange = _chromosome.SightRangeGene.Value;
            _speed = _chromosome.SpeedGene.Value;

            var isSearchingForFood = _energy <= GetBorderForFoodSearch();
            _state = _newState = isSearchingForFood ? new SeekingForFoodState(this) : new SeekingForPartnerState(this);

            _newBorn = true;

            if (position.HasValue)
                _position = _newPosition = position.Value;
            else
                _position = _newPosition = new Vector2(
                _randGen.NextSingle() * mapSizeX - mapSizeX / 2f,
                _randGen.NextSingle() * mapSizeY - mapSizeY / 2f
                );
            _movementDirection = new Vector2(_randGen.NextSingle() * 2 - 1, _randGen.NextSingle() * 2 - 1).Normalized();
            _movementLimitations = new Tuple<float, float>(mapSizeX / 2f, mapSizeY / 2f);
        }

        /// <summary>
        /// Calculates the energy costs of the creature.
        /// </summary>
        private void CalculateEnergyCosts()
        {
            var sightCost = Mathf.Pow(_chromosome.SightRangeGene.Value, 2) * (1 / 800f);
            _energyCostPerTick += sightCost;

            _movementEnergyCostPerUnit = Mathf.Pow(_chromosome.SpeedGene.Value, 2) * (1 / 95f);
        }

        /// <summary>
        /// Calculates the life time cost of the creature.
        /// </summary>
        private void CalculateLifeTimeCost()
        {
            _lifeTimeCost = Mathf.Pow(_lifeTime, 2) * (1 / 20000000f);
        }

        /// <summary>
        /// Gives additional energy for the child.
        /// </summary>
        /// <remarks>
        /// The energy given to the child is taken from the energy of the parent. 
        /// Amount of energy is determined by the additional energy for child gene.
        /// </remarks>
        /// <returns>
        /// The energy given to the child.
        /// </returns>
        private float GiveAdditEnergyForChild()
        {
            var valueFromGene = _chromosome.AdditEnrgyForChldGene.Value;
            float outValue = 0;
            if (valueFromGene > _energy)
            {
                outValue = _energy;
                _energy = 0;
            }
            else
            {
                outValue = valueFromGene;
                _energy -= outValue;
            }
            return outValue;
        }

        /// <summary>
        /// Calculates the energy amount which is the border for the food search.
        /// </summary>
        /// <returns>
        /// The energy amount which is the border for the food search.
        /// </returns>
        private float GetBorderForFoodSearch()
        {
            return _chromosome.EnrgAmntToStrtFdSrchGene.Value * 0.01f *
                    _chromosome.MaxEnergyAmountGene.Value;
        }

        /// <summary>
        /// Calculates the energy amount which is the border for the partner search.
        /// </summary>
        /// <returns>
        /// The energy amount which is the border for the partner search.
        /// </returns>
        private float GetBorderForPartnerSearch()
        {
            return _chromosome.EnrgAmntToStrtPrtnrSrchGene.Value * 0.01f *
                    _chromosome.MaxEnergyAmountGene.Value;
        }
    }
}