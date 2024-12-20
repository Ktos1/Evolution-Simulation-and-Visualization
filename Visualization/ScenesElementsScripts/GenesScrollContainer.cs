using Godot;

public partial class GenesScrollContainer : ScrollContainer
{
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
