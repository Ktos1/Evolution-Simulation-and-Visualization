using Godot;
using ProjectEvolution;
using ProjectEvolution.Utility.BinarySerialization;
using System.Diagnostics;

public partial class Menu : Control
{
    [Export] private Button _simulationButton;
    [Export] private Button _visualizationButton;
    [Export] private Button _exitButton;
    [Export] private Label _errorLabel;

    public override void _Ready()
    {
        // adding a listener to get a debug output in VS (Trace share listeners to Debug)
        Trace.Listeners.Add(new DefaultTraceListener());

        _simulationButton.Pressed += OnSimulationButtonPress;
        _visualizationButton.Pressed += OnVisualizationButtonPress;
        _exitButton.Pressed += OnExitButtonPress;
    }

    private void OnSimulationButtonPress()
    {
        GetTree().ChangeSceneToPacked(Scenes.SimulationScene);
    }
    private void OnVisualizationButtonPress()
    {
        try
        {
            var temp = BinReader.CurrentTickNumber;
            GetTree().ChangeSceneToPacked(Scenes.VisualizationScene);
        }
        catch (System.Exception)
        {
            _errorLabel.Visible = true;
        }
    }
    private void OnExitButtonPress()
    {
        // here should be a close notification in the future
        // which will inform nodes about the upcoming process termination 
        GetTree().Quit();
    }
}
