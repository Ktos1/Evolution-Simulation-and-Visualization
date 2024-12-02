using Godot;

namespace ProjectEvolution.Visualization
{
    internal static class Prefabs
    {
        public static PackedScene Creature { get; } =
            GD.Load<PackedScene>("res://Visualization/SubScenes/Creature.tscn");

        public static PackedScene GeneInfoRow { get; } =
            GD.Load<PackedScene>("res://Visualization/SubScenes/GeneInfoRow.tscn");

        public static PackedScene Plant { get; } =
            GD.Load<PackedScene>("res://Visualization/SubScenes/Plant.tscn");
    }
}
