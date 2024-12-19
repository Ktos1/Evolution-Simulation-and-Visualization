using Godot;
using ProjectEvolution.Utility.BinarySerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ProjectEvolution.Simulation.Algorithm
{
    class Cluster
    {
        private static Random _randGen = new Random();
        private SPlantManager _sPlantManager;
        private SimulationController _controller;
        private List<SPlant> _plants = new List<SPlant>();
        private int _maxCapacity;
        private List<SPlant> _propagatedPlants = new List<SPlant>();
        private List<SPlant> _deadPlants = new List<SPlant>();

        private bool _isEmpty = false;
        private int _timeToRespawn;

        public delegate void DeletionEventHandler(Cluster clusterToDelete);
        public event DeletionEventHandler Deletion;

        public Vector2 Position { get; private set; }
        public bool IsEmpty => _isEmpty;

        public bool IsRespawnable { get; set; }

        public Cluster(
            Vector2 position, 
            float clusterSize, 
            float clusterDensity, 
            SPlantManager sPlantManager,
            SimulationController controller)
        {
            Position = position;
            _sPlantManager = sPlantManager;
            _controller = controller;
            GeneratePlants(clusterSize, clusterDensity);
        }

        public void ProcessPlants(CancellationToken cancelToken)
        {
            if (!_isEmpty)
                _plants.ForEach(plant => plant.Process(cancelToken));
            else if (_timeToRespawn != 0)
                _timeToRespawn--;
            else
                Respawn();

            cancelToken.ThrowIfCancellationRequested();
        }

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

        internal void TryPropagatePlant(SPlant sPlant)
        {
            if (_plants.Count <= _maxCapacity)
                TrySpawnPlant(sPlant.Position, 1, 0.5f, 15, 1, true);
        }

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

        private void Respawn()
        {
            Position = _sPlantManager.GetNewClusterPosition();
            for (int i = 0; i < SimulationSettings.PlantsNumberInRespawnedCluster; i++)
            {
                TrySpawnPlant(Position, 2, 0.5f, 5, 4, false);
            }
            _isEmpty = false;
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
                var xLimit = _controller.Map.Limitations.x;
                var yLimit = _controller.Map.Limitations.y;

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
