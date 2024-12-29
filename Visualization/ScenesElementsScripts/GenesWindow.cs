using Godot;
using ProjectEvolution.CommonStuff;
using ProjectEvolution.Visualization.LogicScripts;
using ProjectEvolution.Visualization.ScenesStorages;

namespace ProjectEvolution.Visualization.ScenesElementsScripts
{
    /// <summary>
    /// Represents the genes window in the visualization scene.
    /// </summary>
    public partial class GenesWindow : PanelContainer
    {
        /// <summary>
        /// The VBoxContainer containing the genes info rows.
        /// </summary>
        [Export] private VBoxContainer _genesVBox;

        /// <summary>
        /// The creature whose genes are currently displayed.
        /// </summary>
        private VCreature _displayedCreature;

        /// <summary>
        /// The average start values of the genes.
        /// </summary>
        public float[] AverageStartGenesValues { get; set; }

        /// <summary>
        /// Handles the click on a creature.
        /// </summary>
        /// <param name="creature">
        /// The creature that was clicked on.
        /// </param>
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
                        Gene.TranslateNameToPolish(propertiesInfo[i].Name),
                        propertiesInfo[i].GetValue(creature.Chromosome) as Gene,
                        AverageStartGenesValues[index]
                        );
                    index++;
                }
            }
        }

        /// <summary>
        /// Deletes all the genes info rows.
        /// </summary>
        private void Clear()
        {
            foreach (var child in _genesVBox.GetChildren())
            {
                child.QueueFree();
            }
        }

        /// <summary>
        /// Handles the deletion of the displayed creature.
        /// </summary>
        /// <param name="creature">
        /// The creature that was deleted.
        /// </param>
        private void OnCreatureDeletion(VCreature creature)
        {
            Visible = false;
        }
    }
}