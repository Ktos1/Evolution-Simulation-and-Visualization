using Godot;
using ProjectEvolution.Utility.BinarySerialization;
using ProjectEvolution.Visualization.ScenesScripts;
using System;
using System.Diagnostics;

public partial class Menu : Control
{
    [Export] private Button _simulationButton;
    [Export] private Button _visualizationButton;
    [Export] private Button _exitButton;
    [Export] private Label _errorLabel;
    [Export] private CheckBox _fullscreenButton;

    public override void _Ready()
    {
        // adding a listener to get a debug output in VS (Trace share listeners with Debug)
        Trace.Listeners.Add(new DefaultTraceListener());

        _simulationButton.Pressed += OnSimulationButtonPress;
        _visualizationButton.Pressed += OnVisualizationButtonPress;
        _exitButton.Pressed += OnExitButtonPress;
        _fullscreenButton.Toggled += OnFullscreenButtonPress;
        SceneManager.Initialize(this, GetTree().Root);
    }

    private void OnSimulationButtonPress()
    {
        _errorLabel.Visible = false;
        SceneManager.ChangeToScene(SceneType.Simulation);
    }

    private void OnVisualizationButtonPress()
    {
        try
        {
            var temp = BinReader.CurrentTickNumber;
            SceneManager.ChangeToScene(SceneType.Visualization);
        }
        catch (Exception)
        {
            _errorLabel.Visible = true;
        }
    }

    private void OnFullscreenButtonPress(bool toggledOn)
    {
        if (toggledOn)
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        else
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
    }

    private void OnExitButtonPress()
    {
        GetTree().Quit();
    }
}
