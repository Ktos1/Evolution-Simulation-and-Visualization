using Godot;

/// <summary>
/// Contains the prefabricated scenes for the visualization.
/// </summary>
namespace ProjectEvolution.Visualization.ScenesStorages
{
    /// <summary>
    /// Contains the prefabricated elements of the scenes.
    /// </summary>
    internal static class Prefabs
    {
        /// <summary>
        /// The prefabricated scene for the creature.
        /// </summary>
        public static PackedScene Creature { get; } =
            GD.Load<PackedScene>("res://Visualization/SubScenes/Creature.tscn");

        /// <summary>
        /// The prefabricated scene for the gene info row.
        /// </summary>
        public static PackedScene GeneInfoRow { get; } =
            GD.Load<PackedScene>("res://Visualization/SubScenes/GeneInfoRow.tscn");

        /// <summary>
        /// The prefabricated scene for the plant.
        /// </summary>
        public static PackedScene Plant { get; } =
            GD.Load<PackedScene>("res://Visualization/SubScenes/Plant.tscn");
    }
}
