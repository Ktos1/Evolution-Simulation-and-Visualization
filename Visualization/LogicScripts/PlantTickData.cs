using Godot;

namespace ProjectEvolution.Visualization.LogicScripts
{
    /// <summary>
    /// Contains the data of a plant in a tick.
    /// </summary>
    /// <param name="Id">
    /// The id of the plant.
    /// </param>
    /// <param name="Position">
    /// The position of the plant.
    /// </param>
    /// <param name="PartsNumber">
    /// The number of the parts of the plant.
    /// </param>
    internal record class PlantTickData
        (uint Id, Vector2? Position, int PartsNumber);
}
