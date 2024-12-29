using Godot;
using ProjectEvolution.Utility.BinarySerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ProjectEvolution.Simulation
{
    /// <summary>
    /// Represents a cluster of plants.
    /// </summary>
    class Cluster : MapObject
    {
        /// <summary>
        /// The random number generator shared by all instances of the 
        /// <see cref="Cluster"/> class.
        /// </summary>
        private static Random _randGen = new Random();
        /// <summary>
        /// The plant manager containing the cluster.
        /// </summary>
        private SPlantsManager _sPlantManager;
        /// <summary>
        /// The simulation controller.
        /// </summary>
        private SimulationController _controller;
        /// <summary>
        /// The list of plants in the cluster.
        /// </summary>
        private List<SPlant> _plants = new List<SPlant>();
        /// <summary>
        /// The maximum plants capacity of the cluster.
        /// </summary>
        private int _maxCapacity;
        /// <summary>
        /// The list of plants that are going to be added to the cluster on the end of
        /// updating plants in the cluster.
        /// </summary>
        private List<SPlant> _propagatedPlants = new List<SPlant>();
        /// <summary>
        /// The list of plants that are going to be deleted from the cluster on the 
        /// end of the processing current tick.
        /// </summary>
        private List<SPlant> _deadPlants = new List<SPlant>();

        /// <summary>
        /// Indicates whether the cluster is empty.
        /// </summary>
        private bool _isEmpty = false;
        /// <summary>
        /// Used to count the time to respawn the cluster.
        /// </summary>
        private int _timeToRespawn;

        /// <summary>
        /// Occurs when the cluster is going to be deleted on the end of a tick.
        /// </summary>
        public event DeletionEventHandler Deletion;

        /// <summary>
        /// Gets a value indicating whether the cluster is empty.
        /// </summary>
        public bool IsEmpty => _isEmpty;

        /// <summary>
        /// Gets or sets a value indicating whether the cluster is respawnable.
        /// </summary>
        public bool IsRespawnable { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cluster"/> class.
        /// </summary>
        /// <param name="position">
        /// The position of the cluster on the map during initial spawn.
        /// </param>
        /// <param name="clusterSize">
        /// The diameter of the cluster at initial spawn.
        /// </param>
        /// <param name="clusterDensity">
        /// The density of the cluster at initial spawn.
        /// </param>
        /// <param name="sPlantManager">
        /// The plant manager containing the cluster.
        /// </param>
        /// <param name="controller">
        /// The simulation controller.
        /// </param>
        public Cluster(
            Vector2 position,
            float clusterSize,
            float clusterDensity,
            SPlantsManager sPlantManager,
            SimulationController controller)
        {
            _position = position;
            _sPlantManager = sPlantManager;
            _controller = controller;
            GeneratePlants(clusterSize, clusterDensity);
        }

        /// <summary>
        /// Processes logic of the cluster for one tick and saves the result in
        /// "new" variables.
        /// </summary>
        /// <param name="cancelToken">
        /// Used to safely abort the simulation which is being processed on separate thread.
        /// </param>
        public void Process(CancellationToken cancelToken)
        {
            if (!_isEmpty)
                _plants.ForEach(plant => plant.Process(cancelToken));
            else if (_timeToRespawn != 0)
                _timeToRespawn--;
            else
                Respawn();

            cancelToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Apply the changes made in process method.
        /// </summary>
        /// <param name="cancelToken">
        /// Used to safely abort the simulation which is being processed on separate thread.
        /// </param>
        public void UpdatePlants(CancellationToken? cancelToken = null)
        {
            if (IsEmpty) return;

            foreach (var plant in _plants)
            {
                plant.Update();
            }
            _propagatedPlants.ForEach(plant => _plants.Add(plant));
            _propagatedPlants.Clear();
            cancelToken?.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Deletes the dead plants from the cluster.
        /// </summary>
        public void DeleteDeadPlants()
        {
            if (_isEmpty) return;

            _deadPlants.ForEach(plant => _plants.Remove(plant));
            if (!_plants.Any())
            {
                if (IsRespawnable)
                {
                    _isEmpty = true;
                    _timeToRespawn = SimulationSettings.TimeToClusterRespawn;
                }
                else
                {
                    Deletion.Invoke(this);
                    return;
                }
            }
            _deadPlants.Clear();
        }

        /// <summary>
        /// Gets the plants in the cluster that are in the specified range from the center point.
        /// </summary>
        /// <param name="centerPoint">
        /// The point from which the distance to the plants is calculated.
        /// </param>
        /// <param name="range">
        /// The range from the center point in which the plants are being returned.
        /// </param>
        /// <returns>
        /// A list of tuples containing the distance from the center point and the plant.
        /// </returns>
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

        /// <summary>
        /// Gets the data of the cluster to save it in the file.
        /// </summary>
        /// <returns>
        /// A tuple containing the number of plants in the cluster and the data of the plants.
        /// </returns>
        public (int plantsNumber, PlantDTO[] plantsData) GetSavingData()
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

        /// <summary>
        /// Tries to propagate a plant in the cluster.
        /// </summary>
        /// <remarks>
        /// The propagation could not be successful if the cluster is full 
        /// or there is not enough space.
        /// </remarks>
        /// <param name="parent">
        /// The plant from which propagation is attempted.
        /// </param>
        internal void TryPropagatePlant(SPlant parent)
        {
            if (_plants.Count <= _maxCapacity)
                TrySpawnPlant(parent.Position, 1, 0.5f, 15, 1, true);
        }

        /// <summary>
        /// Generates plants in the cluster at initial spawn.
        /// </summary>
        /// <param name="clusterSize">
        /// The diameter of the cluster.
        /// </param>
        /// <param name="clusterDensity">
        /// The density of the cluster.
        /// </param>
        private void GeneratePlants(float clusterSize, float clusterDensity)
        {
            var clusterRadius = clusterSize / 2;
            var clusterArea = Math.PI * Math.Pow(clusterRadius, 2);
            var plantsPerCluster = (int)Math.Round(clusterArea * clusterDensity);
            _maxCapacity = plantsPerCluster * 2;

            var plantsMinDist = 1 / clusterDensity * Math.PI * 0.25;
            for (var i = 0; i < plantsPerCluster; i++)
            {
                TrySpawnPlant(Position, clusterRadius, (float)plantsMinDist, 10, 4, false);
            }
            // this must be here because the trySpawnPlant added new plant to the _propagatedPlants
            // and the UpdatePlants() add from the propagated to the _plants from which plants are
            // being saved (specially in the first tick).
            UpdatePlants();
        }

        /// <summary>
        /// Respawns the cluster.
        /// </summary>
        /// <remarks>
        /// Respawn happens in a new avaible position and with a number of plants defined 
        /// in <see cref="SimulationSettings.PlantsNumberInRespawnedCluster"/>.
        /// </remarks>
        private void Respawn()
        {
            _position = _sPlantManager.GetNewClusterPosition();
            for (int i = 0; i < SimulationSettings.PlantsNumberInRespawnedCluster; i++)
            {
                TrySpawnPlant(Position, 2, 0.5f, 5, 4, false);
            }
            _isEmpty = false;
        }

        /// <summary>
        /// Tries to spawn a plant in the cluster.
        /// </summary>
        /// <remarks>
        /// Draws a position in the area around the center point and checks if it
        /// is not too close to others plants. If the position is valid, a new plant
        /// is created and added to the cluster. Otherwise the method tries 
        /// again <paramref name="attemptsNumber"/> times.
        /// </remarks>
        /// <param name="centerPoint">
        /// The center point of the area in which the plant is being spawned.
        /// </param>
        /// <param name="areaRadius">
        /// The radius of the area in which the plant is being spawned.
        /// </param>
        /// <param name="minPlantsDist">
        /// The minimum distance between the other plants.
        /// </param>
        /// <param name="attemptsNumber">
        /// The number of attempts to draw a position before giving up.
        /// </param>
        /// <param name="partsNumber">
        /// The number of parts of the new spawned plant.
        /// </param>
        /// <param name="ignoreBeyondBorders">
        /// Specifies whether a plant spawn should be abandoned if in first attempt
        /// a drawn position was beyond the map borders.
        /// </param>
        private void TrySpawnPlant(
            Vector2 centerPoint,
            float areaRadius,
            float minPlantsDist,
            int attemptsNumber,
            int partsNumber,
            bool ignoreBeyondBorders)
        {
            var firstShot = true;
            while (attemptsNumber > 0)
            {
                var XShot = _randGen.NextSingle() * areaRadius * 2 - areaRadius + centerPoint.X;
                var YShot = _randGen.NextSingle() * areaRadius * 2 - areaRadius + centerPoint.Y;
                var xLimit = _controller.Map.Limitations.x;
                var yLimit = _controller.Map.Limitations.y;

                var ShotsVector = new Vector2(XShot, YShot);
                if ((ShotsVector - centerPoint).Length() <= areaRadius)
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
                    var withPropagated = true;
                    foreach (var cluster in _sPlantManager.Clusters)
                    {
                        var plants = cluster._plants;
                        if (withPropagated)
                        {
                            plants = plants.Concat(_propagatedPlants).ToList();
                            withPropagated = false;
                        }

                        foreach (var plant in plants)
                        {
                            if ((ShotsVector - plant.Position).Length() < minPlantsDist)
                            {
                                tooClose = true;
                                attemptsNumber--;
                                break;
                            }
                        }
                        if (tooClose) break;
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

        /// <summary>
        /// Creates a new plant.
        /// </summary>
        /// <param name="position">
        /// The position of the plant on the map.
        /// </param>
        /// <param name="partsNumber">
        /// The number of parts of the plant.
        /// </param>
        /// <returns>
        /// The new created plant.
        /// </returns>
        private SPlant CreatePlant(Vector2 position, int partsNumber)
        {
            var plant = new SPlant(position, partsNumber, this);
            plant.Deleted += OnPlantDeleted;
            return plant;
        }

        /// <summary>
        /// Handles the deletion event of a plant.
        /// </summary>
        /// <param name="deletedPlant">
        /// The plant that has been deleted.
        /// </param>
        private void OnPlantDeleted(MapObject deletedPlant)
        {
            _deadPlants.Add(deletedPlant as SPlant);
        }
    }
}
