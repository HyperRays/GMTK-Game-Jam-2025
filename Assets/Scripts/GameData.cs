using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // Special attribut that translates? following data/class to a medium that can be handled by json files and others to save and load.
public class GameData // Saves overall values of the game (trees, plots, usw.)
{
    public double time;
    public int money;
    public int wood;
    public List<TreeData> trees;
    public List<PlotData> plots;
    public List<string> UnlockedTypes;
}

[Serializable]
public class TreeData // Saves tree specific values.
{
    //public string id;
    //public Vector3 position;
    
    
}

[Serializable]
public class PlotData // Saves plot specific values.
{
    public string id;
    public bool isOwned;
    public Vector3 position;
    public List<bool> occupiedCells;
    public List<bool> burningTrees;
    public List<int> typeOfTree;
    public List<double> timeOfPlanting;
}

