using Godot;

namespace ProjectEvolution.Visualization
{
    internal static class Prefabs
    {
        public static PackedScene Creature { get; } =
            GD.Load<PackedScene>("res://Visualization/MapObjects/Creature.tscn");
    }
}
