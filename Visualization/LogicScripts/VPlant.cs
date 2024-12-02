using Godot;
using System;

namespace ProjectEvolution.Visualization.LogicScripts
{
    internal class VPlant
    {
        private uint _id;
        private Vector2 _position;
        private int _partsNumber;

        private PlantSceneObject _plantSceneObject;

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
            _plantSceneObject.Initialize(_partsNumber - 1);
            _plantSceneObject.Position = new Vector3(_position.X, 0, _position.Y);
        }
    }
}
