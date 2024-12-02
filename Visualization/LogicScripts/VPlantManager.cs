using ProjectEvolution.Utility.BinarySerialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectEvolution.Visualization.LogicScripts
{
    internal class VPlantManager
    {
        private Visualization _visualization;
        private List<VPlant> _plants = new List<VPlant>();
        private List<VPlant> _deadPlants = new List<VPlant>();

        public VPlantManager(Visualization visualization)
        {
            _visualization = visualization;
            InitializePlants();
        }

        public void UpdatePlants()
        {
            var plantsData = BinReader.GetPlantTickData();
            foreach (var plantData in plantsData)
            {
                if (plantData.Position.HasValue)
                {
                    AddNewPlant(plantData);
                }
                else
                {
                    var plant = _plants.Find(plant => plant.ID == plantData.Id);
                    plant.Update(plantData);
                }
            }
            DeleteDeadPlants();
        }

        public void Clear()
        {
            _plants.ForEach(plant => plant.Delete());
            _plants.Clear();
            _deadPlants.Clear();
        }

        private void InitializePlants()
        {
            var plantsData = BinReader.GetPlantTickData();
            foreach (var plantData in plantsData)
            {
                AddNewPlant(plantData);
            }
        }

        private void AddNewPlant(PlantTickData plantTickData)
        {
            var plant = new VPlant(plantTickData, this);
            plant.Deleted += OnPlantDelete;
            _plants.Add(plant);
            _visualization.CallDeferred("add_child", _plants.Last().StaticBody);
        }

        private void DeleteDeadPlants()
        {
            foreach (var deadPlant in _deadPlants)
            {
                _plants.Remove(deadPlant);
            }
            _deadPlants.Clear();
        }

        private void OnPlantDelete(object plant, EventArgs e)
        {
            _deadPlants.Add(plant as  VPlant);
        }
    }
}
