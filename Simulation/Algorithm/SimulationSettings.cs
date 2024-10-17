namespace ProjectEvolution.Simulation.Algorithm
{
    static class SimulationSettings
    {
        /// <summary>
        /// The time in ticks needed to give a birth.
        /// </summary>
        public static int ReproductionTime = 60;

        public static float MutationChance = 0.2f;

        /// <summary>
        /// The percentage of a gene possible range on which the standard deviation of the mutation is set.
        /// </summary>
        public static float MutationStdDev = 5;
    }
}
