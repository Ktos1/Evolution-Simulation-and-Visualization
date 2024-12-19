namespace ProjectEvolution.Simulation.Algorithm
{
    static class SimulationSettings
    {
        public static Map Map { get; set; }

        public static int SimulDurationInYears { get; set; }

        public static int CreaturesNumber { get; set; }

        /// <summary>
        /// The time in ticks needed to give a birth.
        /// </summary>
        public static int ReproductionDuration = 60;

        public static float ReproductionCost = 25f;

        public static float CrossoverChance = 0.80f;

        public static float MutationChance = 0.01f;

        /// <summary>
        /// The percentage of a gene possible range on which the standard deviation of the mutation is set.
        /// </summary>
        public static float MutationStdDev = 0.05f;

        public static int EatingDuration = 10;

        public static float EnergyFromPlantPart = 5;

        /// <summary>
        /// The duration in ticks of died creature presence on the map.
        /// </summary>
        public static int DiedStateDuration = 60;

        /// <summary>
        /// The time to a plant grow.
        /// </summary>
        public static int TimeToPlantGrow = 180;

        public static int TimeToClusterRespawn = 180;

        public static int PlantsNumberInRespawnedCluster = 4;

        public static int MaxPlantsNumberToRespawn = 160;
    }
}
