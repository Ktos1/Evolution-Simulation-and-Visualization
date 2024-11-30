using Godot;
using ProjectEvolution.Utility.BinarySerialization;

namespace ProjectEvolution.Simulation.Algorithm
{
    internal class Plant : MapObject
    {
        private static uint _iDCounter = 0;

        private uint _id = _iDCounter++;
        private int _partsNumber;
        private int _newPartsNumber;
        private int _timeWithoutBeingEaten = 0;
        private int _newTimeWithoutBeingEaten = 0;

        private bool _wasEaten = false;
        private bool _wasModified = false;
        private bool _saveAfterSpawn = true;

        public Plant(Vector2 position, int partsNumber)
        {
            _position = position;
            _partsNumber = _newPartsNumber = partsNumber;
        }

        public void Process()
        {
            if (!_wasEaten)
            {
                _newTimeWithoutBeingEaten++;
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
            else
            {
                _newTimeWithoutBeingEaten = 0;
                _wasEaten = true;
            } 
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

        private void Delete()
        {

        }
    }
}
