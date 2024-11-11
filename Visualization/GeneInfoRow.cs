using Godot;
using ProjectEvolution.CommonStuff;

public partial class GeneInfoRow : PanelContainer
{
    [Export] private Label _geneNameLabel;
    [Export] private Label _geneValueLabel;
    [Export] private HSlider _geneValueSlider;

    public void SetGeneData(string name, Gene gene)
    {
        _geneNameLabel.Text = name;
        _geneValueSlider.MinValue = gene.MinValue;
        _geneValueSlider.MaxValue = gene.MaxValue;
        _geneValueSlider.Value = gene.Value;
        _geneValueLabel.Text = gene.Value.ToString("F2");

        var sliderWidth = _geneValueSlider.Size.X - 16;
        var sliderFillness = gene.Value / gene.MaxValue;

        var labelPosition = _geneValueLabel.Position;
        labelPosition.X = _geneValueSlider.Position.X + 8 - _geneValueLabel.Size.X/2;
        _geneValueLabel.Position = labelPosition;

        _geneValueLabel.Position += new Vector2(sliderWidth * sliderFillness, 0);
    }
}
