using Godot;
using ProjectEvolution.Visualization.ScenesScripts;

namespace ProjectEvolution.Visualization.ScenesElementsScripts
{
    /// <summary>
    /// Represents the menu panel in the visualization scene.
    /// </summary>
    public partial class MenuPanel : Panel
    {
        /// <summary>
        /// The button to go to the menu view.
        /// </summary>
        [Export] private Button _menuButton;
        /// <summary>
        /// The button to exit the application.
        /// </summary>
        [Export] private Button _exitButton;
        /// <summary>
        /// The button to slide out and in the panel.
        /// </summary>
        [Export] private Button _slidingButton;

        /// <inheritdoc/>
        public override void _Ready()
        {
            _menuButton.Pressed += OnMenuButtonPressed;
            _exitButton.Pressed += OnExitButtonPressed;
            _slidingButton.Toggled += OnSlidingButtonToggled;
        }

        /// <summary>
        /// Handles the toggling of the sliding button.
        /// </summary>
        /// <param name="newState">
        /// The new state of the sliding button.
        /// </param>
        /// <remarks>
        /// Slides the panel in or out depending on the new state.
        /// </remarks>
        private void OnSlidingButtonToggled(bool newState)
        {
            if (newState) Position += new Vector2(310, 0);
            else Position += new Vector2(-310, 0);
        }

        /// <summary>
        /// Handles the pressing of the Menu button on the panel.
        /// </summary>
        private void OnMenuButtonPressed()
        {
            _slidingButton.ButtonPressed = false;
            SceneManager.ChangeToScene(SceneType.Menu);
        }

        /// <summary>
        /// Handles the pressing of the Exit button on the panel.
        /// </summary>
        private void OnExitButtonPressed()
        {
            GetTree().Quit();
        }
    }
}