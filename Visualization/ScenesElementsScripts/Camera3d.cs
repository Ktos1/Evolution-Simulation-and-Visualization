using Godot;
using ProjectEvolution.Utility.BinarySerialization;

public partial class Camera3d : Camera3D
{
    [ExportGroup("Settings")]
    [Export] private float _cameraMotionSensivity = 5f;
    [Export] private float _cameraRotationSensivity = 10f;

    private float _sprintMultiplier = 4;

    private Vector3 _direction = Vector3.Zero;
    private Vector3 _toGlobalDirection = Vector3.Zero;
    private Vector2 _mouseInput = Vector2.Zero;
    private Vector3 _cameraRotation;
    private float _mouseInputFactor = 0.0001f;
    private (float x, float y) _cameraLimitations;
    private bool _isCameraOnFloor = false;
    private bool _isCameraOnCeiling = false;
    private bool _isRotationModeOn = false;
    private bool _isSprintPressed = false;


    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Confined;
        _cameraRotation = Rotation;
        var mapSize = BinReader.SimulationInfo.MapSize;
        _cameraLimitations = (mapSize.x / 2f + 5, mapSize.y / 2f + 5);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion && _isRotationModeOn)
        {
            _mouseInput = (@event as InputEventMouseMotion).ScreenRelative;
            _mouseInput *= _mouseInputFactor * _cameraRotationSensivity;
            _cameraRotation.X -= _mouseInput.Y;
            _cameraRotation.Y -= _mouseInput.X;
            Clamp(ref _cameraRotation.X, -70, -10);
            Rotation = _cameraRotation;
        }
        if (@event is InputEventMouseButton)
        {
            if (@event.IsActionPressed("zoom_in") && !_isCameraOnFloor)
            {
                TranslateObjectLocal(Vector3.Forward * 0.8f);
                if (Position.Y < 2)
                {
                    var newPosition = Position;
                    newPosition.Y = 2f;
                    Position = newPosition;
                    _isCameraOnFloor = true;
                }
                if (_isCameraOnCeiling)
                {
                    _isCameraOnCeiling = false;
                }
            }
            if (@event.IsActionPressed("zoom_out") && !_isCameraOnCeiling)
            {
                TranslateObjectLocal(Vector3.Back * 0.8f);
                if (Position.Y > 13)
                {
                    var newPosition = Position;
                    newPosition.Y = 13f;
                    Position = newPosition;
                    _isCameraOnCeiling = true;
                }
                if (_isCameraOnFloor)
                {
                    _isCameraOnFloor = false;
                }
            }
            if (@event.IsActionPressed("rotation_mode"))
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
                _isRotationModeOn = true;
            }
            if (@event.IsActionReleased("rotation_mode"))
            {
                Input.MouseMode = Input.MouseModeEnum.Confined;
                _isRotationModeOn = false;
            }
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
            if (@event.IsActionPressed("back"))
            {
                _toGlobalDirection += Vector3.Back;
            }
            if (@event.IsActionPressed("forward"))
            {
                _toGlobalDirection += Vector3.Forward;
            }
            if (@event.IsActionPressed("faster_move"))
            {
                _isSprintPressed = true;
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
            if (@event.IsActionReleased("back"))
            {
                _toGlobalDirection -= Vector3.Back;
            }
            if (@event.IsActionReleased("forward"))
            {
                _toGlobalDirection -= Vector3.Forward;
            }
            if (@event.IsActionReleased("faster_move"))
            {
                _isSprintPressed = false;
            }
        }
    }

    public override void _Process(double delta)
    {
        var positionChanged = false;
        var targetSpeed = _cameraMotionSensivity * (_isSprintPressed ? _sprintMultiplier : 1);

        if (_direction != Vector3.Zero)
        {
            TranslateObjectLocal(_direction.Normalized() * (float)delta * targetSpeed);
            positionChanged = true;
        }
        if (_toGlobalDirection != Vector3.Zero)
        {
            var globalDirection = Transform.Basis * _toGlobalDirection;
            globalDirection.Y = 0;
            Transform = Transform.Translated(globalDirection.Normalized()
                * (float)delta * targetSpeed);
            positionChanged = true;
        }

        if (positionChanged)
        {
            if (Position.X > _cameraLimitations.x)
            {
                Position = Position * new Vector3(0, 1, 1) + new Vector3(_cameraLimitations.x, 0, 0);
            }
            else if (Position.X < -_cameraLimitations.x)
            {
                Position = Position * new Vector3(0, 1, 1) + new Vector3(-_cameraLimitations.x, 0, 0);
            }
            if (Position.Z > _cameraLimitations.y)
            {
                Position = Position * new Vector3(1, 1, 0) + new Vector3(0, 0, _cameraLimitations.y);
            }
            else if (Position.Z < -_cameraLimitations.y)
            {
                Position = Position * new Vector3(1, 1, 0) + new Vector3(0, 0, -_cameraLimitations.y);
            }
        }
    }

    private void Clamp(ref float value, float min, float max)
    {
        var minInRad = min * (Mathf.Pi / 180);
        var maxInRad = max * (Mathf.Pi / 180);

        if (value < minInRad)
        {
            value = minInRad;
        }
        else if (value > maxInRad)
        {
            value = maxInRad;
        }
    }
}
