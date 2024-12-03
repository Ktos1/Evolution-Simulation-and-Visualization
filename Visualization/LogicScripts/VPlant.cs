using Godot;
using Godot.Collections;
using MathNet.Numerics.Random;
using System;

namespace ProjectEvolution.Visualization.LogicScripts
{
    internal class VPlant
    {
        private uint _id;
        private Vector2 _position;
        private int _partsNumber;
        private static Random _randGen = new Random();

        private PlantSceneObject _plantSceneObject;
        private static Dictionary<uint, Vector3[]> fruitsPositions = 
            new Dictionary<uint, Vector3[]>();

        private VPlantManager _plantManager;

        public event EventHandler Deleted;

        public uint ID => _id;
        public PlantSceneObject PlantSceneObject => _plantSceneObject;

        public VPlant(PlantTickData plantTickData, VPlantManager vPlantManager)
        {
            _id = plantTickData.Id;
            var(x, y) = plantTickData.Position.Value;
            _position = new Vector2(x, y);
            _partsNumber = plantTickData.PartsNumber;
            if (!fruitsPositions.ContainsKey(_id))
            {
                fruitsPositions.Add(_id, PickFruitsPosition());
            }
            InitializeStaticBodyNode();
            _plantManager = vPlantManager;
        }

        public void Update(PlantTickData plantTickData)
        {
            _partsNumber = plantTickData.PartsNumber;
            if (_partsNumber == 0) Delete();
        }

        public void Delete()
        {
            _plantSceneObject.QueueFree();
            Deleted.Invoke(this, null);
        }

        private void InitializeStaticBodyNode()
        {
            _plantSceneObject = Prefabs.Plant.Instantiate() as PlantSceneObject;
            _plantSceneObject.Initialize(_partsNumber - 1, fruitsPositions[_id]);
            _plantSceneObject.Position = new Vector3(_position.X, 0, _position.Y);
        }

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
