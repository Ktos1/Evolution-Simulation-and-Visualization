using Godot;
using ProjectEvolution.CommonStuff;

public partial class GeneInfoRow : PanelContainer
{
    [Export] private Label _geneNameLabel;
    [Export] private Label _geneValueLabel;
    [Export] private HSlider _geneValueSlider;
    [Export] private ColorRect _diffRect;

    public void SetGeneData(string name, Gene gene, float averageStartValue)
    {
        // Important: Position property tooltip is true only when layout mode is set on position.
        // If anchors are set then Position property is related to the anchor and the position section
        // in inspector in editor is not match to the Position property in code, because the position in
        // editor is related to the top-left corner.
        var geneValue = gene.Value;
        var geneMaxValue = gene.MaxValue;
        var geneMinValue = gene.MinValue;
        var sliderWidth = _geneValueSlider.Size.X - 16;
        var geneRange = geneMaxValue - geneMinValue;
        var shiftedGeneValue = geneValue - geneMinValue;
        var sliderFillness = shiftedGeneValue / geneRange;
        

        _geneNameLabel.Text = name;
        _geneValueSlider.MinValue = geneMinValue;
        _geneValueSlider.MaxValue = geneMaxValue;
        _geneValueSlider.Value = geneValue;
        _geneValueLabel.Text = geneValue.ToString("F2");

        // The value label positioning.
        var labelPosition = _geneValueLabel.Position;
        labelPosition.X = _geneValueSlider.Position.X + 8 - _geneValueLabel.Size.X/2;
        _geneValueLabel.Position = labelPosition;
        _geneValueLabel.Position += new Vector2(sliderWidth * sliderFillness, 0);

        // Setting a ColorRect from the average start value to the gene value
        var shiftedAverage = averageStartValue - geneMinValue;
        if (geneValue > averageStartValue)
        {
            _diffRect.Position = new Vector2((shiftedAverage - geneRange / 2) / geneRange * sliderWidth, -4);
            _diffRect.Size += new Vector2((shiftedGeneValue - shiftedAverage) / geneRange * sliderWidth, 0);
        }
        else
        {
            _diffRect.Position = new Vector2((shiftedGeneValue - geneRange / 2) / geneRange * sliderWidth, -4);
            _diffRect.Size += new Vector2((shiftedAverage - shiftedGeneValue) / geneRange * sliderWidth, 0);
        }
    }
}
