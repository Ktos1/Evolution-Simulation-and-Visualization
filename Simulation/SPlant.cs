using Godot;
using ProjectEvolution.Utility.BinarySerialization;
using System.Threading;

namespace ProjectEvolution.Simulation
{
    /// <summary>
    /// Represents a plant in the simulation part.
    /// </summary>
    internal class SPlant : MapObject
    {
        /// <summary>
        /// The counter used to assign unique identifiers to the plants.
        /// </summary>
        private static uint _idCounter = 0;

        /// <summary>
        /// The cluster containing the plant.
        /// </summary>
        private Cluster _cluster;

        /// <summary>
        /// The unique identifier of the plant.
        /// </summary>
        private uint _id = _idCounter++;
        /// <summary>
        /// The parts number of the plant.
        /// </summary>
        private int _partsNumber;
        /// <summary>
        /// The new parts number of the plant.
        /// </summary>
        /// <remarks>
        /// It is used to change the right parts number of the creature after the current 
        /// tick is processed.
        /// </remarks>
        private int _newPartsNumber;
        /// <summary>
        /// The time without being eaten.
        /// </summary>
        private int _timeWithoutBeingEaten = 0;
        /// <summary>
        /// The new time without being eaten.
        /// </summary>
        /// <remarks>
        /// It is used to change the right time without being eaten after the current tick
        /// is processed.
        /// </remarks>
        private int _newTimeWithoutBeingEaten = 0;

        /// <summary>
        /// Indicates whether the plant was eaten in the current tick.
        /// </summary>
        private bool _wasEaten = false;
        /// <summary>
        /// Indicates whether the plant was modified in the current tick.
        /// </summary>
        private bool _wasModified = false;
        /// <summary>
        /// Indicates whether the plant should be saved appropriately for the first save
        /// after the spawn.
        /// </summary>
        private bool _saveAfterSpawn = true;

        /// <summary>
        /// Occurs when the plant is going to be deleted on the end of a tick.
        /// </summary>
        public event DeletionEventHandler Deleted;

        /// <summary>
        /// Initializes a new instance of the <see cref="SPlant"/> class.
        /// </summary>
        /// <param name="position">
        /// The position of the plant.
        /// </param>
        /// <param name="partsNumber">
        /// The number of parts of the plant.
        /// </param>
        /// <param name="cluster">
        /// The cluster containing the plant.
        /// </param>
        public SPlant(Vector2 position, int partsNumber, Cluster cluster)
        {
            _position = position;
            _partsNumber = _newPartsNumber = partsNumber;
            _cluster = cluster;
        }

        /// <summary>
        /// Processes the logic of the plant for one tick.
        /// </summary>
        /// <param name="cancelToken">
        /// Used to safely abort the simulation which is being processed on separate thread.
        /// </param>
        public void Process(CancellationToken cancelToken)
        {
            if (!_wasEaten)
            {
                _newTimeWithoutBeingEaten++;
                if (_newTimeWithoutBeingEaten > SimulationSettings.TimeToPlantGrow)
                {
                    Grow();
                    _newTimeWithoutBeingEaten = 0;
                }
            }
            cancelToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Updates the plant after the current tick is processed.
        /// </summary>
        public void Update()
        {
            _wasModified = _partsNumber != _newPartsNumber;
            _partsNumber = _newPartsNumber;
            _timeWithoutBeingEaten = _newTimeWithoutBeingEaten;
            _wasEaten = false;
        }

        /// <summary>
        /// The method called by the creature when it eaten a plant part.
        /// </summary>
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

        /// <summary>
        /// Gets the saving data of the plant.
        /// </summary>
        /// <returns>
        /// The saving data of the plant.
        /// </returns>
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

        /// <summary>
        /// Resets the counter of the unique identifiers of the plants to 0.
        /// </summary>
        public static void ResetIds()
        {
            _idCounter = 0;
        }

        /// <summary>
        /// Grows the plant.
        /// </summary>
        /// <remarks>
        /// If the plant has 4 parts, it tries to propagate the plant. Otherwise, it 
        /// increases the number of parts of the plant. 
        /// </remarks>
        private void Grow()
        {
            if (_partsNumber == 4)
            {
                _cluster.TryPropagatePlant(this);
            }
            else
            {
                _newPartsNumber++;
            }
        }

        /// <summary>
        /// Deletes the plant.
        /// </summary>
        private void Delete()
        {
            Deleted.Invoke(this);
        }
    }
}
