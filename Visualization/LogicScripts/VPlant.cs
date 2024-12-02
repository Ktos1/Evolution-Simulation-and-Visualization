using Godot;
using System;

namespace ProjectEvolution.Visualization.LogicScripts
{
    internal class VPlant
    {
        private uint _id;
        private Vector2 _position;
        private int _partsNumber;

        private StaticBody3D _staticBody;

        private VPlantManager _plantManager;

        public event EventHandler Deleted;

        public uint ID => _id;
        public StaticBody3D StaticBody => _staticBody;

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
            _staticBody.QueueFree();
            Deleted.Invoke(this, null);
        }

        private void InitializeStaticBodyNode()
        {
            _staticBody = Prefabs.Creature.Instantiate() as StaticBody3D;
            _staticBody.Position = new Vector3(_position.X, 0.75f, _position.Y);
        }
    }
}
