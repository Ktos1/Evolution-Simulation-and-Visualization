using Godot;
using System.Linq;
using System.Collections.Generic;
using ProjectEvolution.Visualization;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Utility.BinarySerialization;

public partial class Visualization : Node3D
{
    [Export] private HSlider _timeSlider;
    [Export] private Button _startStopButton;
    [Export] private GenesWindow _genesWindow;
    [Export] private MeshInstance3D _floorMesh;

    private double _deltaCount = 0;
    private List<VCreature> _creatures = new List<VCreature>();
    private List<VCreature> _deadCreatures = new List<VCreature>();

    private bool _isTimeSliderDragging = false;
    private bool _isStartStopButtonToggled = false;

    public override void _Ready()
    {
        InitializeCreatures();
        _timeSlider.DragStarted += OnTimeSliderDragStarted;
        _timeSlider.DragEnded += OnTimeSliderDragEnded;
        _startStopButton.Toggled += OnStartStopButtonToggled;

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
                UpdateCreatures();
                _deltaCount -= CommonSettings.TICK_DURATION;
            }
            ProcessCreatures(_deltaCount);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("pick_object"))
        {
            _creatures.ForEach((creature) => creature.Uncheck());
            _genesWindow.Visible = false;
        }
    }

    private void InitializeCreatures()
    {
        var creaturesData = BinReader.NextTick();
        _genesWindow.AverageStartGenesValues = CalculateAverageGenesValues(creaturesData.Select((x) => x.chromosome).ToArray());
        foreach (var creature in creaturesData)
        {
            var (position, state, chromosome) = creature;
            AddNewCreature(position, state, chromosome);
        }
    }

    private void UpdateCreatures()
    {
        var creaturesData = BinReader.NextTick();
        if (creaturesData is null)
        {
            _startStopButton.ButtonPressed = true;
            return;
        }
        for (int i = 0; i < creaturesData.Length; i++)
        {
            if (i < _creatures.Count)
            {
                var creature = _creatures[i];
                var (position, state, _) = creaturesData[i];
                _creatures[i].Update(position, state);
                if (state == CreatureStates.ToDelete)
                {
                    _deadCreatures.Add(_creatures[i]);
                } 
            }
            else
            {
                var (spawnPosition, state, chromosome) = creaturesData[i];
                AddNewCreature(spawnPosition, state, chromosome);
            }
        }
        DeleteDeadCreatures();
    }

    private void DeleteDeadCreatures()
    {
        _deadCreatures.ForEach((deadCreature) =>
        {
            deadCreature.Delete();
            _creatures.Remove(deadCreature);
        });
        _deadCreatures.Clear();
    }

    private void AddNewCreature(Vector2 spawnPosition, CreatureStates state, VChromosome chromosome)
    {
        var creature = new VCreature(spawnPosition, state, chromosome);
        creature.ClickedOn += _genesWindow.OnClickedOnCreature;
        _creatures.Add(creature);
        CallDeferred("add_child", _creatures.Last().StaticBody);
    }

    /// <summary>
    /// Processes creatures without loading new tick data. It uses the linear interpolation to make creature
    /// movement more smooth.
    /// </summary>
    /// <param name="timeCounterBetweenTicks">
    /// Current time counted from last tick data loading. It is used to the interpolation.
    /// </param>
    private void ProcessCreatures(double timeCounterBetweenTicks)
    {
        _creatures.ForEach((creature) => creature.Process((float)timeCounterBetweenTicks));
    }

    private void LoadOnTick(int tickNumber)
    {
        foreach (var creature in _creatures)
        {
            creature.Delete();
        }
        _creatures.Clear();
        _deadCreatures.Clear();
        _deltaCount = 0;
        BinReader.CurrentTickNumber = tickNumber;

        var creaturesData = BinReader.NextTick();
        for (int i = 0; i < creaturesData.Length; i++)
        {
            var(position, state, chromosome) = creaturesData[i];
            AddNewCreature(position, state, chromosome);
        }
    }

    private float[] CalculateAverageGenesValues(VChromosome[] chromosomes)
    {
        float sum = 0;
        int genesNumber = chromosomes[0].Genes.Length;
        var result = new float[genesNumber];

        for (int i = 0; i < genesNumber; i++)
        {
            for (int j = 0; j < chromosomes.Length; j++)
            {
                sum += chromosomes[j].Genes[i].Value;
            }
            result[i] = sum / chromosomes.Length;
            sum = 0;
        }
        return result;
    }

    private void OnTimeSliderDragStarted()
    {
        _deltaCount = 0;
        _isTimeSliderDragging = true;
        _timeSlider.ValueChanged += OnTimeSliderValueChanged;
    }

    private void OnTimeSliderDragEnded(bool valueChanged)
    {
        _isTimeSliderDragging = false;
        _timeSlider.ValueChanged -= OnTimeSliderValueChanged;
    }

    private void OnTimeSliderValueChanged(double value)
    {
        LoadOnTick(value == 0 ? 0 : (int)value - 1);
        UpdateCreatures();
        ProcessCreatures(CommonSettings.TICK_DURATION);
    }

    private void OnStartStopButtonToggled(bool value)
    {
        _isStartStopButtonToggled = value;
    }
}
