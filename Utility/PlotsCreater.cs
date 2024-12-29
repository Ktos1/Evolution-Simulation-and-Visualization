using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Utility.BinarySerialization;
using System.Collections.Generic;
using System.IO;
using Godot;
using System.Linq;
using MathNet.Numerics.Statistics;
using OxyPlot.Legends;
using ProjectEvolution.Simulation;

/// <summary>
/// Contains the classes provided some utility functions.
/// </summary>
namespace ProjectEvolution.Utility
{
    /// <summary>
    /// Represents the class used to create the plots.
    /// </summary>
    internal static class PlotsCreater
    {
        /// <summary>
        /// Creates all the needed plots based on the ticks data transfer objects.
        /// </summary>
        /// <param name="ticksDTOs">
        /// The array of the all ticks data transfer objects for the simulation.
        /// </param>
        public static void CreatePlots(TickDTO[] ticksDTOs)
        {
            var genesNumber = ticksDTOs[0].CreaturesData[0].Genes.Length;
            var diffSumPoints = new List<DataPoint>();
            var diffPoints = new List<DataPoint>[genesNumber];
            var stdDevSumPoints = new List<DataPoint>();
            var stdDevPoints = new List<DataPoint>[genesNumber];
            for (int i = 0; i < genesNumber; i++)
            {
                diffPoints[i] = new List<DataPoint>();
                stdDevPoints[i] = new List<DataPoint>();
            }

            var creaturesPopulPoints = new List<DataPoint>();
            var plantsPopulPoints = new List<DataPoint>();
            int year = 0;
            var creaturesData = new List<CreatureDTO>();
            var oldGeneValues = new float[genesNumber];
            var avgGeneValues = new float[genesNumber];
            var stdDevValues = new float[genesNumber];

            for (int i = 0; i < ticksDTOs.Length; i++)
            {
                foreach (var creatureData in ticksDTOs[i].CreaturesData)
                {
                    if (creatureData.Genes != null) creaturesData.Add(creatureData);
                    else if (creatureData.State == CreatureStates.ToDelete)
                    {
                        var creatureToDelete = creaturesData.Find(
                            creature => creature.ID == creatureData.ID);
                        creaturesData.Remove(creatureToDelete);
                    }   
                }

                if (i % 500 == 0 || i == 0)
                {
                    if (creaturesData.Any())
                    {
                        var creaturesNumber = creaturesData.Count;
                        var scaledCreaturesGenes = new List<float>[genesNumber];
                        for (var j = 0; j < genesNumber; j++)
                        {
                            scaledCreaturesGenes[j] = new List<float>();
                        }
                        var auxChromosome = new SChromosome();
                        foreach (var creature in creaturesData)
                        {
                            for (var j = 0; j < genesNumber; j++)
                            {
                                var geneClass = auxChromosome.Genes[j];
                                var geneRange = geneClass.MaxValue - geneClass.MinValue;
                                var shiftedGeneValue = creature.Genes[j] - geneClass.MinValue;
                                var scaledCreatureGene = shiftedGeneValue * (100f / geneRange);
                                scaledCreaturesGenes[j].Add(scaledCreatureGene);
                            }
                        }
                        for (var j = 0; j < genesNumber; j++)
                        {
                            avgGeneValues[j] = scaledCreaturesGenes[j].Average();
                            stdDevValues[j] = (float)scaledCreaturesGenes[j].PopulationStandardDeviation();
                        }

                        // diffs plot stuff
                        if (i != 0)
                        {
                            var diffsSum = 0f;
                            for (var j = 0; j < genesNumber; j++)
                            {
                                var geneDiff = Mathf.Abs(avgGeneValues[j] - oldGeneValues[j]);
                                diffPoints[j].Add(new DataPoint(year, geneDiff));
                                diffsSum += geneDiff;
                            }
                            diffSumPoints.Add(new DataPoint(year, diffsSum));
                        }
                        for (var j = 0; j < genesNumber; j++)
                        {
                            oldGeneValues[j] = avgGeneValues[j];
                        }

                        // standard deviation plot stuff
                        var stdDevSum = 0f;
                        for (var j = 0; j < genesNumber; j++)
                        {
                            stdDevPoints[j].Add(new DataPoint(year, stdDevValues[j]));
                            stdDevSum += stdDevValues[j];
                        }
                        stdDevSumPoints.Add(new DataPoint(year, stdDevSum));
                    }
                    else
                    {
                        diffSumPoints.Add(new DataPoint(year, 0));
                        stdDevSumPoints.Add(new DataPoint(year, 0));
                        for (var j = 0; j < genesNumber; j++)
                        {
                            diffPoints[j].Add(new DataPoint(year, 0));
                            stdDevPoints[j].Add(new DataPoint(year, 0));
                        }
                    }

                    // population plot stuff
                    creaturesPopulPoints.Add(new DataPoint(year, creaturesData.Count));
                    plantsPopulPoints.Add(new DataPoint(year, ticksDTOs[i].PlantsNumber));

                    year++;
                }
            }

            // common plots
            var title = "Suma zmian uśrednionych genów w stosunku do poprzedniego roku";
            var diffsSumPlot = CreateLinePlot(title, "Lata", "Suma", diffSumPoints);

            title = "Suma odchyleń standardowych poszczególnych genów";
            var stdDevSumPlot = CreateLinePlot(title, "Lata", "Suma", stdDevSumPoints);

            title = "Liczebność populacji stworzeń i roślin";
            var seriesTitles = new string[] { "Stworzenia      ", "Rośliny" };
            var populationsPlot = CreateLinePlot(title, "Lata", "Liczebność",
                new List<List<DataPoint>> { creaturesPopulPoints, plantsPopulPoints }, 
                seriesTitles);
            var legend = new Legend
            {
                LegendPosition = LegendPosition.TopCenter,
                LegendPlacement = LegendPlacement.Inside,
                LegendOrientation = LegendOrientation.Vertical,
                LegendLineSpacing = 5,
                LegendBackground = OxyColor.Parse("#cecece"),
                LegendBorder = OxyColors.Black
            };
            populationsPlot.Legends.Add(legend);

            if (!Directory.Exists("Plots")) Directory.CreateDirectory("Plots");
            ExportPlotToSVG("diffsSumPlot.svg", diffsSumPlot);
            ExportPlotToSVG("stdDevSumPlot.svg", stdDevSumPlot);
            ExportPlotToSVG("populationsPlot.svg", populationsPlot);

            // plots for specified genes
            var genesNames = Chromosome<SGene>.GetGenesNames();
            for (int i = 0; i < genesNumber; i++)
            {
                var geneName = genesNames[i];
                var polishGeneName = Gene.TranslateNameToPolish(geneName);
                // name here should be from translation to polish function.
                var diffsPlot = CreateLinePlot($"{polishGeneName} - zmiany średniej z całej populacji",
                    "Lata", "Procent", diffPoints[i]);
                var stdDevPlot = CreateLinePlot($"{polishGeneName} - odchylenie standardowe",
                    "Lata", "Odchylenie", stdDevPoints[i]);

                ExportPlotToSVG($"{geneName}Diffs.svg", diffsPlot);
                ExportPlotToSVG($"{geneName}StdDev.svg", stdDevPlot);
            }
        }

        /// <summary>
        /// Creates the line plot with the specified parameters for one data series.
        /// </summary>
        /// <param name="title">
        /// The title of the plot.
        /// </param>
        /// <param name="xTitle">
        /// The title of the x-axis.
        /// </param>
        /// <param name="yTitle">
        /// The title of the y-axis.
        /// </param>
        /// <param name="dataSeries">
        /// The data series for the plot to display.
        /// </param>
        /// <returns>
        /// The created plot model.
        /// </returns>
        private static PlotModel CreateLinePlot(
            string title,
            string xTitle,
            string yTitle,
            List<DataPoint> dataSeries)
        {
            return CreateLinePlot(title, xTitle, yTitle, new List<List<DataPoint>> { dataSeries });
        }

        /// <summary>
        /// Creates the line plot with the specified parameters for multiple data series.
        /// </summary>
        /// <param name="title">
        /// The title of the plot.
        /// </param>
        /// <param name="xTitle">
        /// The title of the x-axis.
        /// </param>
        /// <param name="yTitle">
        /// The title of the y-axis.
        /// </param>
        /// <param name="dataSeries">
        /// The data series for the plot to display.
        /// </param>
        /// <param name="dataSeriesTitles">
        /// The titles of the data series. Used for the legend.
        /// </param>
        /// <returns>
        /// The created plot model.
        /// </returns>
        private static PlotModel CreateLinePlot(
            string title,
            string xTitle,
            string yTitle,
            List<List<DataPoint>> dataSeries,
            string[] dataSeriesTitles = null)
        {
            var plotModel = new PlotModel
            {
                Title = title,
                DefaultFont = "Arial",
                TitlePadding = 0,
            };

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = xTitle,
                TitleFontSize = 15,
                TitleFontWeight = FontWeights.Bold,
                AxisTitleDistance = 11,
                MaximumPadding = 0.01,
                MinimumPadding = 0.01,
                IntervalLength = 25,
                AxisTickToLabelDistance = -0,
            });

            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = yTitle,
                TitleFontSize = 15,
                TitleFontWeight = FontWeights.Bold,
                AxisTitleDistance = 15,
                MaximumPadding = 0.01,
                IntervalLength = 25,
            });

            for (int i = 0; i < dataSeries.Count; i++)
            {
                var dataSerie = dataSeries[i];
                var lineSerie = new LineSeries {
                    ItemsSource = dataSerie,
                    Title = (dataSeriesTitles != null) ? dataSeriesTitles[i] : null,
                    };
                plotModel.Series.Add(lineSerie);
            }
            return plotModel;
        }

        /// <summary>
        /// Exports the plot model to the SVG file.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file to save the plot.
        /// </param>
        /// <param name="plotModel">
        /// The plot model to save.
        /// </param>
        private static void ExportPlotToSVG(string fileName, PlotModel plotModel)
        {
            using (var memoryStream = new MemoryStream())
            {
                var exporter = new SvgExporter { Width = 800, Height = 450 };
                exporter.Export(plotModel, memoryStream);

                memoryStream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(memoryStream))
                {
                    string svgContent = reader.ReadToEnd();
                    svgContent = svgContent
                        .Replace("dominant-baseline=\"hanging\"", "dy=\"6\"")
                        .Replace("dominant-baseline=\"middle\"", "dy=\"2\"");

                    File.WriteAllText($"Plots/{fileName}", svgContent);
                }
            }
        }
    }
}
