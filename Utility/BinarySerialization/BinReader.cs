using Godot;
using MessagePack;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Visualization.LogicScripts;
using System;
using System.IO;

/// <summary>
/// Contains the classes used in the binary serialization and in saving the simulation data.
/// </summary>
namespace ProjectEvolution.Utility.BinarySerialization
{
    /// <summary>
    /// Represents the binary reader used to read the simulation data from the file.
    /// </summary>
    internal static class BinReader
    {
        /// <summary>
        /// The main data transfer object.
        /// </summary>
        private static MainDTO _mainDTO;
        /// <summary>
        /// The array of the ticks data transfer objects.
        /// </summary>
        private static TickDTO[] _ticksDTOs;

        /// <summary>
        /// The current tick number.
        /// </summary>
        private static int _currentTickNumber = 0;

        /// <summary>
        /// The event invoked when the bin reader reaches the end of the data in 
        /// the save file.
        /// </summary>
        public static event Action DataEnd;

        /// <summary>
        /// Gets the array of the ticks data transfer objects.
        /// </summary>
        public static TickDTO[] TickDTOs => _ticksDTOs;

        /// <summary>
        /// Gets and privately sets the simulation info data transfer object.
        /// </summary>
        public static SimulationInfoDTO SimulationInfo { get; private set; }

        /// <summary>
        /// Gets and privately sets the total number of the ticks.
        /// </summary>
        public static int TotalTicksNumber { get; private set; }

        /// <summary>
        /// Gets and sets the current tick number.
        /// </summary>
        public static int CurrentTickNumber
        {
            get { return _currentTickNumber; }
            set 
            {
                if (value < 0) _currentTickNumber = 0;
                else if (value >= _ticksDTOs.Length)
                {
                    DataEnd.Invoke();
                    _currentTickNumber = _ticksDTOs.Length - 1;
                }
                else _currentTickNumber = value;
            }
        }

        /// <summary>
        /// Gets the creatures data for the current tick.
        /// </summary>
        /// <returns>
        /// The array of the creatures data.
        /// </returns>
        public static CreatureTickData[] GetCreaturesTickData()
        {
            var creaturesDTOs = _ticksDTOs[_currentTickNumber].CreaturesData;
            var creaturesData = new CreatureTickData[creaturesDTOs.Length];

            for (int i = 0; i < creaturesDTOs.Length; i++)
            {
                creaturesData[i] = GetCreaturesDataFromDTO(creaturesDTOs[i]);
            }
            return creaturesData;
        }

        /// <summary>
        /// Gets the plants data for the current tick.
        /// </summary>
        /// <returns>
        /// The tuple of the number of the plants and the array of 
        /// the plants data.
        /// </returns>
        public static (int plantsNumber, PlantTickData[] plantsData) GetPlantsTickData()
        {
            var tickDTO = _ticksDTOs[_currentTickNumber];
            var plantsDTOs = tickDTO.PlantsData;
            var plantsNumber = tickDTO.PlantsNumber;
            var plantsData = new PlantTickData[plantsDTOs.Length];

            for (int i = 0; i < plantsDTOs.Length; i++)
            {
                plantsData[i] = GetPlantDataFromDTO(plantsDTOs[i]);
            }
            return (plantsNumber, plantsData);
        }

        /// <summary>
        /// Loads the new file with the simulation data.
        /// </summary>
        public static void LoadNewFile()
        {
            byte[] blob = File.ReadAllBytes("result.bin");
            _mainDTO = MessagePackSerializer.Deserialize<MainDTO>(blob);
            SimulationInfo = _mainDTO.SimulationInfo;
            _ticksDTOs = _mainDTO.TicksData;
            PlotsCreater.CreatePlots(_ticksDTOs);
            TotalTicksNumber = _ticksDTOs.Length;
            _currentTickNumber = 0;
        }

        /// <summary>
        /// Gets the creature data from the creature data transfer object.
        /// </summary>
        /// <param name="creatureDTO">
        /// The creature data transfer object.
        /// </param>
        /// <returns>
        /// The creature data formatted as the <see cref="CreatureTickData"/> object.
        /// </returns>
        private static CreatureTickData GetCreaturesDataFromDTO(CreatureDTO creatureDTO)
        {
            var position = creatureDTO.Position;
            var positionVector = new Vector2(position.x, position.y);
            VChromosome chromosome = null;
            if (creatureDTO.Genes != null)
            {
                chromosome = new VChromosome(creatureDTO.Genes);
            }
            return new CreatureTickData(creatureDTO.ID, positionVector, creatureDTO.State, chromosome);
        }

        /// <summary>
        /// Gets the plant data from the plant data transfer object.
        /// </summary>
        /// <param name="plantDTO">
        /// The plant data transfer object.
        /// </param>
        /// <returns>
        /// The plant data formatted as the <see cref="PlantTickData"/> object.
        /// </returns>
        private static PlantTickData GetPlantDataFromDTO(PlantDTO plantDTO)
        {
            var id = plantDTO.ID;
            var partsNumber = plantDTO.PartsNumber;
            Vector2? position = null;
            if (plantDTO.Position.HasValue)
            {
                var (x, y) = plantDTO.Position.Value;
                position = new Vector2(x, y);
            }
            return new PlantTickData(id, position, partsNumber);
        }
    }
}