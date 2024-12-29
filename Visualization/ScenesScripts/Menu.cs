using Godot;
using ProjectEvolution.Utility.BinarySerialization;
using System;
using System.Diagnostics;

/// <summary>
/// Contains the scripts attached to the scenes.
/// </summary>
namespace ProjectEvolution.Visualization.ScenesScripts
{
    /// <summary>
    /// Represents the menu scene.
    /// </summary>
    public partial class Menu : Control
    {
        /// <summary>
        /// The button to change to the simulation scene.
        /// </summary>
        [Export] private Button _simulationButton;
        /// <summary>
        /// The button to change to the visualization scene.
        /// </summary>
        [Export] private Button _visualizationButton;
        /// <summary>
        /// The button to exit the application.
        /// </summary>
        [Export] private Button _exitButton;
        /// <summary>
        /// The label to display the error message.
        /// </summary>
        /// <remarks>
        /// The error message is displayed when the visualization button is pressed and 
        /// there is no simulation save file.
        /// </remarks>
        [Export] private Label _errorLabel;
        /// <summary>
        /// The checkbox to toggle fullscreen mode.
        /// </summary>
        [Export] private CheckBox _fullscreenButton;

        /// <inheritdoc/>
        public override void _Ready()
        {
            // adding a listener to get a debug output in VS (Trace share listeners with
            // Debug)
            Trace.Listeners.Add(new DefaultTraceListener());

            _simulationButton.Pressed += OnSimulationButtonPress;
            _visualizationButton.Pressed += OnVisualizationButtonPress;
            _exitButton.Pressed += OnExitButtonPress;
            _fullscreenButton.Toggled += OnFullscreenButtonPress;
            SceneManager.Initialize(this, GetTree().Root);
        }

        /// <summary>
        /// Handles the Simulation button press.
        /// </summary>
        /// <remarks>
        /// Changes to the simulation scene.
        /// </remarks>
        private void OnSimulationButtonPress()
        {
            _errorLabel.Visible = false;
            SceneManager.ChangeToScene(SceneType.Simulation);
        }

        /// <summary>
        /// Handles the Visualization button press.
        /// </summary>
        /// <remarks>
        /// Changes to the visualization scene.
        /// </remarks>
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

        /// <summary>
        /// Handles the Fullscreen button press.
        /// </summary>
        /// <remarks>
        /// Toggles fullscreen mode.
        /// </remarks>
        private void OnFullscreenButtonPress(bool toggledOn)
        {
            if (toggledOn)
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
            else
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }

        /// <summary>
        /// Handles the Exit button press.
        /// </summary>
        /// <remarks>
        /// Exits the application.
        /// </remarks>
        private void OnExitButtonPress()
        {
            GetTree().Quit();
        }
    }
}
