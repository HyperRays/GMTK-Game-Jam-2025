using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LandManager : MonoBehaviour
{
    private GameObject selectedPlot;
    public Button buyButton;
    public Material colorOfPlotBought;
    public int plotPrice;
    private float distanceToCenter;
    private Vector3 centerPoint = new Vector3(0, 0, 0);
    public TextMeshProUGUI buyButtonText;

    //Vectors in all six directions from the plot
    private Vector3 degrees0 = new Vector3(-Mathf.Sqrt(3) / 2, 0.0f, -4.5f);
    private Vector3 degrees60 = new Vector3(2 * Mathf.Sqrt(3), 0.0f, -3.0f);
    private Vector3 degrees120 = new Vector3(5 * Mathf.Sqrt(3) / 2, 0.0f, 1.5f);
    private Vector3 degrees180 = new Vector3(Mathf.Sqrt(3) / 2, 0.0f, 4.5f);
    private Vector3 degrees240 = new Vector3(-2 * Mathf.Sqrt(3), 0.0f, 3.0f);
    private Vector3 degrees300 = new Vector3(-5 * Mathf.Sqrt(3) / 2, 0.0f, -1.5f);
    public GameObject plotPrefab;

    public AudioClip plotBoughtSuccess;
    private AudioSource GM_audioSource;

    private MoneyManager moneyManager_Scr;
    private ShipMovement shipMovement_Scr;

    // Start is called before the first frame update
    void Start()
    {
        buyButton.gameObject.SetActive(false);
        moneyManager_Scr = GameObject.Find("Money Manager").GetComponent<MoneyManager>();
        shipMovement_Scr = GameObject.Find("Boat").GetComponent<ShipMovement>();
        GM_audioSource = GameObject.Find("Game Manager").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void PlotBuying(RaycastHit hit) //Handles the BuyButton
    {
        // IMPORTANT: always use .root to get Parent of hit gameObject -> Somehow Unity doesn't like either GetComponentInParent or parent.GetComponent...

        TagManager cellParentTagManager_Scr = hit.transform.root.GetComponent<TagManager>(); //Takes the hit object's collider's parent (aka the parent empty "plot") 
        //LandPlot land = cellParent.GetComponent<LandPlot>(); // not of need anymore as it is handled through TagManager.
        if (cellParentTagManager_Scr != null && !cellParentTagManager_Scr.HasTag("isOwned")) //When the raycasted object possesses a LandPlot script and it hasn't been bought yet
        {
            selectedPlot = hit.transform.root.gameObject; // Gets the topmost transform and then the gameObject of the hit gameObject (never returns null)
            ShowBuyButton();
        }
    }

    void ShowBuyButton()
    {
        distanceToCenter = Vector3.Distance(selectedPlot.transform.position, centerPoint); // Makes plot price more expensive the further from the center you are -> encourages buying in circle.
        plotPrice = (int)distanceToCenter*500;
        buyButtonText.text = "Buy for: " + plotPrice.ToString();

        if (!buyButton.gameObject.activeSelf) //Probably redundant but let's leave it for now 
        {
            buyButton.gameObject.SetActive(true);
            buyButton.transform.position = Input.mousePosition + new Vector3(50, 20, 0); //slight offset + Had to add an additional value otherwise its a 2D vector in a 3D world - Kai
        }
        else{ buyButton.gameObject.SetActive(!buyButton.gameObject.activeSelf); }
    }

    public void BuyPlot()
    {
        if (selectedPlot != null && moneyManager_Scr.money >= plotPrice)
        {
            selectedPlot.GetComponent<TagManager>().AddTag("isOwned");
            moneyManager_Scr.RemoveMoney(plotPrice);
            ColorOfFrame(selectedPlot); // Change colour to the bought plot colour.
            GM_audioSource.clip = plotBoughtSuccess;
            GM_audioSource.Play();

            // List of direction offsets
            Vector3[] directions = { degrees0, degrees60, degrees120, degrees180, degrees240, degrees300 };
            Vector3 origin = selectedPlot.transform.position;
            int plotLayer = LayerMask.GetMask("Plot"); // Create a mask for the Plot layer

            foreach (Vector3 offset in directions) //Go through the list of directions
            {
                Vector3 newPlotPosition = origin + offset; // Target position for new plot
                RaycastHit hit;

                //If no object is found within the plot layer instantiate a new one, magnitude returns the vectors length
                if (!Physics.Raycast(origin, offset, out hit, offset.magnitude, plotLayer))
                {
                    // No plot detected in this direction, instantiate a new one
                    GameObject newPlot = Instantiate(plotPrefab, newPlotPosition, Quaternion.identity);
                    newPlot.name = "Plot_" + Random.Range(10000, 99999);
                    //Debug.Log("Spawned new plot at: " + newPlotPosition); 
                }
            }
            shipMovement_Scr.CollectIslandSize();
            buyButton.gameObject.SetActive(false);
        }
    }

    public void ColorOfFrame(GameObject plot)
    {
        plot.transform.GetChild(7).GetComponent<Renderer>().material = colorOfPlotBought;
    }


}
