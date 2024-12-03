using ProjectEvolution.Utility.BinarySerialization;
using System;
using System.Collections.Generic;

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
            var (_, plantsData) = BinReader.GetPlantTickData();
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

        public void LoadOnTick(int tickNumber)
        {
            BinReader.CurrentTickNumber = tickNumber;
            var(plantsNumber, _) = BinReader.GetPlantTickData();
            var plants = new List<PlantTickData>();

            var findedAll = false;
            while (true)
            {
                var (_, plantsData) = BinReader.GetPlantTickData();
                foreach (var plantData in plantsData)
                {
                    var existingPlant = plants.Find(x => x.Id == plantData.Id);
                    if (existingPlant != null)
                    {
                        if (plantData.Position.HasValue)
                        {
                            existingPlant = existingPlant with { Position = plantData.Position };
                        }
                    }
                    else if (plants.Count != plantsNumber && plantData.PartsNumber != 0)
                    {
                        plants.Add(plantData);
                    }

                    if (plants.Count == plantsNumber)
                    {
                        var plant = plants.Find(x => x.Position == null);
                        if (plant == null)
                        {
                            findedAll = true;
                            break;
                        }
                    }
                }
                if (findedAll) break;
                else BinReader.CurrentTickNumber--;
            }
            foreach (var plantData in plants)
            {
                AddNewPlant(plantData);
            }
        }

        private void InitializePlants()
        {
            var (_, plantsData) = BinReader.GetPlantTickData();
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
            _visualization.CallDeferred("add_child", plant.PlantSceneObject);
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
