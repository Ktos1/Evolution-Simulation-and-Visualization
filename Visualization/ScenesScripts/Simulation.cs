using Godot;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Simulation.Algorithm;
using ProjectEvolution.Visualization.ScenesScripts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectEvolution.Visualization
{
    public partial class Simulation : Control
    {
        [Export] private TabContainer _tabContainer;

        // common
        [Export] private SpinBox _xMapSize;
        [Export] private SpinBox _yMapSize;
        [Export] private SpinBox _simulationDuration;

        // creatures
        [Export] private SpinBox _creturesNum;
        [Export] private SpinBox _reproductionDuration;
        [Export] private SpinBox _reproductionCost;
        [Export] private SpinBox _crossoverChance;
        [Export] private SpinBox _mutationChance;
        [Export] private SpinBox _mutationStdDev;
        [Export] private SpinBox _eatingDuration;
        [Export] private SpinBox _energyFromPlantPart;

        // plants
        [Export] private SpinBox _clustersDensity;
        [Export] private SpinBox _clusterSize;
        [Export] private SpinBox _clusterDensity;
        [Export] private SpinBox _timeToPlantGrow;
        [Export] private SpinBox _timeToClusterResp;
        [Export] private SpinBox _plantsNumInRespCluster;
        [Export] private SpinBox _maxPlantsNumToResp;


        [Export] private ProgressBar _progressBar;
        private Label _successLabel;

        [Export] private HBoxContainer _buttonsContainer;
        [Export] private Button _startButton;
        [Export] private Button _defaultSettBtn;
        [Export] private Button _backButton;
        

        [Export] private Button _abortButton;

        private List<SpinBox> _spinBoxs = new List<SpinBox>();

        private int _totalTicks;

        private bool _isSimulating = false;
        private int _actualTick;

        private CancellationTokenSource _cancelTokSource;

        public event Action NewSimulation;
        public event Action ResetToDefault;

        private bool IsSimulating
        {
            get { return _isSimulating; }
            set
            {
                if (value)
                {
                    _buttonsContainer.Visible = false;
                    _abortButton.Visible = true;
                    _progressBar.Visible = true;
                    _successLabel.Visible = false;
                    ChangeEditableForSpinBoxs(false);
                }
                else
                {
                    _buttonsContainer.Visible = true;
                    _abortButton.Visible = false;
                    _progressBar.Visible = false;
                    ChangeEditableForSpinBoxs(true);
                }
                _isSimulating = value;
            }
        }

        public override void _Ready()
        {
            _startButton.Pressed += OnStartButtonPress;
            _backButton.Pressed += OnBackButtonPress;
            _abortButton.Pressed += OnAbortButtonPress;
            _defaultSettBtn.Pressed += OnDefaultSettBtnPress;
            _successLabel = GetNode<Label>("Panel/SuccessLabel");
            GetSpinBoxReferences();
        }

        public override void _Process(double delta)
        {
            if (IsSimulating)
            {
                _progressBar.Value = _actualTick / (float)_totalTicks;
            }
        }

        private void ChangeEditableForSpinBoxs(bool isEditable)
        {
            _spinBoxs.ForEach(spinBox => spinBox.Editable = isEditable);
        }

        private void GetSpinBoxReferences()
        {
            var fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(SpinBox))
                {
                    var spinBox = field.GetValue(this) as SpinBox;
                    _spinBoxs.Add(spinBox);
                }
            }
        }

        private void SubmitSettings()
        {
            // general
            SimulationSettings.Map = new Map((int)_xMapSize.Value, (int)_yMapSize.Value);
            SimulationSettings.SimulDurationInYears = (int)_simulationDuration.Value;
            
            // creatures
            SimulationSettings.CreaturesNumber = (int)_creturesNum.Value;
            SimulationSettings.ReproductionDuration = (int)(_reproductionDuration.Value * 30);
            SimulationSettings.ReproductionCost = (float)_reproductionCost.Value;
            SimulationSettings.CrossoverChance = (float)_crossoverChance.Value;
            SimulationSettings.MutationChance = (float)_mutationChance.Value;
            SimulationSettings.MutationStdDev = (float)_mutationStdDev.Value;
            SimulationSettings.EatingDuration = (int)(_eatingDuration.Value * 30);
            SimulationSettings.EnergyFromPlantPart = (float)_energyFromPlantPart.Value;

            // plants
            SimulationSettings.TimeToPlantGrow = (int)(_timeToPlantGrow.Value * 30);
            SimulationSettings.TimeToClusterRespawn = (int)(_timeToClusterResp.Value * 30);
            SimulationSettings.PlantsNumberInRespawnedCluster = (int)_plantsNumInRespCluster.Value;
            SimulationSettings.MaxPlantsNumberToRespawn = (int)_maxPlantsNumToResp.Value;
        }

        private void Simulate(CancellationToken cancellationToken)
        {
            var simulationControler = new SimulationController(
                new Map((int)_xMapSize.Value, (int)_yMapSize.Value),
                (int)_creturesNum.Value,
                (float)_clustersDensity.Value,
                (float)_clusterSize.Value,
                (float)_clusterDensity.Value,
                cancellationToken
                );

            simulationControler.StartSimulation(cancellationToken, ref _actualTick);
        }

        private async void OnStartButtonPress()
        {
            IsSimulating = true;
            SubmitSettings();
            _totalTicks = SimulationSettings.SimulDurationInYears * CommonSettings.YEAR_DURATION;

            _cancelTokSource = new CancellationTokenSource();

            try
            {
                await Task.Run(() =>
                {
                    Simulate(_cancelTokSource.Token);
                });
                _successLabel.Visible = true;
                NewSimulation.Invoke();
            }
            catch (OperationCanceledException) { }
            finally
            {
                IsSimulating = false;
            }
        }

        private void OnDefaultSettBtnPress()
        {
            ResetToDefault.Invoke();
        }

        private void OnBackButtonPress()
        {
            _successLabel.Visible = false;
            SceneManager.ChangeToScene(SceneType.Menu);
        }

        private void OnAbortButtonPress()
        {
            _cancelTokSource.Cancel();
        }
    }
}
