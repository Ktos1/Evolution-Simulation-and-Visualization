using Godot;

namespace ProjectEvolution.Visualization
{
    internal static class Prefabs
    {
        public static PackedScene Creature { get; } =
            GD.Load<PackedScene>("res://Visualization/MapObjects/Creature.tscn");

        public static PackedScene GeneInfoRow { get; } =
            GD.Load<PackedScene>("res://gene_info_row.tscn");
    }
}
