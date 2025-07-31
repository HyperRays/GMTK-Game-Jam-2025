using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 

public class MarketSystem : MonoBehaviour
{
    private List<List<float>> allTypesPriceHistory = new List<List<float>>(); // List that has a priceHistory in every index for each type.
    public float[] allTypesCurrentPrices = new float[6] { 0.5f, 0.5f , 0.5f , 0.5f , 0.5f ,0.5f}; // Current prices for all types.
    public Material[] allTypesMaterials = new Material[5];
    private Color[] allTypesColors; // Colors for each line of the graph.
    

    public float[] rValues = new float[6] { 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f };
    public float fallMultiplicator = 0.7f;
    public float[] decreaseOdds = new float[6] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
    public float decreaseRise = 0.14f;
    public float decreaseFall = 0.3f;
    public int counter = 0;
    public int spikeCounter;


    public int S_che = 0; // cherry
    public int S_map = 1; // maple
    public int S_wis = 2; // wisteria
    public int S_gin = 3; // ginkgo
    public int S_oak = 4; // oak
    public int Wood = 5; // Wood of all

    private float priceRelativeStart = 0.5f; // Price of all types at start relative to min and max. So 0.5f is LowerLimit + 0.5f * (Upperlimit - LowerLimit)

    public float UpperPriceLimit; // Max price 200 for sapling, 100 for wood
    public float LowerPriceLimit; // lowest, 20 and 10


    public Image priceGraphImage;
    private Texture2D graphTexture;
    Color[] clearColors;
    private List<float> priceHistory = new List<float>();

    private int graphWidth = 600; // Resolution.
    private int graphHeight = 300;

    private int bandHeight;

    private int maxDataPoints = 50;
    private float updateInterval = 0.5f;
    private float timer = 0f;


    private GameManager gameManager;

    void Start()
    {
        allTypesColors = new Color[6] { allTypesMaterials[0].color, allTypesMaterials[1].color, allTypesMaterials[2].color, allTypesMaterials[3].color, allTypesMaterials[4].color, Color.red };
        

        for (int i = 0; i < 6; i++) 
        {
            allTypesPriceHistory.Add(new List<float>()); // creates the lists for priceHistory in allTypes list.
            
        }

        


        gameManager = FindObjectOfType<GameManager>();

        graphTexture = new Texture2D(graphWidth, graphHeight);
        priceGraphImage.sprite = Sprite.Create(graphTexture, new Rect(0, 0, graphWidth, graphHeight), new Vector2(0.5f, 0.5f));

        clearColors = new Color[graphWidth * graphHeight]; // Clears the Graph to update the values. Sets the Array to all black,
        for (int i = 0; i < clearColors.Length; i++)
            clearColors[i] = Color.black;

        graphTexture.SetPixels(clearColors);
    
        priceHistory.Add(priceRelativeStart); // initial value
    }

    void Update()
    {
        
        
        timer += Time.deltaTime;

        if (timer >= updateInterval) // So that the graph doesn't move too fast.
        {
            for (int i = 0; i < 6; i++)
            {
                UpdatePrice(i);
                UpdateGraphData(i);
                DrawGraph(i);
            }
            graphTexture.SetPixels(clearColors);
            timer = 0f;
        }
    }

    public int ReturnPrice(int typeIndex) // Calculates the int price between the ranges for the buttons.
    {
        return (int)(allTypesCurrentPrices[typeIndex] * (UpperPriceLimit - LowerPriceLimit) + LowerPriceLimit);
    }

    void UpdatePrice(int typeIndex) // Based on animal crossing stalk market system the prices between 0 and 1 are calculated and added to Current Prices and History
    {
        //Reset rValues and decreaseOdds
        if (counter == 0 || counter == 8)
        {
            counter = 0;
            rValues[typeIndex] = 0.05f;
            decreaseOdds[typeIndex] = 0.1f;
        }

        //Increase
        if (UnityEngine.Random.value > decreaseOdds[typeIndex])
        {
            allTypesCurrentPrices[typeIndex] *= (1f+ rValues[typeIndex]); //increase price by its rValue
            rValues[typeIndex] += 0.01f; //increase rValue
            decreaseOdds[typeIndex] += 0.14f; //increase decreaseOdds to make another price increase less likely
            counter += 1;
        }

        //Decrease
        else if (UnityEngine.Random.value < decreaseOdds[typeIndex])
        {
            allTypesCurrentPrices[typeIndex] *= fallMultiplicator; //Decrease prices
            decreaseOdds[typeIndex] *= decreaseFall; //Decrease decreaseOdds to make a price increase more likely
            counter += 1;
        }
    
        // Clamps between 0.1 and 0.9.
        allTypesCurrentPrices[typeIndex] = Mathf.Clamp(allTypesCurrentPrices[typeIndex], 0.1f, 0.9f); 

        allTypesPriceHistory[typeIndex].Add(allTypesCurrentPrices[typeIndex]);
    }

    void UpdateGraphData(int typeIndex) // Deletes point that aren't on the graph anymore.
    {
        if (allTypesPriceHistory[typeIndex].Count >= maxDataPoints)
            allTypesPriceHistory[typeIndex].RemoveAt(0);
    }

    void DrawGraph(int typeIndex) // Calculates the point at the moment and the next one.
    {
        int verticalOffset;
        if (typeIndex != 5)
        {
            bandHeight = (int)((2*graphHeight) / 3);
            verticalOffset = graphHeight / 3;
        }
        else
        {
            bandHeight = (int)(graphHeight / 3);
            verticalOffset = 0;
        }

        for (int i = 0; i < allTypesPriceHistory[typeIndex].Count - 1; i++)
        {
            int x1 = i * graphWidth / maxDataPoints; // Point atm depending on width.
            int x2 = (i + 1) * graphWidth / maxDataPoints; // Next point.
            int y1 = verticalOffset + (int)(allTypesPriceHistory[typeIndex][i] * bandHeight);
            int y2 = verticalOffset + (int)(allTypesPriceHistory[typeIndex][i + 1] * bandHeight);

            DrawLine(x1, y1, x2, y2, allTypesColors[typeIndex]);
        }
        graphTexture.Apply();
        priceGraphImage.sprite.texture.Apply();
    }

    void DrawLine(int x1, int y1, int x2, int y2, Color color) // Draws the line between point atm and next one.
    {
        int length = Mathf.Max(Mathf.Abs(x2 - x1), Mathf.Abs(y2 - y1));
        for (int i = 0; i <= length; i++)
        {
            float t = i / (float)length;
            int x = Mathf.RoundToInt(Mathf.Lerp(x1, x2, t)); // Smooths out the line in between the x and the y coordinates of the two points.
            int y = Mathf.RoundToInt(Mathf.Lerp(y1, y2, t));
            graphTexture.SetPixel(x, y, color);
        }
    }
}