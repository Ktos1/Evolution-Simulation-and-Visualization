using Godot;
using MathNet.Numerics.Random;
using System;

public partial class PlantSceneObject : StaticBody3D
{
    [Export] private Node3D _nest;

    private static Random _randGen = new Random();
    private MeshInstance3D[] _fruits = new MeshInstance3D[3];
    private int _fruitNumber = 0;

    private int FruitNumber
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
    // this probably is not needed any longer
    //public void SetFruitNumber(int fruitNumber)
    //{
    //    foreach (var fruit in _fruits)
    //    {
    //        fruit.Visible = false;
    //    }
            
    //    FruitNumber = 0;
    //    while (FruitNumber !=  fruitNumber)
    //    {
    //        AddFruit();
    //    }
    //}

    public void SubstractFruit()
    {
        _fruits[--FruitNumber].Visible = false;
    }

    public void AddFruit()
    {
        _fruits[FruitNumber++].Visible = true;
    }

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
