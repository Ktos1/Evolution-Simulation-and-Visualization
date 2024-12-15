using Godot;
using ProjectEvolution;
using ProjectEvolution.Simulation.Algorithm;

public partial class Simulation : Control
{
    [Export] private TabBar _tabBar;

    // common
    [Export] private VBoxContainer _commonContainer;
    [Export] private SpinBox _creturesNum;
    [Export] private SpinBox _xMapSize;
    [Export] private SpinBox _yMapSize;
    [Export] private SpinBox _simulationDuration;

    // plants
    [Export] private VBoxContainer _plantsContainer;
    [Export] private SpinBox _clustersDensity;
    [Export] private SpinBox _clusterSize;
    [Export] private SpinBox _clusterDensity;

    [Export] private Button _startButton;
    [Export] private Button _backButton;
    
    
    public override void _Ready()
    {
        _startButton.Pressed += OnStartButtonPress;
        _backButton.Pressed += OnBackButtonPress;
        _tabBar.TabChanged += ChangeSceneTo;
    }

    private void ChangeSceneTo(long scene)
    {
        if (scene == 0)
        {
            _commonContainer.Visible = true;
            _plantsContainer.Visible = false;
        }
        else
        {
            _commonContainer.Visible = false;
            _plantsContainer.Visible = true;
        }
    }

    private void OnStartButtonPress()
    {
        var simulationControler = new SimulationController(
            new Map((int)_xMapSize.Value, (int)_yMapSize.Value),
            (int)_creturesNum.Value,
            (float)_clustersDensity.Value,
            (float)_clusterSize.Value,
            (float)_clusterDensity.Value
            );
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
