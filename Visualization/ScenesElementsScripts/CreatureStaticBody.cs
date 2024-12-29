using Godot;

namespace ProjectEvolution.Visualization.ScenesElementsScripts
{
    /// <summary>
    /// Script attached to the creature static body.
    /// </summary>
    /// <remarks>
    /// This class inherited from <see cref="StaticBody3D"/> godot class and 
    /// represents directly the creature object on the visualization scene.
    /// </remarks>
    public partial class CreatureStaticBody : StaticBody3D
    {
        /// <summary>
        /// The mesh of the creature.
        /// </summary>
        [Export] private MeshInstance3D _mesh3D;

        /// <summary>
        /// The current color of the creature.
        /// </summary>
        private Color _currentColor;
        /// <summary>
        /// The default color of the creature.
        /// </summary>
        private Color _defaultColor;
        /// <summary>
        /// The color of the creature when it is dead.
        /// </summary>
        private Color _deathColor;
        /// <summary>
        /// The color of the creature when it is selected.
        /// </summary>
        private Color _checkColor;
        /// <summary>
        /// Whether the creature is dead.
        /// </summary>
        private bool _isDead = false;

        /// <summary>
        /// Sets the height of the creature.
        /// </summary>
        /// <remarks>
        /// There is a predefined range of height possible modification equal to 2.
        /// </remarks>
        /// <param name="heightFillness">
        /// From 0 to 1 how much the height is increased in possible range.
        /// </param>
        public void SetHeight(float heightFillness)
        {
            var heightRange = 2;
            var height = 0.5f + heightFillness * heightRange;
            var scaleFactor = height / 1.5f;
            _mesh3D.Scale *= new Vector3(1, scaleFactor, 1);

            Position += new Vector3(0, height / 2f, 0);
        }

        /// <summary>
        /// Sets the width of the creature.
        /// </summary>
        /// <remarks>
        /// There is a predefined range of width possible modification equal to 1.1.
        /// </remarks>
        /// <param name="widthFillness">
        /// From 0 to 1 how much the width is increased in possible range.
        /// </param>
        public void SetWidth(float widthFillness)
        {
            var widthRange = 1.1;
            var scaleFactor = 0.5f + widthFillness * widthRange;
            _mesh3D.Scale *= new Vector3(1, 1, (float)scaleFactor);
        }

        /// <summary>
        /// Sets the color of the creature.
        /// </summary>
        /// <param name="saturation">
        /// From 0 to 1 how much the color is saturated in the possible range.
        /// </param>
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

        /// <summary>
        /// Changes the color of the creature to the death color.
        /// </summary>
        public void ChangeToDeathColor()
        {
            if (_currentColor != _checkColor)
                ChangeColor(_deathColor);
            _isDead = true;
        }

        /// <summary>
        /// Changes the color of the creature based on the selection state.
        /// </summary>
        /// <param name="isSelected">
        /// Whether the creature is selected.
        /// </param>
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

        /// <summary>
        /// Moves the creature to the given position.
        /// </summary>
        /// <remarks>
        /// The creature is also rotated based on the movement vector.
        /// </remarks>
        /// <param name="newPosition">
        /// The new position of the creature.
        /// </param>
        public void MoveTo(Vector2 newPosition)
        {
            var oldPosition3D = Position;
            var oldPosition2D = new Vector2(oldPosition3D.X, oldPosition3D.Z);
            var movementVector = newPosition - oldPosition2D;

            Rotate(movementVector.Angle());
            Position += new Vector3(movementVector.X, 0, movementVector.Y);
        }

        /// <summary>
        /// Changes the color of the creature.
        /// </summary>
        /// <param name="color">
        /// The new color of the creature.
        /// </param>
        private void ChangeColor(Color color)
        {
            var material = _mesh3D.GetSurfaceOverrideMaterial(0) as StandardMaterial3D;
            material.AlbedoColor = color;
            _currentColor = color;
        }

        /// <summary>
        /// Rotates the creature to the given angle.
        /// </summary>
        /// <param name="angle">
        /// The angle to rotate to.
        /// </param>
        private void Rotate(float angle)
        {
            Rotation = new Vector3(0, -angle, 0);
        }
    }
}