namespace ProjectEvolution.Simulation
{
    /// <summary>
    /// Contains the settings used in the simulation part of the project.
    /// </summary>
    static class SimulationSettings
    {
        /// <summary>
        /// The map of the simulation.
        /// </summary>
        public static Map Map { get; set; }

        /// <summary>
        /// The simulation duration in years.
        /// </summary>
        public static int SimulDurationInYears { get; set; }

        /// <summary>
        /// The creatures number on the start of the simulation.
        /// </summary>
        public static int CreaturesNumber { get; set; }

        /// <summary>
        /// The time in ticks needed to give a birth.
        /// </summary>
        public static int ReproductionDuration = 60;

        /// <summary>
        /// The energy cost of the reproduction per every parent.
        /// </summary>
        public static float ReproductionCost = 25f;

        /// <summary>
        /// The chance of the crossover operator to be applied.
        /// </summary>
        public static float CrossoverChance = 0.80f;

        /// <summary>
        /// The chance of the mutation operator to be applied.
        /// </summary>
        public static float MutationChance = 0.01f;

        /// <summary>
        /// The percentage of a gene possible range on which the standard deviation of 
        /// a mutation is set.
        /// </summary>
        public static float MutationStdDev = 0.05f;

        /// <summary>
        /// The duration in ticks of the creature's eating.
        /// </summary>
        public static int EatingDuration = 10;

        /// <summary>
        /// The energy gained by the creature from eating one plant part.
        /// </summary>
        public static float EnergyFromPlantPart = 5;

        /// <summary>
        /// The duration in ticks of died creature presence on the map.
        /// </summary>
        public static int DiedStateDuration = 60;

        /// <summary>
        /// The time in ticks to a plant grow.
        /// </summary>
        public static int TimeToPlantGrow = 180;

        /// <summary>
        /// The time in ticks to a cluster of plants respawn.
        /// </summary>
        public static int TimeToClusterRespawn = 180;

        /// <summary>
        /// The number of plants in the respawned cluster.
        /// </summary>
        public static int PlantsNumberInRespawnedCluster = 4;

        /// <summary>
        /// The maximum number of plants to respawn.
        /// </summary>
        public static int MaxPlantsNumberToRespawn = 160;
    }
}
