using Godot;

namespace ProjectEvolution.Visualization.ScenesStorages
{
    /// <summary>
    /// Contains the prefabricated main scenes.
    /// </summary>
    internal static class Scenes
    {
        /// <summary>
        /// The prefabricated scene for the menu.
        /// </summary>
        public static PackedScene MenuScene { get; } =
            ResourceLoader.Load<PackedScene>("res://Visualization/Scenes/Menu.tscn");
        /// <summary>
        /// The prefabricated scene for the simulation.
        /// </summary>
        public static PackedScene SimulationScene { get; } =
            ResourceLoader.Load<PackedScene>("res://Visualization/Scenes/Simulation.tscn");
        /// <summary>
        /// The prefabricated scene for the visualization.
        /// </summary>
        public static PackedScene VisualizationScene { get; } =
            ResourceLoader.Load<PackedScene>("res://Visualization/Scenes/Visualization.tscn");
        /// <summary>
        /// The prefabricated scene for the plots.
        /// </summary>
        public static PackedScene PlotsScene { get; } =
            ResourceLoader.Load<PackedScene>("res://Visualization/Scenes/PlotsScene.tscn");
    }
}
