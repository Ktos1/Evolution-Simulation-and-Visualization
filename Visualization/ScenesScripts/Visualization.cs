using Godot;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Utility.BinarySerialization;
using ProjectEvolution.Visualization.LogicScripts;

namespace ProjectEvolution.Visualization
{
    public partial class Visualization : Node3D
    {
        [Export] private TimeSliderPanel _timeSliderPanel;
        [Export] private HSlider _timeSlider;
        [Export] private Button _startStopButton;
        [Export] public GenesWindow GenesWindow;
        [Export] private BaseButton _plotsButton;
        [Export] private MeshInstance3D _floorMesh;
        [Export] private Label _yearLabel;

        private PlotsScene _plotsScene;

        private double _deltaCount = 0;
        
        private VCreaturesManager _creaturesManager;
        private VPlantManager _plantsManager;

        private bool _isTimeSliderDragging = false;
        private bool _isStartStopButtonToggled = false;
        private bool _isOnEnd = false;

        public override void _Ready()
        {
            _timeSlider.DragStarted += OnTimeSliderDragStarted;
            _timeSlider.DragEnded += OnTimeSliderDragEnded;
            _startStopButton.Toggled += OnStartStopButtonToggled;
            _plotsButton.Pressed += OnPlotsButtonPressed;

            _plotsScene = ResourceLoader.Load<PackedScene>(
                    "res://Visualization/Scenes/PlotsScene.tscn").Instantiate() as PlotsScene;
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

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsActionPressed("pick_object"))
            {
                _creaturesManager.UncheckAllCreatures();
                GenesWindow.Visible = false;
            }
        }

        private void LoadOnTick(int tickNumber)
        {
            ResetVisualizationState();
            _creaturesManager.LoadOnTick(tickNumber);
            _plantsManager.LoadOnTick(tickNumber);
            BinReader.CurrentTickNumber = tickNumber + 1;
        }

        private void ResetVisualizationState()
        {
            _creaturesManager.Clear();
            _plantsManager.Clear();
            _deltaCount = 0;
        }

        private void OnDataEnd()
        {
            _isOnEnd = true;
        }

        private void OnTimeSliderDragStarted()
        {
            _deltaCount = 0;
            _isTimeSliderDragging = true;
            _timeSlider.ValueChanged += OnTimeSliderValueChanged;
            _timeSliderPanel.IsRunning = false;
        }

        private void OnTimeSliderDragEnded(bool valueChanged)
        {
            _isTimeSliderDragging = false;
            _timeSlider.ValueChanged -= OnTimeSliderValueChanged;
            _timeSliderPanel.IsRunning = true;
            _isOnEnd = false;
        }

        private void OnTimeSliderValueChanged(double value)
        {
            LoadOnTick((int)value);
        }

        private void OnStartStopButtonToggled(bool value)
        {
            _isStartStopButtonToggled = value;
        }

        private void OnPlotsButtonPressed()
        {
            var root = GetTree().Root;
            root.RemoveChild(this);
            root.AddChild(_plotsScene);
        }
    }
}
