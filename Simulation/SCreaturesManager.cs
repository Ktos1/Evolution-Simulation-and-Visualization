using Godot;
using ProjectEvolution.Utility.BinarySerialization;
using System.Collections.Generic;
using System.Threading;

namespace ProjectEvolution.Simulation
{
    /// <summary>
    /// Manages the creatures in the simulation part.
    /// </summary>
    public class SCreaturesManager
    {
        /// <summary>
        /// The list of creatures.
        /// </summary>
        private List<SCreature> _creatures = new List<SCreature>();
        /// <summary>
        /// The list of creatures that will be deleted from the main 
        /// <see cref="_creatures"/> list at the end of a tick.
        /// </summary>
        private List<SCreature> _deletedCreatures = new List<SCreature>();
        /// <summary>
        /// The list of creatures that will be added to the main
        /// <see cref="_creatures"/> list at the end of a tick.
        /// </summary>
        private List<SCreature> _bornCreatures = new List<SCreature>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SCreaturesManager"/> class.
        /// </summary>
        /// <param name="controller">
        /// The simulation controller.
        /// </param>
        /// <param name="cancelToken">
        /// Used to safely abort the simulation which is being processed on separate thread.
        /// </param>
        public SCreaturesManager(
            SimulationController controller, 
            CancellationToken cancelToken)
        {
            for (int i = 0; i < SimulationSettings.CreaturesNumber; i++)
            {
                _creatures.Add(new SCreature(this, controller));
                cancelToken.ThrowIfCancellationRequested();
            }
        }

        /// <summary>
        /// Processes the creatures logic for one tick and saves the results in "new"
        /// variables.
        /// </summary>
        /// <param name="cancelToken">
        /// Used to safely abort the simulation which is being processed on separate thread.
        /// </param>
        public void ProcessCreatures(CancellationToken cancelToken)
        {
            foreach (var creature in _creatures)
            {
                creature.Process();
                cancelToken.ThrowIfCancellationRequested();
            }
        }

        /// <summary>
        /// Updates the creatures by assigning the values of the "new" variables to 
        /// the "right" variables.
        /// </summary>
        /// <remarks>
        /// The method also adds the creatures from the <see cref="_bornCreatures"/> list
        /// to the <see cref="_creatures"/> list and removes the creatures from the
        /// <see cref="_deletedCreatures"/> list from the <see cref="_creatures"/> list.
        /// </remarks>
        /// <param name="cancelToken">
        /// Used to safely abort the simulation which is being processed on separate thread.
        /// </param>
        public void UpdateCreatures(CancellationToken cancelToken)
        {
            foreach (var creature in _creatures)
            {
                creature.Update();
                cancelToken.ThrowIfCancellationRequested();
            }
            _creatures.AddRange(_bornCreatures);
            _creatures.RemoveAll(creature => _deletedCreatures.Contains(creature));
            _bornCreatures.Clear();
            _deletedCreatures.Clear();
        }

        /// <summary>
        /// Returns the list of creatures in the range of the given point.
        /// </summary>
        /// <param name="centerPoint">
        /// The point from which the range is calculated.
        /// </param>
        /// <param name="range">
        /// The range of the point.
        /// </param>
        /// <returns>
        /// The list of tuples containing a distance between the center point and 
        /// a creature and the creature itself.
        /// </returns>
        public List<(float distance, SCreature creature)> GetCreaturesInRange(Vector2 centerPoint, float range)
        {
            var creaturesInRange = new List<(float distance, SCreature creature)>();
            foreach (var creature in _creatures)
            {
                var distance = (centerPoint - creature.Position).Length();
                if (distance <= range && distance != 0)
                {
                    creaturesInRange.Add((distance, creature));
                }
            }
            return creaturesInRange;
        }

        /// <summary>
        /// Gets the saving data of the creatures for the current tick.
        /// </summary>
        /// <returns>
        /// An array of <see cref="CreatureDTO"/> objects corresponding to the creatures
        /// for the current tick.
        /// </returns>
        public CreatureDTO[] GetSavingData()
        {
            var creaturesDTOs = new CreatureDTO[_creatures.Count];
            for (int i = 0; i < _creatures.Count; i++)
            {
                SCreature creature = _creatures[i];
                var (id, position, state, genes) = creature.GetSavingData();
                creaturesDTOs[i] = new CreatureDTO(id, position, state, genes);
            }
            return creaturesDTOs;
        }

        /// <summary>
        /// Called when a creature is born.
        /// </summary>
        /// <param name="bornCreature">
        /// The creature that was born.
        /// </param>
        public void OnCreatureBirth(SCreature bornCreature)
        {
            _bornCreatures.Add(bornCreature);
        }

        /// <summary>
        /// Called when a creature is deleted.
        /// </summary>
        /// <param name="deletedCreature">
        /// The creature that was deleted.
        /// </param>
        public void OnCreatureDeletion(SCreature deletedCreature)
        {
            _deletedCreatures.Add(deletedCreature);
        }

    }
}
