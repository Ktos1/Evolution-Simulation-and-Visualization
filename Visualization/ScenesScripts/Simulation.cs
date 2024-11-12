using Godot;
using ProjectEvolution;
using ProjectEvolution.Simulation.Algorithm;

public partial class Simulation : Control
{
    [Export(PropertyHint.NodeType)] private SpinBox _creturesNum;
    [Export(PropertyHint.NodeType)] private SpinBox _xMapSize;
    [Export(PropertyHint.NodeType)] private SpinBox _yMapSize;
    [Export(PropertyHint.NodeType)] private SpinBox _simulationDuration;
    [Export] private Button _startButton;
    [Export] private Button _backButton;
    
    
    public override void _Ready()
    {
        _startButton.Pressed += OnStartButtonPress;
        _backButton.Pressed += OnBackButtonPress;
    }

    private void OnStartButtonPress()
    {
        var simulationControler = new SimulationController(new Map((int)_xMapSize.Value, (int)_yMapSize.Value), (int)_creturesNum.Value);
        if (simulationControler.StartSimulation((int)_simulationDuration.Value))
        {
            GetNode<Label>("Panel/SuccessLabel").Visible = true;
        }
    }

    private void OnBackButtonPress()
    {
        GetTree().ChangeSceneToPacked(Scenes.MenuScene);
    }
}
