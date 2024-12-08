using Godot;
using ProjectEvolution.Utility.BinarySerialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectEvolution.Simulation.Algorithm
{
    internal class SPlantManager
    {
        private SimulationController _controller;
        private Random _randGen = new Random();
        private List<SPlant> _plants = new List<SPlant>();
        private List<SPlant> _propagatedPlants = new List<SPlant>();
        private List<SPlant> _deadPlants = new List<SPlant>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clustersDensity">
        /// Specifies the number of clusters per unit area.
        /// </param>
        /// <param name="clusterSize">
        /// Specifies the size of each cluster.
        /// </param>
        /// <param name="clusterDensity">
        /// Specifies the number of plants per unit area.
        /// </param>
        public SPlantManager
            (float clustersDensity, 
            float clusterSize, 
            float clusterDensity, 
            SimulationController controller)
        {
            _controller = controller;
            GeneratePlants(clustersDensity, clusterSize, clusterDensity);
        }

        public void ProcessPlants()
        {
            foreach (var plant in _plants)
            {
                plant.Process();
            }
        }

        public void UpdatePlants()
        {
            foreach (var plant in _plants)
            {
                plant.Update();
            }
            _propagatedPlants.ForEach(plant => _plants.Add(plant));
            _propagatedPlants.Clear();
        }

        public void DeleteDeadPlants()
        {
            _deadPlants.ForEach(plant => _plants.Remove(plant));
            _deadPlants.Clear();
        }

        public List<(float distance, SPlant plant)> GetPlantsInRange(Vector2 centerPoint, float range)
        {
            var result = new List<(float distance, SPlant plant)>();
            foreach (var plant in _plants)
            {
                var distance = (centerPoint - plant.Position).Length();
                if (distance <= range)
                {
                    result.Add((distance, plant));
                }
            }
            return result;
        }

        public (int plantsNumber, PlantDTO[] plantsDTOs) GetSavingData()
        {
            var result = new List<PlantDTO>();
            foreach (var plant in _plants)
            {
                var plantInfo = plant.GetSavingData();
                if (plantInfo != null)
                {
                    result.Add(plantInfo);
                }
            }
            return (_plants.Count, result.ToArray());
        }

        internal void TryPropagatePlant(SPlant sPlant)
        {
            TrySpawnPlant(sPlant.Position, 1, 0.5f, 15, 1, true);
        }

        private void GeneratePlants(
            float clustersDensity,
            float clusterSize,
            float clusterDensity)
        {
            var clusterCenters = GenerateClustersCenter(clustersDensity);

            var clusterRadius = clusterSize / 2;
            var clusterArea = Math.PI * Math.Pow(clusterRadius, 2);
            var plantsPerCluster = (int)Math.Round(clusterArea * clusterDensity);

            var plantsMinDist = 1 / clusterDensity * Math.PI * 0.25;
            foreach (var clusterCenter in clusterCenters)
            {
                for (var i = 0; i < plantsPerCluster; i++)
                {
                    TrySpawnPlant(clusterCenter, clusterRadius, (float)plantsMinDist, 10, 4, false);
                }
            }
        }

        private void TrySpawnPlant(
            Vector2 refPoint,
            float areaRadius,
            float minPlantsDist,
            int attemptsNumber,
            int partsNumber,
            bool ignoreBeyondBorders)
        {
            var firstShot = true;
            while (attemptsNumber > 0)
            {
                var XShot = _randGen.NextSingle() * areaRadius * 2 - areaRadius + refPoint.X;
                var YShot = _randGen.NextSingle() * areaRadius * 2 - areaRadius + refPoint.Y;
                var xLimit = _controller.Map.Size.x / 2f;
                var yLimit = _controller.Map.Size.y / 2f;

                var ShotsVector = new Vector2(XShot, YShot);
                if ((ShotsVector - refPoint).Length() <= areaRadius)
                {
                    if (XShot > xLimit || XShot < -xLimit || YShot > yLimit || YShot < -yLimit)
                    {
                        if (ignoreBeyondBorders) continue;
                        else
                        {
                            if (firstShot) break;
                            else continue;
                        }
                    }
                    var tooClose = false;
                    foreach (var plant in _plants.Concat(_propagatedPlants))
                    {
                        if ((ShotsVector - plant.Position).Length() < minPlantsDist)
                        {
                            tooClose = true;
                            attemptsNumber--;
                            break;
                        }
                    }
                    if (!tooClose)
                    {
                        _propagatedPlants.Add(CreatePlant(ShotsVector, partsNumber));
                        break;
                    }
                    firstShot = false;
                }
            }
        }

        private Vector2[] GenerateClustersCenter(float clustersDensity)
        {
            // the inverse of clusters density is a side of a square on which,
            // on average, appear one cluster.
            var clustersMinDist = 1 / clustersDensity * 0.143;

            var (mapSizeX, mapSizeY) = _controller.Map.Size;
            var mapArea = mapSizeX * mapSizeY;
            var clustersNumber = (int)Math.Round(mapArea * clustersDensity);
            var clustersCenters = new Vector2[clustersNumber];

            for (int i = 0; i < clustersNumber; i++)
            {
                bool goodPosition = false;
                while (true)
                {
                    var positionProposition = new Vector2(
                    _randGen.NextSingle() * mapSizeX - mapSizeX / 2f,
                    _randGen.NextSingle() * mapSizeY - mapSizeY / 2f
                    );
                    foreach (var clusterCenter in clustersCenters)
                    {
                        if (clusterCenter != Vector2.Zero)
                        {
                            if ((clusterCenter - positionProposition).Length() < clustersMinDist)
                            {
                                break;
                            }
                        }
                        else
                        {
                            goodPosition = true;
                            break;
                        }
                    }
                    if (goodPosition)
                    {
                        clustersCenters[i] = positionProposition;
                        break;
                    }
                }
            }
            return clustersCenters;
        }

        private SPlant CreatePlant(Vector2 position, int partsNumber)
        {
            var plant = new SPlant(position, partsNumber, this);
            plant.Deleted += OnPlantDeleted;
            return plant;
        }

        private void OnPlantDeleted(MapObject deletedPlant)
        {
            _deadPlants.Add(deletedPlant as SPlant);
        }
    }
}