using Godot;
using ProjectEvolution;
using ProjectEvolution.Simulation.Algorithm;

public partial class Simulation : Control
{

    public override void _Ready()
    {
        GetNode<Button>("Panel/ButtonsBoxContainer/StartButton").Pressed += OnStartButtonPress;
        GetNode<Button>("Panel/ButtonsBoxContainer/BackButton").Pressed += OnBackButtonPress;
    }

    private void OnStartButtonPress()
    {
        SimulationController simulationControler = new SimulationController(new Map(10, 10));
        if (simulationControler.StartSimulation(1))
        {
            GetNode<Label>("Panel/SuccessLabel").Visible = true;
        }
    }

    private void OnBackButtonPress()
    {
        GetTree().ChangeSceneToPacked(Scenes.MenuScene);
    }
}
