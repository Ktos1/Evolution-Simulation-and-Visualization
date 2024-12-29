using ProjectEvolution.CommonStuff;
using ProjectEvolution.Utility.BinarySerialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

/// <summary>
/// Contains the classes used in the simulation part of the project.
/// </summary>
namespace ProjectEvolution.Simulation
{
    /// <summary>
    /// Represents the simulation controller - the main class responsible for
    /// managing the simulation process.
    /// </summary>
    public class SimulationController
    {
        /// <summary>
        /// The map of the simulation.
        /// </summary>
        public readonly Map Map;
        /// <summary>
        /// The creatures manager.
        /// </summary>
        private SCreaturesManager _creaturesManager;
        /// <summary>
        /// The plants manager.
        /// </summary>
        private SPlantsManager _plantManager;

        /// <summary>
        /// The binary writer used to save the simulation data.
        /// </summary>
        private BinWriter _binWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulationController"/> class.
        /// </summary>
        /// <param name="map">
        /// The map of the simulation.
        /// </param>
        /// <param name="creaturesNum">
        /// The number of creatures on the start of the simulation.
        /// </param>
        /// <param name="clustersDensity">
        /// The density of the clusters of plants on the start of the simulation.
        /// </param>
        /// <param name="clusterSize">
        /// The size of the clusters of plants on the start of the simulation.
        /// </param>
        /// <param name="clusterDensity">
        /// The density of the plants in the clusters on the start of the simulation.
        /// </param>
        /// <param name="cancelToken">
        /// Used to safely abort the simulation which is being processed on separate thread.
        /// </param>
        public SimulationController(
            float clustersDensity,
            float clusterSize,
            float clusterDensity,
            CancellationToken cancelToken
            )
        {
            Map = SimulationSettings.Map;
            ResetMapObjectIds();
            _creaturesManager = new SCreaturesManager(this, cancelToken);
            _plantManager = new SPlantsManager(clustersDensity, clusterSize, clusterDensity, this, cancelToken);
        }

        /// <summary>
        /// Starts the simulation process.
        /// </summary>
        /// <param name="cancelToken">
        /// Used to safely abort the simulation which is being processed on separate thread.
        /// </param>
        /// <param name="currentTick">
        /// Variable given by reference which is used to indicate the current tick of 
        /// the simulation.
        /// </param>
        public void StartSimulation(CancellationToken cancelToken, ref int currentTick)
        {
            int totalTicksNumber = SimulationSettings.SimulDurationInYears * CommonSettings.YEAR_DURATION;
            _binWriter = new BinWriter(totalTicksNumber + 1, new SimulationInfoDTO(Map.Size));
            SaveTickData();
            for (int i = 0; i < totalTicksNumber; i++)
            {
                _creaturesManager.ProcessCreatures(cancelToken);
                _plantManager.ProcessPlants(cancelToken);
                _creaturesManager.UpdateCreatures(cancelToken);
                _plantManager.UpdatePlants(cancelToken);

                SaveTickData();
                _plantManager.DeleteDeadPlants();

                currentTick = i;
            }
            _binWriter.SaveToFile(cancelToken);
        }

        /// <summary>
        /// Gets the T-type map objects in the range of the <paramref name="seeker"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the object sought.
        /// </typeparam>
        /// <param name="seeker">
        /// The creature which is seeking the objects.
        /// </param>
        /// <param name="range">
        /// The range in which the objects are sought.
        /// </param>
        /// <returns>
        /// An array of T-type objects sorted ascending by the distance to 
        /// the <paramref name="seeker"/>
        /// </returns>
        public T[] ObjectsInRange<T>(SCreature seeker, float range) where T : MapObject
        {
            var objectsInRange = new List<(float distance, T _object)>();

            float bestDistance = range + 1;

            if (typeof(T) == typeof(SCreature))
            {
                objectsInRange = _creaturesManager.GetCreaturesInRange(seeker.Position, range)
                    as List<(float, T)>;
            }
            else if (typeof(T) == typeof(SPlant))
            {
                objectsInRange = _plantManager.GetPlantsInRange(seeker.Position, range)
                    as List<(float, T)>;
            }

            var sortedResult = objectsInRange.OrderBy((pair) => pair.distance).ToList();
            var resultArray = new T[sortedResult.Count()];

            for (int i = 0; i < resultArray.Length; i++)
            {
                resultArray[i] = sortedResult[i]._object;
            }

            return resultArray;
        }

        /// <summary>
        /// Submit the data of the current tick to the binary writer.
        /// </summary>
        private void SaveTickData()
        {
            var creaturesDTOs = _creaturesManager.GetSavingData();
            var (plantsNumber, plantsDTOs) = _plantManager.GetSavingData();
            _binWriter.AddTick(new TickDTO(plantsNumber, plantsDTOs, creaturesDTOs));
        }

        /// <summary>
        /// Resets the ids counters of the map objects to 0.
        /// </summary>
        private void ResetMapObjectIds()
        {
            SCreature.ResetIds();
            SPlant.ResetIds();
        }
    }
}
