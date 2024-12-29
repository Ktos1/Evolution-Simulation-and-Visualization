using Godot;
using Godot.Collections;
using MathNet.Numerics.Random;
using System;
using ProjectEvolution.Visualization.ScenesStorages;
using ProjectEvolution.Visualization.ScenesElementsScripts;

namespace ProjectEvolution.Visualization.LogicScripts
{
    /// <summary>
    /// Represents a plant in the visualization part.
    /// </summary>
    internal class VPlant
    {
        /// <summary>
        /// The ID of the plant.
        /// </summary>
        private uint _id;
        /// <summary>
        /// The position of the plant.
        /// </summary>
        private Vector2 _position;
        /// <summary>
        /// The number of parts of the plant.
        /// </summary>
        private int _partsNumber;
        /// <summary>
        /// The random generator shared by all instances of the <see cref="VPlant"/> 
        /// class.
        /// </summary>
        private static Random _randGen = new Random();

        /// <summary>
        /// The Godot node directly representing the plant object on the scene.
        /// </summary>
        private PlantStaticBody _plantStaticBody;
        /// <summary>
        /// The dictionary containing the fruits positions for each plant.
        /// </summary>
        private static Dictionary<uint, Vector3[]> _fruitsPositions = 
            new Dictionary<uint, Vector3[]>();

        /// <summary>
        /// Delegate for events returning a plant.
        /// </summary>
        public delegate void PlantReturnEventHandler(VPlant creature);
        /// <summary>
        /// The event that is invoked when the plant is deleted.
        /// </summary>
        public event PlantReturnEventHandler Deleted;

        /// <summary>
        /// Gets the ID of the plant.
        /// </summary>
        public uint ID => _id;
        /// <summary>
        /// Gets the Godot node directly representing the plant object on the scene.
        /// </summary>
        public PlantStaticBody PlantStaticBody => _plantStaticBody;

        /// <summary>
        /// Initializes a new instance of the <see cref="VPlant"/> class.
        /// </summary>
        /// <param name="plantTickData">
        /// The data of the plant.
        /// </param>
        /// <param name="vPlantManager">
        /// A manager of the plants containing this plant.
        /// </param>
        public VPlant(PlantTickData plantTickData, VPlantManager vPlantManager)
        {
            _id = plantTickData.Id;
            var(x, y) = plantTickData.Position.Value;
            _position = new Vector2(x, y);
            _partsNumber = plantTickData.PartsNumber;
            if (!_fruitsPositions.ContainsKey(_id))
            {
                _fruitsPositions.Add(_id, PickFruitsPosition());
            }
            InitializePlantStaticBody();
        }

        /// <summary>
        /// Updates the plant with the new tick data.
        /// </summary>
        /// <param name="plantTickData">
        /// The tick data of the plant.
        /// </param>
        public void Update(PlantTickData plantTickData)
        {
            if (plantTickData.PartsNumber == 0) Delete();
            else
            {
                var oldPartsNumber = _partsNumber;
                _partsNumber = plantTickData.PartsNumber;
                if (oldPartsNumber > _partsNumber) _plantStaticBody.SubstractFruit();
                else _plantStaticBody.AddFruit();
            }
        }

        /// <summary>
        /// Deletes the plant.
        /// </summary>
        public void Delete()
        {
            _plantStaticBody.QueueFree();
            Deleted.Invoke(this);
        }

        /// <summary>
        /// Initializes the plant static body.
        /// </summary>
        private void InitializePlantStaticBody()
        {
            _plantStaticBody = Prefabs.Plant.Instantiate() as PlantStaticBody;
            _plantStaticBody.Initialize(_partsNumber - 1, _fruitsPositions[_id]);
            _plantStaticBody.Position = new Vector3(_position.X, 0, _position.Y);
        }

        /// <summary>
        /// Picks the random fruits positions for a plant.
        /// </summary>
        /// <returns>
        /// The fruits positions.
        /// </returns>
        private Vector3[] PickFruitsPosition()
        {
            var result = new Vector3[3];
            for (int i = 0; i < result.Length; i++)
            {
                var sign = _randGen.NextBoolean();
                var firstCoord = (_randGen.NextSingle() * 0.11f + 0.11f) * (sign ? 1 : -1);
                var secondCoord = _randGen.NextSingle() * 0.44f - 0.22f;
                var heightCoord = _randGen.NextSingle() * 0.11f - 0.07f;
                if (_randGen.NextBoolean())
                {
                    result[i] = new Vector3(firstCoord, heightCoord, secondCoord);
                }
                else
                {
                    result[i] = new Vector3(secondCoord, heightCoord, firstCoord);
                }
            }
            return result;
        }
    }
}
