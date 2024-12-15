namespace ProjectEvolution.CommonStuff
{
    static class CommonSettings
    {
        /// <summary>
        /// Duration of the tick in seconds.
        /// </summary>
        /// <remarks>
        /// It is set to 1/30 part of a second that means there is 30 ticks per second.
        /// </remarks>
        public const float TICK_DURATION = 1 / 30f;

        public const float TICKS_PER_SECONDS = 1 / TICK_DURATION;

        public const int YEAR_DURATION = 500; // in ticks

        public static bool GenerateJSON = false;
    }
}
