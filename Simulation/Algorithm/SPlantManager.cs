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
        private List<Cluster> _clusters = new List<Cluster>();

        public List<Cluster> Clusters => _clusters;

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
            GenerateClusters(clustersDensity, clusterSize, clusterDensity);
        }

        public void ProcessPlants()
        {
            _clusters.ForEach(cluster => cluster.ProcessPlants());
        }

        public void UpdatePlants()
        {
            _clusters.ForEach(cluster => cluster.UpdatePlants());
        }

        public void DeleteDeadPlants()
        {
            _clusters.ForEach(cluster => cluster.DeleteDeadPlants());
        }

        public List<(float distance, SPlant plant)> GetPlantsInRange(Vector2 centerPoint, float range)
        {
            var result = new List<(float distance, SPlant plant)>();
            foreach (var cluster in _clusters)
            {
                result.AddRange(cluster.GetPlantsInRange(centerPoint, range));
            }
            return result;
        }

        public (int plantsNumber, PlantDTO[] plantsDTOs) GetSavingData()
        {
            var result = new List<PlantDTO>();
            foreach (var cluster in _clusters)
            {
                result.AddRange(cluster.GetSavingData());
            }
            return (result.Count, result.ToArray());
        }

        private void GenerateClusters(float clustersDensity, float clusterSize, float clusterDensity)
        {
            // the inverse of clusters density is a side of a square on which,
            // on average, appear one cluster.
            var clustersMinDist = 1 / clustersDensity * 0.1;

            var (mapSizeX, mapSizeY) = _controller.Map.Size;
            var mapArea = mapSizeX * mapSizeY;
            var clustersNumber = (int)Math.Round(mapArea * clustersDensity);

            for (int i = 0; i < clustersNumber; i++)
            {
                bool badPosition = false;
                while (true)
                {
                    var positionProposition = new Vector2(
                    _randGen.NextSingle() * mapSizeX - mapSizeX / 2f,
                    _randGen.NextSingle() * mapSizeY - mapSizeY / 2f
                    );

                    foreach (var cluster in _clusters)
                    {
                        var clusterCenter = cluster.Position;
                        if (clusterCenter != Vector2.Zero)
                        {
                            if ((clusterCenter - positionProposition).Length() < clustersMinDist)
                            {
                                badPosition = true;
                                break;
                            }
                        }
                    }
                    if (!badPosition)
                    {
                        _clusters.Add(new Cluster(positionProposition, clusterSize, clusterDensity, this, _controller));
                        break;
                    }
                }
            }
        }
    }
}