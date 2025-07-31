using System.Collections.Generic;
using System.IO;
using UnityEngine;

//
//
//
//  tutorial used: https://youtu.be/aUi9aijvpgs?si=Ncl9ZEXvetpY1vYi
//
//

public class SaveSystem : MonoBehaviour
{
    private string savePath;
    private int resetActivated;

    void Start()
    {
        savePath = Application.persistentDataPath + "/savegame.json";
        Debug.Log("Path: " + savePath);

        if (PlayerPrefs.GetInt("ResetActivated", resetActivated) == 1)
        {
            PlayerPrefs.SetInt("ResetActivated", 0);
            return;
        }
        else
        {
            LoadGame();
            
        }
        

    }

    public void SaveGame()
    {
        GameData data = new GameData();

        // Save game time.
        GameManager gameManager = FindObjectOfType<GameManager>();
        data.time = gameManager.time;
        data.UnlockedTypes = gameManager.UnlockedSaplingTypes;
        

        // Save money and wood.
        MoneyManager moneyManager = FindObjectOfType<MoneyManager>();
        data.money = moneyManager.money;
        data.wood = moneyManager.wood;
        

        // Save plots and trees.
        data.plots = new List<PlotData>();

        foreach (Plot plot in FindObjectsOfType<Plot>())
        {

            // Save relevant data of plot.
            PlotData plotData = new PlotData();
            plotData.id = plot.gameObject.name;
            plotData.position = plot.transform.position;
            plotData.isOwned = plot.GetComponent<TagManager>().HasTag("isOwned");
            plotData.occupiedCells = new List<bool>(); // I thought that that's not needed but it seems like it is...
            plotData.burningTrees = new List<bool>();
            plotData.timeOfPlanting = new List<double>();
            plotData.typeOfTree = new List<int>();


            foreach (Transform cell in plot.transform)
            {
                bool occupied = cell.GetComponent<TagManager>().HasTag("Occupied"); // Saves occupied tag.
                bool burning = cell.GetComponent<TagManager>().HasTag("OnFire"); // Saves burning state.
                plotData.burningTrees.Add(burning);
                plotData.occupiedCells.Add(occupied);
                

                GrowthOfTree tree = cell.GetComponentInChildren<GrowthOfTree>(); // Saves for each tree in cells of plot the timeOfPlanting.
                if (tree != null)
                {
                    plotData.timeOfPlanting.Add(tree.timeOfPlanting);

                    plotData.typeOfTree.Add(tree.saveTypeIdentifier);
                    //Debug.Log(tree.saveTypeIdentifier);
                    //Debug.Log(tree.transform.parent.gameObject.name);
                    
                }
                else
                {
                    plotData.timeOfPlanting.Add(0); // Adds 0 when the cell is not occupied and thus tree == null.
                    plotData.typeOfTree.Add(0);
                    //Debug.Log("nothing: " + tree.transform.parent.gameObject.name);
                }
                
                
            }

            data.plots.Add(plotData); // Adds it all to the list.
        }

        // Save to JSON file. (persistent data)
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("game saved");
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath)) // Later of use when we have the home screen.
        {
            Debug.Log("No save file");
            return;
        }

        string json = File.ReadAllText(savePath);
        GameData data = JsonUtility.FromJson<GameData>(json);

        // Load game time.
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.timeOfLastSession = data.time;
        gameManager.UnlockedSaplingTypes = data.UnlockedTypes;
        foreach (var x in gameManager.UnlockedSaplingTypes)
        {
            Debug.Log(x.ToString());
        }


        // Load money and wood.
        MoneyManager moneyManager = FindObjectOfType<MoneyManager>();
        moneyManager.money = data.money;
        moneyManager.wood = data.wood;
        

        // Load plots and trees.
        foreach (PlotData plotData in data.plots)
        {
            GameObject plot = GameObject.Find(plotData.id);

            if (plot == null)
            {
                plot = Instantiate(Resources.Load<GameObject>("Prefabs/HoneyCombPlot"));
                plot.name = plotData.id;
                plot.transform.position = plotData.position;
            }

            if (plotData.isOwned)
            {
                plot.GetComponent<TagManager>().AddTag("isOwned");
                FindObjectOfType<LandManager>().ColorOfFrame(plot);
            }

            int i = 0;
            foreach (Transform cell in plot.transform)
            {
                if (i < plotData.occupiedCells.Count && plotData.occupiedCells[i])
                {
                    cell.GetComponent<TagManager>().AddTag("Occupied");
                    

                    GrowthOfTree tree = cell.transform.GetChild(0).GetComponent<GrowthOfTree>();
                    //Debug.Log("count: " + plotData.occupiedCells.Count);
                    //Debug.Log("i: " + i);
                    tree.timeOfPlanting = plotData.timeOfPlanting[i];
                    tree.saveTypeIdentifier = plotData.typeOfTree[i];
                    tree.gameObject.SetActive(true);
                    if (plotData.burningTrees[i])
                    {
                        cell.GetComponent<TagManager>().AddTag("OnFire");
                        cell.transform.GetChild(1).gameObject.SetActive(true);
                    }
                    
                   


                }
                i++;
            }
        }

        Debug.Log("Game Loaded.");
    }
}


