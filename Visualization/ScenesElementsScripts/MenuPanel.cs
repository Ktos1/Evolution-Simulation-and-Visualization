using Godot;
using ProjectEvolution.Visualization.ScenesScripts;

public partial class MenuPanel : Panel
{
    [Export] private Button _menuButton;
    [Export] private Button _exitButton;
    [Export] private Button _slidingButton;
    
    public override void _Ready()
    {
        _menuButton.Pressed += OnMenuButtonPressed;
        _exitButton.Pressed += OnExitButtonPressed;
        _slidingButton.Toggled += OnSlidingButtonToggled;
    }

    private void OnSlidingButtonToggled(bool newState)
    {
        if (newState) Position += new Vector2(310, 0);
        else Position += new Vector2(-310, 0);
    }

    private void OnMenuButtonPressed()
    {
        _slidingButton.ButtonPressed = false;
        SceneManager.ChangeToScene(SceneType.Menu);
    }
    private void OnExitButtonPressed()
    {
        GetTree().Quit();
    }
}
