using Godot;

public partial class Camera3d : Node3D
{
    [ExportGroup("Settings")]
    [Export] private float _cameraMotionSensivity = 5f;
    [Export] private float _cameraRotationSensivity = 10f;

    private Vector3 _direction = Vector3.Zero;
    private Vector2 _mouseInput = Vector2.Zero;
    private float _mouseInputFactor = 0.0001f;
    private Vector3 _cameraRotation = Vector3.Zero;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            _mouseInput = (@event as InputEventMouseMotion).ScreenRelative;
            _mouseInput *= _mouseInputFactor * _cameraRotationSensivity;
            _cameraRotation.X += -_mouseInput.Y;
            _cameraRotation.Y += -_mouseInput.X;
            Rotation = _cameraRotation;
        }
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsPressed())
        {
            if (@event.IsActionPressed("left"))
            {
                _direction += Vector3.Left;
            }
            if (@event.IsActionPressed("right"))
            {
                _direction += Vector3.Right;
            }
            if (@event.IsActionPressed("up"))
            {
                _direction += Vector3.Up;
            }
            if (@event.IsActionPressed("down"))
            {
                _direction += Vector3.Down;
            }
            if (@event.IsActionPressed("back"))
            {
                _direction += Vector3.Back;
            }
            if (@event.IsActionPressed("forward"))
            {
                _direction += Vector3.Forward;
            }
        }
        else
        {
            if (@event.IsActionReleased("left"))
            {
                _direction -= Vector3.Left;
            }
            if (@event.IsActionReleased("right"))
            {
                _direction -= Vector3.Right;
            }
            if (@event.IsActionReleased("up"))
            {
                _direction -= Vector3.Up;
            }
            if (@event.IsActionReleased("down"))
            {
                _direction -= Vector3.Down;
            }
            if (@event.IsActionReleased("back"))
            {
                _direction -= Vector3.Back;
            }
            if (@event.IsActionReleased("forward"))
            {
                _direction -= Vector3.Forward;
            }
        }
    }

    public override void _Process(double delta)
    {
        if (_direction != Vector3.Zero)
        {
            TranslateObjectLocal(_direction.Normalized() * (float)delta * _cameraMotionSensivity);
        }
    }
}
