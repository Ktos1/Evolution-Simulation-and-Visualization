using Godot;

public partial class CreatureStaticBody : StaticBody3D
{
    [Export] private MeshInstance3D _mesh3D;

    private Color _currentColor;
    private Color _defaultColor;
    private Color _deathColor;
    private Color _checkColor;
    private bool _isDead = false;

    public void SetHeight(float heightFillness)
    {
        var heightRange = 2;
        var height = 0.5f + heightFillness * heightRange;
        var scaleFactor = height / 1.5f;
        _mesh3D.Scale = new Vector3(1, scaleFactor, 1);
        
        Position += new Vector3(0, height / 2f, 0);
    }

    public void SetColorSaturation(float saturation)
    {
        var targetSaturation = saturation * 0.7f + 0.3f;
        _defaultColor = Color.FromHsv(0, targetSaturation, 1);
        _checkColor = Color.FromHsv(0.676056f, _defaultColor.S, 1);

        var targetBrightness = (1 - saturation) * 0.7f;
        _deathColor = Color.FromHsv(0, 0, targetBrightness);

        var material = new StandardMaterial3D();
        material.AlbedoColor = _defaultColor;
        _mesh3D.SetSurfaceOverrideMaterial(0, material);
        _currentColor = _defaultColor;
    }

    public void ChangeToDeathColor()
    {
        if (_currentColor != _checkColor)
            ChangeColor(_deathColor);
        _isDead = true;
    }

    public void ChangeColorBasedOnSelection(bool isSelected)
    {
        if (isSelected)
            ChangeColor(_checkColor);
        else
        {
            if (_isDead) ChangeColor(_deathColor);
            else ChangeColor(_defaultColor);
        }
    }

    public void MoveTo(Vector2 newPosition)
    {
        var oldPosition3D = Position;
        var oldPosition2D = new Vector2(oldPosition3D.X, oldPosition3D.Z);
        var movementVector = newPosition - oldPosition2D;

        Rotate(movementVector.Angle());
        Position += new Vector3(movementVector.X, 0, movementVector.Y);
    }

    private void ChangeColor(Color color)
    {
        var material = _mesh3D.GetSurfaceOverrideMaterial(0) as StandardMaterial3D;
        material.AlbedoColor = color;
        _currentColor = color;
    }

    private void Rotate(float angle)
    {
        Rotation = new Vector3(0, -angle, 0);
    }
}
