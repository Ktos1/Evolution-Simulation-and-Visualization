using Godot;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Visualization;
using System;

public partial class GenesWindow : PanelContainer
{
    [Export] private VBoxContainer _genesVBox;

    public void OnClickedOnCreature(object sender, EventArgs e)
    {
        Visible = true;
        Clear();
        var creature = (VCreature)sender;
        var propertiesInfo = creature.Chromosome.GetType().GetProperties();
        foreach (var property in propertiesInfo)
        {
            if (property.PropertyType == typeof(VGene))
            {
                var geneRow = Prefabs.GeneInfoRow.Instantiate() as GeneInfoRow;
                _genesVBox.CallDeferred("add_child", geneRow);
                geneRow.SetGeneData(property.Name, property.GetValue(creature.Chromosome) as Gene);
            }
        }
    }

    private void Clear()
    {
        foreach (var child in _genesVBox.GetChildren())
        {
            child.QueueFree();
        }
    }
}
