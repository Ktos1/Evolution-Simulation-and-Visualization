namespace ProjectEvolution.Simulation.Algorithm
{
    static class SimulationSettings
    {
        /// <summary>
        /// Duration of the tick in seconds.
        /// </summary>
        /// <remarks>
        /// It is set to 1/30 part of a second. That means there is 30 ticks per second.
        /// </remarks>
        public const float TICK_DURATION = 0.0333f;

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
