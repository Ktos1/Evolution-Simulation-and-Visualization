using Godot;
using ProjectEvolution.Utility.BinarySerialization;

/// <summary>
/// Contains the scripts of the scene elements.
/// </summary>
namespace ProjectEvolution.Visualization.ScenesElementsScripts
{
    /// <summary>
    /// Script attached to the camera.
    /// </summary>
    /// <remarks>
    /// This script handles the camera movement and rotation.
    /// </remarks>
    public partial class Camera : Camera3D
    {
        /// <summary>
        /// The sensitivity of the camera movement.
        /// </summary>
        [ExportGroup("Settings")]
        [Export] private float _cameraMotionSensivity = 5f;
        /// <summary>
        /// The sensitivity of the camera rotation.
        /// </summary>
        [Export] private float _cameraRotationSensivity = 10f;

        /// <summary>
        /// The speed multiplier for the sprint mode.
        /// </summary>
        private float _sprintMultiplier = 4;

        /// <summary>
        /// The local component of the direction of the camera movement.
        /// </summary>
        private Vector3 _direction = Vector3.Zero;
        /// <summary>
        /// The global component of the direction of the camera movement.
        /// </summary>
        /// <remarks>
        /// This direction is converted to the global space.
        /// </remarks>
        private Vector3 _toGlobalDirection = Vector3.Zero;
        /// <summary>
        /// The mouse motion input.
        /// </summary>
        private Vector2 _mouseInput = Vector2.Zero;
        /// <summary>
        /// The rotation of the camera.
        /// </summary>
        private Vector3 _cameraRotation;
        /// <summary>
        /// The mouse motion input factor.
        /// </summary>
        private float _mouseInputFactor = 0.0001f;
        /// <summary>
        /// The limitations of the camera movement.
        /// </summary>
        private (float x, float y) _cameraLimitations;
        /// <summary>
        /// Whether the camera is on the floor.
        /// </summary>
        private bool _isCameraOnFloor = false;
        /// <summary>
        /// Whether the camera is on the ceiling.
        /// </summary>
        private bool _isCameraOnCeiling = false;
        /// <summary>
        /// Whether the rotation mode is on.
        /// </summary>
        private bool _isRotationModeOn = false;
        /// <summary>
        /// Whether the sprint mode is active.
        /// </summary>
        private bool _isSprintActive = false;

        /// <inheritdoc/>
        public override void _Ready()
        {
            _cameraRotation = Rotation;
            var mapSize = BinReader.SimulationInfo.MapSize;
            _cameraLimitations = (mapSize.x / 2f + 5, mapSize.y / 2f + 5);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Used to handle a mouse input. 
        /// </remarks>
        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventMouseMotion && _isRotationModeOn)
            {
                _mouseInput = (@event as InputEventMouseMotion).ScreenRelative;
                _mouseInput *= _mouseInputFactor * _cameraRotationSensivity;
                _cameraRotation.X -= _mouseInput.Y;
                _cameraRotation.Y -= _mouseInput.X;
                ClampRadInDegrees(ref _cameraRotation.X, -70, -10);
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
                    Input.MouseMode = Input.MouseModeEnum.Visible;
                    _isRotationModeOn = false;
                }
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Used to handle a keyboard input.
        /// </remarks>
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
                    _isSprintActive = true;
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
                    _isSprintActive = false;
                }
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Used to update the camera position and rotation.
        /// </remarks>
        public override void _Process(double delta)
        {
            var positionChanged = false;
            var targetSpeed = _cameraMotionSensivity * (_isSprintActive ? _sprintMultiplier : 1);

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

        /// <summary>
        /// Clamps the given value in radians between the given minimum and maximum values 
        /// in degrees.
        /// </summary>
        /// <param name="radValue">
        /// The value in radians to clamp.
        /// </param>
        /// <param name="minDegrees">
        /// The minimum value in degrees.
        /// </param>
        /// <param name="maxDegrees">
        /// The maximum value in degrees.
        /// </param>
        private void ClampRadInDegrees(ref float radValue, float minDegrees, float maxDegrees)
        {
            var minInRad = minDegrees * (Mathf.Pi / 180);
            var maxInRad = maxDegrees * (Mathf.Pi / 180);

            if (radValue < minInRad)
            {
                radValue = minInRad;
            }
            else if (radValue > maxInRad)
            {
                radValue = maxInRad;
            }
        }
    } 
}
