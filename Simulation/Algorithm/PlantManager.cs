using Godot;
using ProjectEvolution.Utility.BinarySerialization;
using System;
using System.Collections.Generic;

namespace ProjectEvolution.Simulation.Algorithm
{
    internal class PlantManager
    {
        private SimulationController _controller;
        private Random _randGen = new Random();
        private List<Plant> _plants;

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
        /// Specifies the number of plant per unit area.
        /// </param>
        public PlantManager
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

        public PlantDTO[] GetSavingData()
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
            return result.ToArray();
        }

        private List<Plant> GeneratePlants(
            float clustersDensity,
            float clusterSize,
            float clusterDensity)
        {
            var (mapSizeX, mapSizeY) = _controller.Map.Size;
            var clusterCenters = GenerateClustersCenter(clustersDensity);

            var clusterArea = Math.PI * Math.Pow(clusterSize, 2);
            var plantsPerCluster = (int)Math.Round(clusterArea * clusterDensity);

            var plants = new List<Plant>();
            foreach (var clusterCenter in clusterCenters)
            {
                plants.Add(new Plant(clusterCenter, 3));
            }

            return plants;
        }

        private Vector2[] GenerateClustersCenter(float clustersDensity)
        {
            // cluster density is a side of a square on which, on average, appear one cluster.
            var clustersMinDist = clustersDensity * 0.7;

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