using Godot;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Simulation;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectEvolution.Visualization.ScenesScripts
{
    /// <summary>
    /// Represents the simulation scene.
    /// </summary>
    public partial class Simulation : Control
    {
        /// <summary>
        /// The tab container for categories of parameters.
        /// </summary>
        [Export] private TabContainer _tabContainer;

        // common
        /// <summary>
        /// The spin box for the x size of the map.
        /// </summary>
        [Export] private SpinBox _xMapSize;
        /// <summary>
        /// The spin box for the y size of the map.
        /// </summary>
        [Export] private SpinBox _yMapSize;
        /// <summary>
        /// The spin box for the simulation duration parameter.
        /// </summary>
        [Export] private SpinBox _simulationDuration;

        // creatures
        /// <summary>
        /// The spin box for the number of creatures on start the simulation.
        /// </summary>
        [Export] private SpinBox _creturesNum;
        /// <summary>
        /// The spin box for the reproduction duration parameter.
        /// </summary>
        [Export] private SpinBox _reproductionDuration;
        /// <summary>
        /// The spin box for the reproduction cost parameter.
        /// </summary>
        [Export] private SpinBox _reproductionCost;
        /// <summary>
        /// The spin box for the crossover chance parameter.
        /// </summary>
        [Export] private SpinBox _crossoverChance;
        /// <summary>
        /// The spin box for the mutation chance parameter.
        /// </summary>
        [Export] private SpinBox _mutationChance;
        /// <summary>
        /// The spin box for the mutation standard deviation parameter.
        /// </summary>
        [Export] private SpinBox _mutationStdDev;
        /// <summary>
        /// The spin box for the eating duration parameter.
        /// </summary>
        [Export] private SpinBox _eatingDuration;
        /// <summary>
        /// The spin box for the energy from plant part parameter.
        /// </summary>
        [Export] private SpinBox _energyFromPlantPart;

        // plants
        /// <summary>
        /// The spin box for the clusters density on start the simulation.
        /// </summary>
        [Export] private SpinBox _clustersDensity;
        /// <summary>
        /// The spin box for the cluster size on start the simulation.
        /// </summary>
        [Export] private SpinBox _clusterSize;
        /// <summary>
        /// The spin box for the cluster density on start the simulation.
        /// </summary>
        [Export] private SpinBox _clusterDensity;
        /// <summary>
        /// The spin box for the time to plant grow parameter.
        /// </summary>
        [Export] private SpinBox _timeToPlantGrow;
        /// <summary>
        /// The spin box for the time to cluster respawn parameter.
        /// </summary>
        [Export] private SpinBox _timeToClusterResp;
        /// <summary>
        /// The spin box for the plants number in respawned cluster parameter.
        /// </summary>
        [Export] private SpinBox _plantsNumInRespCluster;
        /// <summary>
        /// The spin box for the max plants number to respawn parameter.
        /// </summary>
        [Export] private SpinBox _maxPlantsNumToResp;

        /// <summary>
        /// The progress bar for the simulation progress.
        /// </summary>
        [Export] private ProgressBar _progressBar;
        /// <summary>
        /// The label for the success message.
        /// </summary>
        private Label _successLabel;

        /// <summary>
        /// The container for the buttons.
        /// </summary>
        [Export] private HBoxContainer _buttonsContainer;
        /// <summary>
        /// The button for starting the simulation.
        /// </summary>
        [Export] private Button _startButton;
        /// <summary>
        /// The button for resetting the simulation parameters to default.
        /// </summary>
        [Export] private Button _defaultSettBtn;
        /// <summary>
        /// The button for going back to the menu.
        /// </summary>
        [Export] private Button _backButton;

        /// <summary>
        /// The button for aborting the simulation.
        /// </summary>
        /// <remarks>
        /// Visible only when the simulation is running.
        /// </remarks>
        [Export] private Button _abortButton;

        /// <summary>
        /// The list of all spin boxes for the simulation parameters.
        /// </summary>
        private List<SpinBox> _spinBoxs = new List<SpinBox>();

        /// <summary>
        /// The total number of ticks for the simulation.
        /// </summary>
        private int _totalTicks;

        /// <summary>
        /// Indicates if the simulation is currently running.
        /// </summary>
        private bool _isSimulating = false;
        /// <summary>
        /// The current tick of the simulation.
        /// </summary>
        /// <remarks>
        /// Used to check on which tick the simulation thread is and update 
        /// the progress bar.
        /// </remarks>
        private int _actualTick;

        /// <summary>
        /// The cancellation token source for the simulation.
        /// </summary>
        private CancellationTokenSource _cancelTokSource;

        /// <summary>
        /// Event triggered when a new simulation is calculated.
        /// </summary>
        public event Action NewSimulation;
        /// <summary>
        /// Event triggered when ResetToDefault button is pressed.
        /// </summary>
        public event Action ResetToDefault;

        /// <summary>
        /// Gets or sets a value indicating if the simulation is currently running.
        /// </summary>
        /// <remarks>
        /// Changes the UI elements visibility and the editable state of the spin boxes
        /// when its value changes.
        /// </remarks>
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

        /// <inheritdoc/>
        public override void _Ready()
        {
            _startButton.Pressed += OnStartButtonPress;
            _backButton.Pressed += OnBackButtonPress;
            _abortButton.Pressed += OnAbortButtonPress;
            _defaultSettBtn.Pressed += OnDefaultSettBtnPress;
            _successLabel = GetNode<Label>("Panel/SuccessLabel");
            GetSpinBoxReferences();
        }

        /// <inheritdoc/>
        public override void _Process(double delta)
        {
            if (IsSimulating)
            {
                _progressBar.Value = _actualTick / (float)_totalTicks;
            }
        }

        /// <summary>
        /// Changes the editable state of the spin boxes.
        /// </summary>
        /// <param name="isEditable">
        /// The new editable state.
        /// </param>
        private void ChangeEditableForSpinBoxs(bool isEditable)
        {
            _spinBoxs.ForEach(spinBox => spinBox.Editable = isEditable);
        }

        /// <summary>
        /// Gets the references to all spin boxes in the scene by reflection.
        /// </summary>
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

        /// <summary>
        /// Submits the simulation parameters to the simulation settings.
        /// </summary>
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

        /// <summary>
        /// Starts the simulation.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token, used to safely abort the simulation which 
        /// is being processed on separate thread.
        /// </param>
        private void Simulate(CancellationToken cancellationToken)
        {
            var simulationControler = new SimulationController(
                (float)_clustersDensity.Value,
                (float)_clusterSize.Value,
                (float)_clusterDensity.Value,
                cancellationToken
                );

            simulationControler.StartSimulation(cancellationToken, ref _actualTick);
        }

        /// <summary>
        /// Handles the Start button press event.
        /// </summary>
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

        /// <summary>
        /// Handles the ResetToDefault button press event.
        /// </summary>
        private void OnDefaultSettBtnPress()
        {
            ResetToDefault.Invoke();
        }

        /// <summary>
        /// Handles the Back button press event.
        /// </summary>
        private void OnBackButtonPress()
        {
            _successLabel.Visible = false;
            SceneManager.ChangeToScene(SceneType.Menu);
        }

        /// <summary>
        /// Handles the Abort button press event.
        /// </summary>
        private void OnAbortButtonPress()
        {
            _cancelTokSource.Cancel();
        }
    }
}
