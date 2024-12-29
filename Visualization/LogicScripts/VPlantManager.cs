using ProjectEvolution.Utility.BinarySerialization;
using ProjectEvolution.Visualization.ScenesScripts;
using System.Collections.Generic;

namespace ProjectEvolution.Visualization.LogicScripts
{
    /// <summary>
    /// Manages the plants in the visualization part.
    /// </summary>
    internal class VPlantManager
    {
        /// <summary>
        /// The main node of the visualization scene.
        /// </summary>
        private VisualizationScene _visualization;
        /// <summary>
        /// The list of plants contained by this manager.
        /// </summary>
        private List<VPlant> _plants = new List<VPlant>();
        /// <summary>
        /// The list of plants that will be deleted from the main 
        /// <see cref="_plants"/> list at the end of a plants update.
        /// </summary>
        private List<VPlant> _deadPlants = new List<VPlant>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VPlantManager"/> class.
        /// </summary>
        /// <param name="visualization">
        /// The visualization node.
        /// </param>
        public VPlantManager(VisualizationScene visualization)
        {
            _visualization = visualization;
            var (_, plantsData) = BinReader.GetPlantsTickData();
            foreach (var plantData in plantsData)
            {
                AddNewPlant(plantData);
            }
        }

        /// <summary>
        /// Updates the plants.
        /// </summary>
        /// <remarks>
        /// This method updates the plants by loading the new plants tick data from 
        /// the binary reader, updating with this data the plants that are already
        /// in the list, and adding or deleting plants if necessary.
        /// </remarks>
        public void UpdatePlants()
        {
            var (_, plantsData) = BinReader.GetPlantsTickData();
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

        /// <summary>
        /// Clears all the lists of plants.
        /// </summary>
        public void Clear()
        {
            _plants.ForEach(plant => plant.Delete());
            _plants.Clear();
            _deadPlants.Clear();
        }

        /// <summary>
        /// Loads the plants on a specific tick.
        /// </summary>
        /// <param name="tickNumber">
        /// The tick number to load the plants on.
        /// </param>
        public void LoadOnTick(int tickNumber)
        {
            BinReader.CurrentTickNumber = tickNumber;
            var(plantsNumber, plantsDataTemp) = BinReader.GetPlantsTickData();
            foreach (var plant in plantsDataTemp)
                if (plant.PartsNumber == 0) plantsNumber--;

            var plants = new List<PlantTickData>();
            var plantsNotInTargetTickIds = new List<uint>();

            var findedAll = false;
            while (!findedAll)
            {
                var(_, plantsData) = BinReader.GetPlantsTickData();
                foreach (var plantData in plantsData)
                {
                    var existingPlant = plants.Find(x => x.Id == plantData.Id);
                    if (existingPlant != default)
                    {
                        if (plantData.Position.HasValue)
                        {
                            var index = plants.IndexOf(existingPlant);
                            plants[index] = existingPlant with { Position = plantData.Position };
                        }
                    }
                    else if (plantData.PartsNumber == 0)
                    {
                        plantsNotInTargetTickIds.Add(plantData.Id);
                    }
                    else if (plants.Count != plantsNumber)
                    {
                        var forbiddenPlant = false;
                        foreach (var forbbidenId in plantsNotInTargetTickIds)
                        {
                            if (plantData.Id == forbbidenId)
                            {
                                forbiddenPlant = true;
                                break;
                            }
                        }
                        if (!forbiddenPlant) plants.Add(plantData);
                    }

                    if (plants.Count == plantsNumber)
                    {
                        var plant = plants.Find(x => x.Position == null);
                        if (plant == default)
                        {
                            findedAll = true;
                            break;
                        }
                    }
                }
                BinReader.CurrentTickNumber--;
            }
            foreach (var plantData in plants)
            {
                AddNewPlant(plantData);
            }
        }

        /// <summary>
        /// Adds a new plant.
        /// </summary>
        /// <remarks>
        /// This method adds a new plant by initializing a new <see cref="VPlant"/> 
        /// with the given plant data and adding it to the <see cref="_plants"/> list.
        /// </remarks>
        /// <param name="plantTickData">
        /// The tick data of the plant to add.
        /// </param>
        private void AddNewPlant(PlantTickData plantTickData)
        {
            var plant = new VPlant(plantTickData, this);
            plant.Deleted += OnPlantDelete;
            _plants.Add(plant);
            _visualization.CallDeferred("add_child", plant.PlantStaticBody);
        }

        /// <summary>
        /// Deletes the plants that are in the <see cref="_deadPlants"/> list 
        /// from the <see cref="_plants"/> list.
        /// </summary>
        private void DeleteDeadPlants()
        {
            foreach (var deadPlant in _deadPlants)
            {
                _plants.Remove(deadPlant);
            }
            _deadPlants.Clear();
        }

        /// <summary>
        /// Handles the deletion of a plant.
        /// </summary>
        /// <param name="plant">
        /// The plant that is being deleted.
        /// </param>
        private void OnPlantDelete(VPlant plant)
        {
            _deadPlants.Add(plant);
        }
    }
}
