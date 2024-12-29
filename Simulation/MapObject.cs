using Godot;

namespace ProjectEvolution.Simulation
{
    /// <summary>
    /// Represents an object on the map.
    /// </summary>
    public abstract class MapObject
    {
        /// <summary>
        /// The position of the object on the map.
        /// </summary>
        protected Vector2 _position;

        /// <summary>
        /// Gets the position of the object on the map.
        /// </summary>
        public Vector2 Position => _position;

        /// <summary>
        /// Used to handle a deletion event of a map object.
        /// </summary>
        /// <param name="deletedObj">
        /// The map object to delete.
        /// </param>
        public delegate void DeletionEventHandler(MapObject deletedObj);
    }
}
