using Godot;
using ProjectEvolution.Utility.BinarySerialization;
using System;
using System.Collections.Generic;

namespace ProjectEvolution.Simulation.Algorithm
{
    internal class SPlantManager
    {
        private SimulationController _controller;
        private Random _randGen = new Random();
        private List<SPlant> _plants;

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
            _plants = GeneratePlants(clustersDensity, clusterSize, clusterDensity);
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

        private List<SPlant> GeneratePlants(
            float clustersDensity,
            float clusterSize,
            float clusterDensity)
        {
            var (mapSizeX, mapSizeY) = _controller.Map.Size;
            var clusterCenters = GenerateClustersCenter(clustersDensity);

            var clusterRadius = clusterSize / 2;
            var clusterArea = Math.PI * Math.Pow(clusterRadius, 2);
            var plantsPerCluster = (int)Math.Round(clusterArea * clusterDensity);

            var plants = new List<SPlant>();

            var plantsMinDist = 1 / clusterDensity * Math.PI * 0.25;
            foreach (var clusterCenter in clusterCenters)
            {
                for (var i = 0; i < plantsPerCluster; i++)
                {
                    var firstShot = true;
                    var shotsCounter = 0;
                    while (true)
                    {
                        var XShot = _randGen.NextSingle() * clusterSize - clusterRadius + clusterCenter.X;
                        var YShot = _randGen.NextSingle() * clusterSize - clusterRadius + clusterCenter.Y;
                        var xLimit = _controller.Map.Size.x / 2f;
                        var yLimit = _controller.Map.Size.y / 2f;
                        
                        var ShotsVector = new Vector2(XShot, YShot);
                        if ((ShotsVector - clusterCenter).Length() <= clusterRadius)
                        {
                            if(XShot > xLimit || XShot < -xLimit || YShot > yLimit || YShot < -yLimit)
                            {
                                if (firstShot) break;
                                else continue;
                            }
                            var tooClose = false;
                            foreach (var plant in plants)
                            {
                                if ((ShotsVector - plant.Position).Length() < plantsMinDist)
                                {
                                    tooClose = true;
                                    shotsCounter++;
                                    break;
                                }
                            }
                            if (!tooClose)
                            {
                                plants.Add(new SPlant(ShotsVector, 4));
                                break;
                            }
                            if (shotsCounter > 10) break;
                            firstShot = false;
                        }
                    }
                }
            }
            return plants;
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
    }
}