using Godot;

public partial class CreatureStaticBody : StaticBody3D
{
    [Export] private MeshInstance3D _mesh3D;

    public void SetHeight(float heightFillness)
    {
        var heightRange = 2;
        var height = 0.5f + heightFillness * heightRange;
        var scaleFactor = height / 1.5f;
        _mesh3D.Scale = new Vector3(1, scaleFactor, 1);
        
        Position += new Vector3(0, height / 2f, 0);
    }
}
