using Godot;
using System.Linq;
using System.Collections.Generic;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Utility.BinarySerialization;

namespace ProjectEvolution.Visualization
{
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
            _genesWindow.AverageStartGenesValues = CalculateAverageGenesValues(
                creaturesData.Select((x) => x.Chromosome).ToArray()
                );
            AddNewCreatures(creaturesData);
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
                    _creatures[i].Update(creaturesData[i]);
                }
                else
                {
                    AddNewCreature(creaturesData[i]);
                }
            }
            DeleteDeadCreatures();
        }

        private void DeleteDeadCreatures()
        {
            _deadCreatures.ForEach((deadCreature) =>
            {
                _creatures.Remove(deadCreature);
            });
            _deadCreatures.Clear();
        }

        private void AddNewCreature(CreatureTickData creatureData)
        {
            var creature = new VCreature(creatureData, this);
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
            ResetVisualizationState();
            int tickIndex = tickNumber;
            BinReader.CurrentTickNumber = tickIndex;
            var creaturesData = BinReader.NextTick();
            var idsWithoutChromosome = new List<uint>();
            foreach (var creatureData in creaturesData)
            {
                if (creatureData.Chromosome is null)
                {
                    idsWithoutChromosome.Add(creatureData.Id);
                }
            }
            tickIndex--;

            int missingChromosomeCounter = idsWithoutChromosome.Count;
            while (true)
            {
                BinReader.CurrentTickNumber = tickIndex;
                var tickCreaturesData = BinReader.NextTick();
                for (int i = tickCreaturesData.Length - 1; i >= 0; i--)
                {
                    var (id, _, _, chromosome) = tickCreaturesData[i];
                    if (chromosome is not null)
                    {
                        for (int j = 0; j < idsWithoutChromosome.Count; j++)
                        {
                            if (idsWithoutChromosome[j] == id)
                            {
                                creaturesData[j] = creaturesData[j] with { Chromosome = chromosome };
                                missingChromosomeCounter--;
                            }
                        }
                    }
                    else
                        break;
                }
                if (missingChromosomeCounter == 0)
                {
                    break;
                }
                tickIndex--;
            }
            AddNewCreatures(creaturesData);
            BinReader.CurrentTickNumber = tickNumber + 1;
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

        private void ResetVisualizationState()
        {
            foreach (var creature in _creatures)
            {
                creature.Delete();
            }
            _creatures.Clear();
            _deadCreatures.Clear();
            _deltaCount = 0;
        }

        private void AddNewCreatures(CreatureTickData[] creatures)
        {
            foreach (var creatureData in creatures)
            {
                AddNewCreature(creatureData);
            }
        }

        public void OnCreatureDeletion(VCreature creature)
        {
            _deadCreatures.Add(creature);
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
}
