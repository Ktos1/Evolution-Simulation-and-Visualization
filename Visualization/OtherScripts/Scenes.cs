using Godot;

namespace ProjectEvolution
{
    internal static class Scenes
    {
        public static PackedScene MenuScene { get; } =
            ResourceLoader.Load<PackedScene>("res://Visualization/Scenes/Menu.tscn");
        public static PackedScene SimulationScene { get; } =
            ResourceLoader.Load<PackedScene>("res://Visualization/Scenes/Simulation.tscn");
        public static PackedScene VisualizationScene { get; } =
            ResourceLoader.Load<PackedScene>("res://Visualization/Scenes/Visualization.tscn");
    }
}
