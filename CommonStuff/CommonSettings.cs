/// <summary>
/// Contains the classes used in the visualization and the simulation parts of the project.
/// </summary>
namespace ProjectEvolution.CommonStuff
{
    /// <summary>
    /// Contains the settings used in the visualization and the simulation parts of the project.
    /// </summary>
    static class CommonSettings
    {
        /// <summary>
        /// Duration of a tick in seconds.
        /// </summary>
        /// <remarks>
        /// It is set to 1/30 part of a second that means there is 30 ticks per second.
        /// </remarks>
        public const float TICK_DURATION = 1 / 30f;

        /// <summary>
        /// The number of ticks per second.
        /// </summary>
        public const float TICKS_PER_SECONDS = 1 / TICK_DURATION;

        /// <summary>
        /// The duration of a year in ticks.
        /// </summary>
        public const int YEAR_DURATION = 500;

        /// <summary>
        /// Specifies whether the JSON save file of a simulation should be generated.
        /// </summary>
        public static bool GenerateJSON = false;
    }
}
