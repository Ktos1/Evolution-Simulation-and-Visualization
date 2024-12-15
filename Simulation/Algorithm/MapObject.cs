using Godot;

namespace ProjectEvolution.Simulation.Algorithm
{
    public abstract class MapObject
    {
        protected Vector2 _position;

        public Vector2 Position => _position;

        public delegate void DeleteEventHandler(MapObject deletedObj);
    }
}
