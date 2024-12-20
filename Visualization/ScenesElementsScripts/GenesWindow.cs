using Godot;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Visualization;

public partial class GenesWindow : PanelContainer
{
    [Export] private VBoxContainer _genesVBox;

    private VCreature _displayedCreature;

    public float[] AverageStartGenesValues { get; set; }

    public void OnClickOnCreature(VCreature creature)
    {
        if (_displayedCreature != null) 
            _displayedCreature.Deleted -= OnCreatureDeletion;
        _displayedCreature = creature;
        _displayedCreature.Deleted += OnCreatureDeletion;

        Visible = true;
        Clear();
        var propertiesInfo = creature.Chromosome.GetType().GetProperties();
        int index = 0;
        for (int i = 0; i < propertiesInfo.Length; i++)
        {
            if (propertiesInfo[i].PropertyType == typeof(VGene))
            {
                var geneRow = Prefabs.GeneInfoRow.Instantiate() as GeneInfoRow;
                _genesVBox.CallDeferred("add_child", geneRow);
                geneRow.SetGeneData(
                    propertiesInfo[i].Name, 
                    propertiesInfo[i].GetValue(creature.Chromosome) as Gene, 
                    AverageStartGenesValues[index]
                    );
                index++;
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

    private void OnCreatureDeletion(VCreature creature)
    {
        Visible = false;
    }
}
