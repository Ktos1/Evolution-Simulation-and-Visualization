using Godot;
using System;

namespace ProjectEvolution.Visualization.ScenesElementsScripts
{
    /// <summary>
    /// Script attached to the plant static body.
    /// </summary>
    /// <remarks>
    /// This class inherited from <see cref="StaticBody3D"/> godot class and 
    /// represents directly the plant object on the visualization scene.
    /// </remarks>
    public partial class PlantStaticBody : StaticBody3D
    {
        /// <summary>
        /// The node which is the point of reference for the fruits positions.
        /// </summary>
        [Export] private Node3D _nest;
        
        /// <summary>
        /// The array of fruits meshes.
        /// </summary>
        private MeshInstance3D[] _fruits = new MeshInstance3D[3];
        /// <summary>
        /// The current number of fruits on the plant.
        /// </summary>
        private int _fruitNumber = 0;

        /// <summary>
        /// Gets or sets the number of fruits on the plant.
        /// </summary>
        public int FruitNumber
        {
            get { return _fruitNumber; }
            set
            {
                if (value < 0 || value > 3)
                {
                    throw new ArgumentOutOfRangeException(
                        "value",
                        "Plants can have only 0-3 fruits"
                        );
                }
                else
                {
                    _fruitNumber = value;
                }
            }
        }

        /// <summary>
        /// Initializes the plant.
        /// </summary>
        /// <param name="fruitNumber">
        /// The number of fruits on the plant.
        /// </param>
        /// <param name="fruitsPositions">
        /// The positions of the fruits.
        /// </param>
        public void Initialize(int fruitNumber, Vector3[] fruitsPositions)
        {
            FruitNumber = fruitNumber;
            for (int i = 0; i < _fruits.Length; i++)
            {
                var fruit = InstantiateFruitMesh();
                _nest.AddChild(fruit);
                _fruits[i] = fruit;

                fruit.Position = fruitsPositions[i];

                if (i >= fruitNumber) fruit.Visible = false;
            }
        }

        /// <summary>
        /// Substracts a fruit from the plant.
        /// </summary>
        public void SubstractFruit()
        {
            _fruits[--FruitNumber].Visible = false;
        }

        /// <summary>
        /// Adds a fruit to the plant.
        /// </summary>
        public void AddFruit()
        {
            _fruits[FruitNumber++].Visible = true;
        }

        /// <summary>
        /// Instantiates a fruit mesh.
        /// </summary>
        /// <returns>
        /// The fruit mesh.
        /// </returns>
        private MeshInstance3D InstantiateFruitMesh()
        {
            var mesh = new MeshInstance3D();

            var specifiedMesh = new BoxMesh();
            specifiedMesh.Size = new Vector3(0.2f, 0.2f, 0.2f);
            mesh.Mesh = specifiedMesh;

            var material = new StandardMaterial3D();
            material.AlbedoColor = new Color("#FF0000");
            mesh.SetSurfaceOverrideMaterial(0, material);
            return mesh;
        }
    }
}