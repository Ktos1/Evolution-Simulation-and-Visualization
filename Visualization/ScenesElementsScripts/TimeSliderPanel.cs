using Godot;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Utility.BinarySerialization;

namespace ProjectEvolution.Visualization.ScenesElementsScripts
{
    /// <summary>
    /// Represents the panel related with the time management.
    /// </summary>
    public partial class TimeSliderPanel : Panel
    {
        /// <summary>
        /// The button to slide out and in the panel.
        /// </summary>
        [Export] private Button _slidingButton;
        /// <summary>
        /// The label displaying the actual time of the visualization.
        /// </summary>
        [Export] private Label _actualTimeLabel;
        /// <summary>
        /// The label displaying the total time of the visualization.
        /// </summary>
        [Export] private Label _totalTimeLabel;
        /// <summary>
        /// The slider to control the time.
        /// </summary>
        [Export] private HSlider _timeSlider;

        /// <summary>
        /// The total ticks number of the simulation saved in the file.
        /// </summary>
        private int _totalTicksNumber = BinReader.TotalTicksNumber;
        /// <summary>
        /// The tick number on which the visualization currently is.
        /// </summary>
        private int _currentTickNumber => BinReader.CurrentTickNumber;
        /// <summary>
        /// Gets or sets whether the simulation is running.
        /// </summary>
        public bool IsRunning { get; set; } = true;

        /// <inheritdoc/>
        public override void _Ready()
        {
            _slidingButton.Toggled += OnSlidingButtonToggled;
            _totalTimeLabel.Text = $"{GetTimeFromTicks(_totalTicksNumber)}";
            _timeSlider.MaxValue = _totalTicksNumber - 1;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Updates the actual time label and the time slider value.
        /// </remarks>
        public override void _Process(double delta)
        {
            _actualTimeLabel.Text = $"{GetTimeFromTicks(_currentTickNumber)}";
            if (IsRunning) _timeSlider.Value = _currentTickNumber;
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
            if (newState) Position += new Vector2(0, -52);
            else Position += new Vector2(0, 52);
        }

        /// <summary>
        /// Converts ticks to a time string.
        /// </summary>
        /// <remarks>
        /// Method calculates the time based on the 
        /// <see cref="CommonSettings.TICK_DURATION"/>
        /// </remarks>
        /// <param name="ticks">
        /// The number of ticks to convert.
        /// </param>
        /// <returns>
        /// The time string in the format "HH:MM:SS".
        /// </returns>
        private string GetTimeFromTicks(int ticks)
        {
            float totalSeconds = ticks * CommonSettings.TICK_DURATION;
            ushort hours = 0;
            ushort minutes = 0;
            ushort seconds = 0;
            if (totalSeconds < 1)
            {
                seconds = 1;
            }
            else
            {
                hours = (ushort)(totalSeconds / 3600);
                totalSeconds -= hours * 3600;
                minutes = (ushort)(totalSeconds / 60);
                totalSeconds -= minutes * 60;
                seconds = (ushort)totalSeconds;
            }
            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }
    }
}