using Godot;
using ProjectEvolution;
using System.Diagnostics;

public partial class Menu : Control
{
    public override void _Ready()
    {
        // adding a listener to get a debug output in VS (Trace share listeners to Debug)
        Trace.Listeners.Add(new DefaultTraceListener());

        GetNode<Button>("BoxContainer/SimulationButton").Pressed += OnSimulationButtonPress;
        GetNode<Button>("BoxContainer/VisualizationButton").Pressed += OnVisualizationButtonPress;
        GetNode<Button>("BoxContainer/ExitButton").Pressed += OnExitButtonPress;
    }

    private void OnSimulationButtonPress()
    {
        GetTree().ChangeSceneToPacked(Scenes.SimulationScene);
    }
    private void OnVisualizationButtonPress()
    {
        GetTree().ChangeSceneToPacked(Scenes.VisualizationScene);
    }
    private void OnExitButtonPress()
    {
        // here should be a close notification in the future
        // which will inform nodes about the upcoming process termination 
        GetTree().Quit();
    }
}
