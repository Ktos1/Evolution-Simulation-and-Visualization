using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Simulation.Algorithm;
using ProjectEvolution.Utility.BinarySerialization;
using System.Collections.Generic;
using System.IO;
using Godot;
using System.Linq;
using MathNet.Numerics.Statistics;
using OxyPlot.Legends;

namespace ProjectEvolution.Utility
{
    internal class PlotsCreater
    {
        private TickDTO[] _ticksDTOs;

        public PlotsCreater(TickDTO[] ticksDTO)
        {
            _ticksDTOs = ticksDTO;
        }

        public void CreatePlots()
        {
            var diffPoints = new List<DataPoint>();
            var stdDevPoints = new List<DataPoint>();
            var creaturesPopulPoints = new List<DataPoint>();
            var plantsPopulPoints = new List<DataPoint>();
            int year = 0;
            var creaturesData = new List<CreatureDTO>();
            var genesNumber = _ticksDTOs[0].CreaturesData[0].Genes.Length;
            var oldGeneValues = new float[genesNumber];
            var avgGeneValues = new float[genesNumber];
            var stdDevValues = new float[genesNumber];
            for (int i = 0; i < _ticksDTOs.Length; i++)
            {
                foreach (var creatureData in _ticksDTOs[i].CreaturesData)
                {
                    if (creatureData.Genes != null) creaturesData.Add(creatureData);
                    else if (creatureData.CurrentState == CreatureStates.ToDelete)
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
                                diffsSum += Mathf.Abs(avgGeneValues[j] - oldGeneValues[j]);
                            }
                            diffPoints.Add(new DataPoint(year, diffsSum));
                        }
                        for (var j = 0; j < genesNumber; j++)
                        {
                            oldGeneValues[j] = avgGeneValues[j];
                        }

                        // standard deviation plot stuff
                        var stdDevSum = 0f;
                        for (var j = 0; j < genesNumber; j++)
                        {
                            stdDevSum += stdDevValues[j];
                        }
                        stdDevPoints.Add(new DataPoint(year, stdDevSum));
                    }
                    else
                    {
                        diffPoints.Add(new DataPoint(year, 0));
                        stdDevPoints.Add(new DataPoint(year, 0));
                    }

                    // population plot stuff
                    creaturesPopulPoints.Add(new DataPoint(year, creaturesData.Count));
                    plantsPopulPoints.Add(new DataPoint(year, _ticksDTOs[i].PlantsNumber));

                    year++;
                }
            }

            var title = "Suma zmian uśrednionych genów w stosunku do poprzedniego roku";
            var diffsplot = CreateLinePlot(title, "Lata", "Suma", diffPoints.ToArray());

            title = "Suma odchyleń standardowych genów";
            var stdDevPlot = CreateLinePlot(title, "Lata", "Suma", stdDevPoints.ToArray());

            title = "Liczebność populacji stworzeń i roślin";
            var seriesTitles = new string[] { "Stworzenia      ", "Rośliny" };
            var populationsPlot = CreateLinePlot(title, "Lata", "Liczebność",
                new DataPoint[][] { creaturesPopulPoints.ToArray(), plantsPopulPoints.ToArray() }, 
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

            ExportPlotToSVG("diffsPlot.svg", diffsplot);
            ExportPlotToSVG("stdDevPlot.svg", stdDevPlot);
            ExportPlotToSVG("populationsPlot.svg", populationsPlot);
        }

        private PlotModel CreateLinePlot(
            string title,
            string xTitle,
            string yTitle,
            DataPoint[] dataSeries)
        {
            return CreateLinePlot(title, xTitle, yTitle, new DataPoint[1][] { dataSeries });
        }

        private PlotModel CreateLinePlot(
            string title,
            string xTitle,
            string yTitle,
            DataPoint[][] dataSeries,
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

            for (int i = 0; i < dataSeries.Length; i++)
            {
                DataPoint[] dataSerie = dataSeries[i];
                var lineSerie = new LineSeries {
                    ItemsSource = dataSerie,
                    Title = (dataSeriesTitles != null) ? dataSeriesTitles[i] : null,
                    };
                plotModel.Series.Add(lineSerie);
            }
            return plotModel;
        }

        private void ExportPlotToSVG(string fileName, PlotModel plotModel)
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
