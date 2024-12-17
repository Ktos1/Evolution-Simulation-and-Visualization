using Godot;
using SkiaSharp;
using System.IO;
using Svg.Skia;
using ProjectEvolution.Visualization;

public partial class PlotsScene : Control
{
    [Export] private TextureRect _plotDisplay;
    [Export] private TabBar _plotsTabBar;
    [Export] private Button _backButton;

    private Visualization _visualScene;

    public void Initialize(Visualization visualizationScene)
    {
        _visualScene = visualizationScene;
        _plotsTabBar.TabChanged += OnPlotChange;
        _backButton.Pressed += OnBackButtonPress;

        ScaleSVG("diffsPlot");
        ScaleSVG("populationsPlot");
        ScaleSVG("stdDevPlot");
        var image = Image.LoadFromFile($"Plots/diffsPlot.png");
        _plotDisplay.Texture = ImageTexture.CreateFromImage(image);
    }

    private void ScaleSVG(string filename)
    {
        using var stream = File.OpenRead($"Plots/{filename}.svg");
        var svg = new SKSvg();
        svg.Load(stream);

        float scaleFactor = _plotDisplay.Size.X / 800f;

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

    private void OnPlotChange(long tab)
    {
        Image image = null;
        switch (tab)
        {
            case 0:
                image = Image.LoadFromFile($"Plots/diffsPlot.png");
                break;
            case 1:
                image = Image.LoadFromFile($"Plots/stdDevPlot.png");
                break;
            case 2:
                image = Image.LoadFromFile($"Plots/populationsPlot.png");
                break;
        }
        _plotDisplay.Texture = ImageTexture.CreateFromImage(image);
    }

    private void OnBackButtonPress()
    {
        var root = GetTree().Root;
        root.RemoveChild(this);
        root.AddChild(_visualScene);
    }
}
