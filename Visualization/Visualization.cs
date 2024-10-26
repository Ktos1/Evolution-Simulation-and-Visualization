using Godot;
using System.Linq;
using System.Collections.Generic;
using ProjectEvolution.Visualization;
using ProjectEvolution.Utility;
using ProjectEvolution.CommonStuff;

public partial class Visualization : Node3D
{
    private double _deltaCount = 0;
    private List<VCreature> _creatures = new List<VCreature>();

    List<VCreature> _deadCreatures = new List<VCreature>();

    public override void _Ready()
    {
        InitializeCreatures();
    }

    public override void _Process(double delta)
    {
        _deltaCount += delta;
        if (_deltaCount > CommonSettings.TICK_DURATION)
        {
            UpdateCreatures();
            _deltaCount -= CommonSettings.TICK_DURATION;
        }
        ProcessCreatures();
    }

    private void InitializeCreatures()
    {
        var creaturesData = JsonReader.NextTick();
        foreach (var creature in creaturesData)
        {
            var (position, state, chromosome) = creature;
            AddNewCreature(position, state, chromosome);
        }
    }

    private void UpdateCreatures()
    {
        var creaturesData = JsonReader.NextTick();
        for (int i = 0; i < creaturesData.Length; i++)
        {
            if (i < _creatures.Count)
            {
                var (position, state, _) = creaturesData[i];
                _creatures[i].Update(position, state);
                if (state == CreatureStates.Died)
                {
                    _deadCreatures.Add(_creatures[i]);
                }
            }
            else
            {
                var (spawnPosition, state, chromosome) = creaturesData[i];
                AddNewCreature(spawnPosition, state, chromosome);
            }
        }
        DeleteDeadCreatures();
    }

    private void DeleteDeadCreatures()
    {
        _deadCreatures.ForEach((deadCreature) => _creatures.Remove(deadCreature));
        _deadCreatures.Clear();
    }

    private void AddNewCreature(Vector2 spawnPosition, CreatureStates state, VChromosome chromosome)
    {
        _creatures.Add(new VCreature(spawnPosition, state, chromosome));
        CallDeferred("add_child", _creatures.Last().StaticBody);
    }

    private void ProcessCreatures()
    {
        _creatures.ForEach((creature) => creature.Process((float)_deltaCount));
    }


}
