using Godot;
using ProjectEvolution.Utility.BinarySerialization;

namespace ProjectEvolution.Simulation.Algorithm
{
    internal class SPlant : MapObject
    {
        private static uint _idCounter = 0;

        private SPlantManager _plantManager;

        private uint _id = _idCounter++;
        private int _partsNumber;
        private int _newPartsNumber;
        private int _timeWithoutBeingEaten = 0;
        private int _newTimeWithoutBeingEaten = 0;

        private bool _wasEaten = false;
        private bool _wasModified = false;
        private bool _saveAfterSpawn = true;

        public event DeleteEventHandler Deleted;

        public SPlant(Vector2 position, int partsNumber, SPlantManager plantManager)
        {
            _position = position;
            _partsNumber = _newPartsNumber = partsNumber;
            _plantManager = plantManager;
        }

        public void Process()
        {
            if (!_wasEaten)
            {
                _newTimeWithoutBeingEaten++;
                if (_newTimeWithoutBeingEaten > SimulationSettings.TimePlantToGrow)
                {
                    Grow();
                    _newTimeWithoutBeingEaten = 0;
                }
            }
        }

        public void Update()
        {
            _wasModified = (_partsNumber != _newPartsNumber) ? true : false;
            _partsNumber = _newPartsNumber;
            _timeWithoutBeingEaten = _newTimeWithoutBeingEaten;
            _wasEaten = false;
        }

        public void BeingEaten()
        {
            if (--_newPartsNumber == 0)
            {
                Delete();
            }
            else if (_newPartsNumber < 0)
            {
                _newPartsNumber = 0;
            }
            else
            {
                _newTimeWithoutBeingEaten = 0;
            }
            _wasEaten = true;
        }

        public PlantDTO GetSavingData()
        {
            if (_saveAfterSpawn)
            {
                (float x, float y) = _position;
                _saveAfterSpawn = false;
                return new PlantDTO(_id, (x, y), _partsNumber);
            }
            else if (_wasModified)
            {
                return new PlantDTO(_id, null, _partsNumber);
            }
            else
            {
                return null;
            }
        }

        public static void ResetIds()
        {
            _idCounter = 0;
        }

        private void Grow()
        {
            if (_partsNumber == 4)
            {
                _plantManager.TryPropagatePlant(this);
            }
            else
            {
                _newPartsNumber++;
            }
        }

        private void Delete()
        {
            Deleted.Invoke(this);
        }
    }
}
