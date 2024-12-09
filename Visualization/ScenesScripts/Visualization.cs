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
        [Export] public GenesWindow _genesWindow;
        [Export] private MeshInstance3D _floorMesh;

        private double _deltaCount = 0;
        
        private VCreaturesManager _creaturesManager;
        private VPlantManager _plantsManager;

        private bool _isTimeSliderDragging = false;
        private bool _isStartStopButtonToggled = false;

        public override void _Ready()
        {
            _timeSlider.DragStarted += OnTimeSliderDragStarted;
            _timeSlider.DragEnded += OnTimeSliderDragEnded;
            _startStopButton.Toggled += OnStartStopButtonToggled;

            _creaturesManager = new VCreaturesManager(this);
            _plantsManager = new VPlantManager(this);
            BinReader.CurrentTickNumber++;
            _genesWindow.AverageStartGenesValues = _creaturesManager.AverageStartGenesValues;
            BinReader.DataEnd += OnDataEnd;

            (int x, int y) = BinReader.SimulationInfo.MapSize;
            var mesh = _floorMesh.Mesh as BoxMesh;
            mesh.Size = new Vector3(x, 0.5f, y);
        }

        public override void _Process(double delta)
        {
            if (!_isTimeSliderDragging && !_isStartStopButtonToggled)
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
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event.IsActionPressed("pick_object"))
            {
                _creaturesManager.UncheckAllCreatures();
                _genesWindow.Visible = false;
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
            _startStopButton.ButtonPressed = true;
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
        }

        private void OnTimeSliderValueChanged(double value)
        {
            LoadOnTick((int)value);
        }

        private void OnStartStopButtonToggled(bool value)
        {
            _isStartStopButtonToggled = value;
        }
    }
}
