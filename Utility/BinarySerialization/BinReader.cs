using Godot;
using MessagePack;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Visualization;
using System.IO;

namespace ProjectEvolution.Utility.BinarySerialization
{
    internal static class BinReader
    {
        private static MainDTO _mainDTO;
        private static TickDTO[] _ticksDTOs;

        private static int _currentTickNumber = -1;

        public static SimulationInfoDTO SimulationInfo { get; private set; }

        public static int TotalTicksNumber { get; private set; }
        public static int CurrentTickNumber
        {
            get { return _currentTickNumber; }
            set { _currentTickNumber = (value < 0) ? -1 : value - 1; }
        }

        static BinReader()
        {
            LoadNewFile();
        }

        public static CreatureTickData[] NextTick()
        {
            _currentTickNumber++;
            if (_currentTickNumber >= _ticksDTOs.Length) return null;

            var creaturesDTOs = _ticksDTOs[_currentTickNumber].CreaturesData;
            var creaturesData = new CreatureTickData[creaturesDTOs.Length];

            for (int i = 0; i < creaturesDTOs.Length; i++)
            {
                creaturesData[i] = GetCreaturesDataFromDTO(creaturesDTOs[i]);
            }
            return creaturesData;
        }

        public static void LoadNewFile()
        {
            byte[] blob = File.ReadAllBytes("result.bin");
            _mainDTO = MessagePackSerializer.Deserialize<MainDTO>(blob);
            SimulationInfo = _mainDTO.SimulationInfo;
            _ticksDTOs = _mainDTO.TicksData;
            TotalTicksNumber = _ticksDTOs.Length;
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
    }
}
