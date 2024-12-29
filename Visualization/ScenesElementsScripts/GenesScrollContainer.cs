using Godot;

namespace ProjectEvolution.Visualization.ScenesElementsScripts
{
    /// <summary>
    /// Represents a scroll container for genes in the genes window.
    /// </summary>
    public partial class GenesScrollContainer : ScrollContainer
    {
        /// <inheritdoc/>
        /// <remarks>
        /// Used to catch the zoom in and out events, and accept them if the mouse is over 
        /// the scroll container. This is needed to zoom in and out actions do not trigger
        /// on the scene when the mouse is over the scroll container.
        /// </remarks>
        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseEvent)
            {
                if ((@event.IsActionPressed("zoom_in") || @event.IsActionPressed("zoom_out")) &&
                    GetGlobalRect().HasPoint(GetGlobalMousePosition()))
                {
                    AcceptEvent();
                }
            }
        }
    } 
}
