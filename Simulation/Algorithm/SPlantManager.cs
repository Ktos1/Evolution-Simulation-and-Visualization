using Godot;
using ProjectEvolution.Utility.BinarySerialization;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ProjectEvolution.Simulation.Algorithm
{
    internal class SPlantManager
    {
        private SimulationController _controller;
        private Random _randGen = new Random();
        private List<Cluster> _clusters = new List<Cluster>();
        private float _clustersMinDist;

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
            SimulationController controller,
            CancellationToken cancelToken)
        {
            _controller = controller;
            GenerateClusters(clustersDensity, clusterSize, clusterDensity, cancelToken);
        }

        public void ProcessPlants(CancellationToken cancelToken)
        {
            _clusters.ForEach(cluster => cluster.ProcessPlants(cancelToken));
        }

        public void UpdatePlants(CancellationToken cancelToken)
        {
            _clusters.ForEach(cluster => cluster.UpdatePlants(cancelToken));
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
            var totalPlantsNumber = 0;
            var totalPlantsData = new List<PlantDTO>();
            foreach (var cluster in _clusters)
            {
                (int plantsNumber, PlantDTO[] plantsData) = cluster.GetSavingData();
                totalPlantsNumber += plantsNumber;
                totalPlantsData.AddRange(plantsData);
            }
            return (totalPlantsNumber, totalPlantsData.ToArray());
        }

        public Vector2 GetNewClusterPosition(CancellationToken? cancelToken = null)
        {
            var (mapSizeX, mapSizeY) = _controller.Map.Size;
            bool badPosition = false;
            while (true)
            {
                var positionProposition = new Vector2(
                _randGen.NextSingle() * mapSizeX - mapSizeX / 2f,
                _randGen.NextSingle() * mapSizeY - mapSizeY / 2f
                );

                foreach (var cluster in _clusters)
                {
                    if (cluster.IsEmpty) continue;
                    var clusterCenter = cluster.Position;
                    if ((clusterCenter - positionProposition).Length() < _clustersMinDist)
                    {
                        badPosition = true;
                        break;
                    }
                }
                if (!badPosition)
                {
                    return positionProposition;
                }
                else badPosition = false;
                cancelToken?.ThrowIfCancellationRequested();
            }
        }

        private void GenerateClusters(
            float clustersDensity, 
            float clusterSize, 
            float clusterDensity, 
            CancellationToken cancelToken)
        {
            // the inverse of clusters density is a side of a square on which,
            // on average, appear one cluster.
            var idealClustersMinDist = 1 / clustersDensity;
            _clustersMinDist = idealClustersMinDist * 0.75f;

            var (mapSizeX, mapSizeY) = _controller.Map.Size;
            var mapArea = mapSizeX * mapSizeY;
            var clustersNumber = Mathf.FloorToInt(mapArea / Mathf.Pow(idealClustersMinDist, 2));

            for (int i = 0; i < clustersNumber; i++)
            {
                var position = GetNewClusterPosition(cancelToken);
                _clusters.Add(new Cluster(position, clusterSize, clusterDensity, this, _controller));
            }
        }
    }
}