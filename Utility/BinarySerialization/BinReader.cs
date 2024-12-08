using Godot;
using MessagePack;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Visualization;
using ProjectEvolution.Visualization.LogicScripts;
using System;
using System.IO;

namespace ProjectEvolution.Utility.BinarySerialization
{
    internal static class BinReader
    {
        private static MainDTO _mainDTO;
        private static TickDTO[] _ticksDTOs;

        private static int _currentTickNumber = 0;

        public static event Action DataEnd;

        public static SimulationInfoDTO SimulationInfo { get; private set; }

        public static int TotalTicksNumber { get; private set; }
        public static int CurrentTickNumber
        {
            get { return _currentTickNumber; }
            set { _currentTickNumber = (value < 0) ? 0 : value; }
        }

        static BinReader()
        {
            LoadNewFile();
        }

        public static void NextTick()
        {
            if (++_currentTickNumber >= _ticksDTOs.Length)
            {
                _currentTickNumber--;
                DataEnd.Invoke();
            } 
        }

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

        public static (int plantsNumber, PlantTickData[] plantsData) GetPlantTickData()
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

        public static void LoadNewFile()
        {
            byte[] blob = File.ReadAllBytes("result.bin");
            _mainDTO = MessagePackSerializer.Deserialize<MainDTO>(blob);
            SimulationInfo = _mainDTO.SimulationInfo;
            _ticksDTOs = _mainDTO.TicksData;
            TotalTicksNumber = _ticksDTOs.Length;
            _currentTickNumber = 0;
        }

        private static CreatureTickData GetCreaturesDataFromDTO(CreatureDTO creatureDTO)
        {
            var position = creatureDTO.Position;
            var positionVector = new Vector2(position.x, position.y);
            VChromosome chromosome = null;
            if (creatureDTO.Genes != null)
            {
                chromosome = new VChromosome(creatureDTO.Genes);
            }
            return new CreatureTickData(creatureDTO.ID, positionVector, creatureDTO.CurrentState, chromosome);
        }

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