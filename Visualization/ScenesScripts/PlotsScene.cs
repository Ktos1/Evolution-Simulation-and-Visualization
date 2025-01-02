using Godot;
using SkiaSharp;
using System.IO;
using Svg.Skia;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Simulation;


namespace ProjectEvolution.Visualization.ScenesScripts
{
    /// <summary>
    /// Represents the plots scene.
    /// </summary>
    public partial class PlotsScene : Control
    {
        /// <summary>
        /// The tab container for the common plots.
        /// </summary>
        [Export] private TabContainer _commonTabContainer;
        /// <summary>
        /// The tab container for the differents plots.
        /// </summary>
        [Export] private TabContainer _diffsTabContainer;
        /// <summary>
        /// The tab container for the std dev plots.
        /// </summary>
        [Export] private TabContainer _stdDevTabContainer;
        /// <summary>
        /// The tab container for the averages values plots.
        /// </summary>
        [Export] private TabContainer _avgsTabContainer;
        /// <summary>
        /// The button to back to the visualization scene.
        /// </summary>
        [Export] private Button _backButton;

        /// <summary>
        /// The visualization scene.
        /// </summary>
        private VisualizationScene _visualScene;

        /// <summary>
        /// Initializes the plots scene.
        /// </summary>
        /// <remarks>
        /// Firstly scales the svg plots and converts them to png.
        /// Then loads them into the tab containers.
        /// </remarks>
        /// <param name="visualizationScene">
        /// The visualization scene.
        /// </param>
        public void Initialize(VisualizationScene visualizationScene)
        {
            _visualScene = visualizationScene;
            _backButton.Pressed += OnBackButtonPress;

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

            var genesNames = Chromosome<SGene>.GetGenesNames();
            for (int i = 0; i < genesNames.Length; i++)
            {
                var geneName = genesNames[i];
                ScaleSVG($"{geneName}Diffs");
                ScaleSVG($"{geneName}StdDev");
                ScaleSVG($"{geneName}Avgs");
                var diffImage = Image.LoadFromFile($"Plots/{geneName}Diffs.png");
                var stdDevImage = Image.LoadFromFile($"Plots/{geneName}StdDev.png");
                var avgsImage = Image.LoadFromFile($"Plots/{geneName}Avgs.png");

                var polishGeneName = Gene.TranslateNameToPolish(geneName);
                _diffsTabContainer.AddChild(new TextureRect()
                {
                    Texture = ImageTexture.CreateFromImage(diffImage),
                    ExpandMode = TextureRect.ExpandModeEnum.FitWidth,
                    Name = $"{polishGeneName}"
                });
                _stdDevTabContainer.AddChild(new TextureRect()
                {
                    Texture = ImageTexture.CreateFromImage(stdDevImage),
                    ExpandMode = TextureRect.ExpandModeEnum.FitWidth,
                    Name = $"{polishGeneName}"
                });
                _avgsTabContainer.AddChild(new TextureRect()
                {
                    Texture = ImageTexture.CreateFromImage(avgsImage),
                    ExpandMode = TextureRect.ExpandModeEnum.FitWidth,
                    Name = $"{polishGeneName}"
                });
            }
        }

        /// <summary>
        /// Scales the svg plot and converts it to png.
        /// </summary>
        /// <param name="filename">
        /// The filename of the svg plot without the extension.
        /// </param>
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

        /// <summary>
        /// Handles the Back button press.
        /// </summary>
        /// <remarks>
        /// Changes the scene to the visualization scene.
        /// </remarks>
        private void OnBackButtonPress()
        {
            var root = GetTree().Root;
            root.RemoveChild(this);
            root.AddChild(_visualScene);
        }
    }
}
