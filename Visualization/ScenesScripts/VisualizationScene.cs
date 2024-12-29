using Godot;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Utility.BinarySerialization;
using ProjectEvolution.Visualization.LogicScripts;
using ProjectEvolution.Visualization.ScenesElementsScripts;
using ProjectEvolution.Visualization.ScenesStorages;

namespace ProjectEvolution.Visualization.ScenesScripts
{
    /// <summary>
    /// Represents the visualization scene.
    /// </summary>
    public partial class VisualizationScene : Node3D
    {
        /// <summary>
        /// The time slider panel.
        /// </summary>
        [Export] private TimeSliderPanel _timeSliderPanel;
        /// <summary>
        /// The time slider.
        /// </summary>
        [Export] private HSlider _timeSlider;
        /// <summary>
        /// The Start/Stop button.
        /// </summary>
        /// <remarks>
        /// Used to start or stop the time of the visualization.
        /// </remarks>
        [Export] private Button _startStopButton;
        /// <summary>
        /// The genes window.
        /// </summary>
        /// <remarks>
        /// Used to display the genes of the selected creature.
        /// </remarks>
        [Export] public GenesWindow GenesWindow;
        /// <summary>
        /// The plots button.
        /// </summary>
        [Export] private BaseButton _plotsButton;
        /// <summary>
        /// The floor mesh.
        /// </summary>
        [Export] private MeshInstance3D _floorMesh;
        /// <summary>
        /// The label displaying the actual year of the visualization.
        /// </summary>
        [Export] private Label _yearLabel;

        /// <summary>
        /// The plots scene.
        /// </summary>
        private PlotsScene _plotsScene;

        /// <summary>
        /// The delta count.
        /// </summary>
        /// <remarks>
        /// Used to count the time since the last tick loaded.
        /// </remarks>
        private double _deltaCount = 0;

        /// <summary>
        /// The creatures manager.
        /// </summary>
        private VCreaturesManager _creaturesManager;
        /// <summary>
        /// The plants manager.
        /// </summary>
        private VPlantManager _plantsManager;

        /// <summary>
        /// Indicates if the time slider is being dragged.
        /// </summary>
        private bool _isTimeSliderDragging = false;
        /// <summary>
        /// Indicates if the Start/Stop button is toggled.
        /// </summary>
        private bool _isStartStopButtonToggled = false;
        /// <summary>
        /// Indicates if the visualization is on the end of time.
        /// </summary>
        private bool _isOnEnd = false;

        /// <inheritdoc/>
        public override void _Ready()
        {
            _timeSlider.DragStarted += OnTimeSliderDragStarted;
            _timeSlider.DragEnded += OnTimeSliderDragEnded;
            _startStopButton.Toggled += OnStartStopButtonToggled;
            _plotsButton.Pressed += OnPlotsButtonPressed;

            _plotsScene = Scenes.PlotsScene.Instantiate() as PlotsScene;
            _plotsScene.Initialize(this);

            _creaturesManager = new VCreaturesManager(this);
            _plantsManager = new VPlantManager(this);
            BinReader.CurrentTickNumber++;
            GenesWindow.AverageStartGenesValues = _creaturesManager.AverageStartGenesValues;
            BinReader.DataEnd += OnDataEnd;

            (int x, int y) = BinReader.SimulationInfo.MapSize;
            var mesh = _floorMesh.Mesh as BoxMesh;
            mesh.Size = new Vector3(x, 0.5f, y);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Advances the visualization if the time slider is not being dragged,
        /// the Start/Stop button is not toggled and the visualization is not on 
        /// the end of time.
        /// </remarks>
        public override void _Process(double delta)
        {
            if (!_isTimeSliderDragging && !_isStartStopButtonToggled && !_isOnEnd)
            {
                _deltaCount += delta;
                if (_deltaCount > CommonSettings.TICK_DURATION)
                {
                    _plantsManager.UpdatePlants();
                    _creaturesManager.UpdateCreatures();
                    _deltaCount -= CommonSettings.TICK_DURATION;
                    BinReader.CurrentTickNumber++;
                }
                _creaturesManager.ProcessCreatures(_deltaCount);
            }
            _yearLabel.Text = $"Rok: " +
                $"{Mathf.CeilToInt(BinReader.CurrentTickNumber / (float)CommonSettings.YEAR_DURATION)}";
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Handles the pick object action.
        /// </remarks>
        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsActionPressed("pick_object"))
            {
                _creaturesManager.UncheckAllCreatures();
                GenesWindow.Visible = false;
            }
        }

        /// <summary>
        /// Loads the visualization on the given tick.
        /// </summary>
        /// <param name="tickNumber">
        /// The tick number.
        /// </param>
        private void LoadOnTick(int tickNumber)
        {
            ResetVisualizationState();
            _creaturesManager.LoadOnTick(tickNumber);
            _plantsManager.LoadOnTick(tickNumber);
            BinReader.CurrentTickNumber = tickNumber + 1;
        }

        /// <summary>
        /// Resets the visualization state.
        /// </summary>
        private void ResetVisualizationState()
        {
            _creaturesManager.Clear();
            _plantsManager.Clear();
            _deltaCount = 0;
        }

        /// <summary>
        /// Handles the data end event.
        /// </summary>
        private void OnDataEnd()
        {
            _isOnEnd = true;
        }

        /// <summary>
        /// Handles the time slider drag start event.
        /// </summary>
        private void OnTimeSliderDragStarted()
        {
            _deltaCount = 0;
            _isTimeSliderDragging = true;
            _timeSlider.ValueChanged += OnTimeSliderValueChanged;
            _timeSliderPanel.IsRunning = false;
        }

        /// <summary>
        /// Handles the time slider drag end event.
        /// </summary>
        /// <param name="valueChanged">
        /// Indicates if the value has changed.
        /// </param>
        private void OnTimeSliderDragEnded(bool valueChanged)
        {
            _isTimeSliderDragging = false;
            _timeSlider.ValueChanged -= OnTimeSliderValueChanged;
            _timeSliderPanel.IsRunning = true;
            if (BinReader.CurrentTickNumber != BinReader.TotalTicksNumber - 1)
                _isOnEnd = false;
        }

        /// <summary>
        /// Handles the time slider value change event.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        private void OnTimeSliderValueChanged(double value)
        {
            LoadOnTick((int)value);
        }

        /// <summary>
        /// Handles the start/stop button toggle event.
        /// </summary>
        /// <param name="value">
        /// Indicates if the button is toggled.
        /// </param>
        private void OnStartStopButtonToggled(bool value)
        {
            _isStartStopButtonToggled = value;
            string iconPath = "Resources/Icons/";
            if (value) iconPath += "start.png";
            else iconPath += "stop.png";
            var iconImage = Image.LoadFromFile(iconPath);
            _startStopButton.Icon = ImageTexture.CreateFromImage(iconImage);
        }

        /// <summary>
        /// Handles the plots button press event.
        /// </summary>
        private void OnPlotsButtonPressed()
        {
            var root = GetTree().Root;
            root.RemoveChild(this);
            root.AddChild(_plotsScene);
        }
    }
}
