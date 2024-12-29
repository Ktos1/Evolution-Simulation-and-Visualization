using Godot;
using ProjectEvolution.Utility.BinarySerialization;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ProjectEvolution.Simulation
{
    /// <summary>
    /// Represents the plants manager.
    /// </summary>
    internal class SPlantsManager
    {
        /// <summary>
        /// The simulation controller.
        /// </summary>
        private SimulationController _controller;
        /// <summary>
        /// The random generator.
        /// </summary>
        private Random _randGen = new Random();
        /// <summary>
        /// The list of clusters.
        /// </summary>
        private List<Cluster> _clusters = new List<Cluster>();
        /// <summary>
        /// The list of clusters that will be deleted from the main 
        /// <see cref="_clusters"/> list at the end of a tick.
        /// </summary>
        private List<Cluster> _clustersToDelete = new List<Cluster>();
        /// <summary>
        /// The minimum distance between clusters.
        /// </summary>
        private float _clustersMinDist;

        /// <summary>
        /// Gets the list of clusters.
        /// </summary>
        public List<Cluster> Clusters => _clusters;

        /// <summary>
        /// Initializes a new instance of the <see cref="SPlantsManager"/> class.
        /// </summary>
        /// <param name="clustersDensity">
        /// Specifies the number of clusters per unit area at the start of the simulation.
        /// </param>
        /// <param name="clusterSize">
        /// Specifies the size of each cluster at the start of the simulation.
        /// </param>
        /// <param name="clusterDensity">
        /// Specifies the number of plants per unit area in each cluster at the start 
        /// of the simulation.
        /// </param>
        /// <param name="controller">
        /// The simulation controller.
        /// </param>
        /// <param name="cancelToken">
        /// Used to safely abort the simulation which is being processed on separate thread.
        /// </param>
        public SPlantsManager
            (float clustersDensity,
            float clusterSize,
            float clusterDensity,
            SimulationController controller,
            CancellationToken cancelToken)
        {
            _controller = controller;
            GenerateClusters(clustersDensity, clusterSize, clusterDensity, cancelToken);
        }

        /// <summary>
        /// Processes the plants logic for one tick.
        /// </summary>
        /// <param name="cancelToken">
        /// Used to safely abort the simulation which is being processed on separate thread.
        /// </param>
        public void ProcessPlants(CancellationToken cancelToken)
        {
            _clusters.ForEach(cluster => cluster.Process(cancelToken));
        }

        /// <summary>
        /// Updates the plants after the current tick is processed.
        /// </summary>
        /// <param name="cancelToken">
        /// Used to safely abort the simulation which is being processed on separate thread.
        /// </param>
        public void UpdatePlants(CancellationToken cancelToken)
        {
            _clusters.ForEach(cluster => cluster.UpdatePlants(cancelToken));
        }

        /// <summary>
        /// Deletes the dead plants and clusters marked for deletion.
        /// </summary>
        public void DeleteDeadPlants()
        {
            _clusters.ForEach(cluster => cluster.DeleteDeadPlants());
            _clustersToDelete.ForEach(cluster => _clusters.Remove(cluster));
            _clustersToDelete.Clear();
        }

        /// <summary>
        /// Gets the plants in the specified range from the specific point.
        /// </summary>
        /// <param name="centerPoint">
        /// The point from which the range is calculated.
        /// </param>
        /// <param name="range">
        /// The range of the point.
        /// </param>
        /// <returns>
        /// The list of tuples containing a distance between the center point and a plant
        /// and the plant itself.
        /// </returns>
        public List<(float distance, SPlant plant)> GetPlantsInRange(Vector2 centerPoint, float range)
        {
            var result = new List<(float distance, SPlant plant)>();
            foreach (var cluster in _clusters)
            {
                result.AddRange(cluster.GetPlantsInRange(centerPoint, range));
            }
            return result;
        }

        /// <summary>
        /// Gets the data of the plants to save.
        /// </summary>
        /// <returns>
        /// A tuple containing the number of plants and an array of plants data.
        /// </returns>
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

        /// <summary>
        /// Gets a new cluster position.
        /// </summary>
        /// <remarks>
        /// The method generates a new cluster position which is not closer than
        /// <see cref="_clustersMinDist"/> to any other cluster.
        /// </remarks>
        /// <param name="cancelToken">
        /// Used to safely abort the simulation which is being processed on separate thread.
        /// </param>
        /// <returns>
        /// The new cluster position.
        /// </returns>
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

        /// <summary>
        /// Generates the clusters of plants on the start of the simulation.
        /// </summary>
        /// <param name="clustersDensity">
        /// The density of the clusters of plants on the start of the simulation.
        /// </param>
        /// <param name="clusterSize">
        /// The size of each cluster at the start of the simulation.
        /// </param>
        /// <param name="clusterDensity">
        /// The density of the plants in the clusters on the start of the simulation.
        /// </param>
        /// <param name="cancelToken">
        /// Used to safely abort the simulation which is being processed on separate thread.
        /// </param>
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

            var respawnablePlantsSum = 0;
            var respawnable = true;
            for (int i = 0; i < clustersNumber; i++)
            {
                var position = GetNewClusterPosition(cancelToken);
                if (respawnable)
                {
                    if (respawnablePlantsSum > SimulationSettings.MaxPlantsNumberToRespawn)
                        respawnable = false;
                    else
                        respawnablePlantsSum += SimulationSettings.PlantsNumberInRespawnedCluster;
                }

                var newCluster = new Cluster(position, clusterSize, clusterDensity, this, _controller)
                { IsRespawnable = respawnable };
                newCluster.Deletion += OnClusterDeletion;
                _clusters.Add(newCluster);
            }
        }

        /// <summary>
        /// Invoked when a cluster is deleted.
        /// </summary>
        /// <param name="clusterToDelete">
        /// The cluster to delete.
        /// </param>
        private void OnClusterDeletion(MapObject clusterToDelete)
        {
            _clustersToDelete.Add(clusterToDelete as Cluster);
        }
    }
}