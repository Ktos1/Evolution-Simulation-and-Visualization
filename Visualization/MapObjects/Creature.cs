using Godot;
using System.Diagnostics;

public partial class Creature : StaticBody3D
{
    public override void _InputEvent(Camera3D camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, int shapeIdx)
    {
        Debug.WriteLine("dupa");
    }
}
