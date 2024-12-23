using Godot;
using SkiaSharp;
using System.IO;
using Svg.Skia;
using ProjectEvolution.Visualization;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Simulation.Algorithm;
using System.Collections.Generic;
using ProjectEvolution.Utility;
using ProjectEvolution.Utility.BinarySerialization;

public partial class PlotsScene : Control
{
    [Export] private TabContainer _commonTabContainer;
    [Export] private TabContainer _diffsTabContainer;
    [Export] private TabContainer _stdDevTabContainer;

    [Export] private Button _backButton;

    private Visualization _visualScene;

    public void Initialize(Visualization visualizationScene)
    {
        _visualScene = visualizationScene;
        _backButton.Pressed += OnBackButtonPress;

        if (!Directory.Exists("Plots")) PlotsCreater.CreatePlots(BinReader.TickDTOs);
        ScaleSVG("diffsSumPlot");
        ScaleSVG("stdDevSumPlot");
        ScaleSVG("populationsPlot");

        var image = Image.LoadFromFile($"Plots/diffsSumPlot.png");
        _commonTabContainer.AddChild(new TextureRect() 
        { 
            Texture = ImageTexture.CreateFromImage(image),
            ExpandMode = TextureRect.ExpandModeEnum.FitWidth,
            Name = "Suma zmian uśrednionych genów"
        });
        image = Image.LoadFromFile($"Plots/stdDevSumPlot.png");
        _commonTabContainer.AddChild(new TextureRect()
        {
            Texture = ImageTexture.CreateFromImage(image),
            ExpandMode = TextureRect.ExpandModeEnum.FitWidth,
            Name = "Suma odchyleń standardowych poszczególnych genów"
        });
        image = Image.LoadFromFile($"Plots/populationsPlot.png");
        _commonTabContainer.AddChild(new TextureRect()
        {
            Texture = ImageTexture.CreateFromImage(image),
            ExpandMode = TextureRect.ExpandModeEnum.FitWidth,
            Name = "Populacje"
        });

        var propertiesInfo = typeof(Chromosome<SGene>).GetProperties();
        var geneNames = new List<string>();
        for (int i = 0; i < propertiesInfo.Length; i++)
        {
            if (propertiesInfo[i].PropertyType == typeof(SGene))
                geneNames.Add(propertiesInfo[i].Name);
        }

        for (int i = 0; i < geneNames.Count; i++)
        {
            var geneName = geneNames[i];
            ScaleSVG($"{geneName}Diffs");
            ScaleSVG($"{geneName}StdDev");
            var diffImage = Image.LoadFromFile($"Plots/{geneName}Diffs.png");
            var stdDevImage = Image.LoadFromFile($"Plots/{geneName}StdDev.png");
            _diffsTabContainer.AddChild(new TextureRect()
            {
                Texture = ImageTexture.CreateFromImage(diffImage),
                ExpandMode = TextureRect.ExpandModeEnum.FitWidth,
                Name = $"{geneName}"
            });
            _stdDevTabContainer.AddChild(new TextureRect()
            {
                Texture = ImageTexture.CreateFromImage(stdDevImage),
                ExpandMode = TextureRect.ExpandModeEnum.FitWidth,
                Name = $"{geneName}"
            });
        }
    }

    private void ScaleSVG(string filename)
    {
        using var stream = File.OpenRead($"Plots/{filename}.svg");
        var svg = new SKSvg();
        svg.Load(stream);

        float scaleFactor = _visualScene.GetTree().Root.Size.X / 1152f;

        var originalSize = svg.Picture.CullRect.Size;
        var scaledWidth = (int)(originalSize.Width * scaleFactor);
        var scaledHeight = (int)(originalSize.Height * scaleFactor);

        using var surface = SKSurface.Create(new SKImageInfo(scaledWidth, scaledHeight));
        var canvas = surface.Canvas;
        canvas.Scale(scaleFactor);
        canvas.DrawPicture(svg.Picture);
        canvas.Flush();

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        File.WriteAllBytes($"Plots/{filename}.png", data.ToArray());
    }

    private void OnBackButtonPress()
    {
        var root = GetTree().Root;
        root.RemoveChild(this);
        root.AddChild(_visualScene);
    }
}
