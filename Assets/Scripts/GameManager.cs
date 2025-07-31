using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;



public class GameManager : MonoBehaviour
{
    private Camera mainCamera;
    public int dayCycle; // Real minutes for a full ingame day (24 hours).
    public double timeSinceSessionStart; // Total ingame minutes passed since session start.
    public double time; // Total ingame minutes passed since the game was first started (adds up).
    public double timeOfLastSession; // Time since the game was first started (later of relevance to remember where the time/day the person left off)
    public bool showingTreeInfo = false;
    public string[] treeNames = new string[5] { "Cherry", "Maple", "Wisteria", "Ginkgo", "Oak" };
    private TimeSpan gameTime; // The current ingame clock (hours:minutes format).
    private DateTime timeOfStart; // RL time when the session started.

    public List<string> UnlockedSaplingTypes = new List<string>(); // Important for SaveSystem, collects unlocked Types.
    public int saplingTypeIdentifier; // Unlocked script sets this to the sapling button that was clicked -> correct tree type setting.

    public TextMeshProUGUI gameTimeText; // display for the clock.

    public TextMeshProUGUI treeInfo;
    private Moving_around MA_Scr; // Monving around script
    private LandManager landManager_Scr; // land manager script
    private GrowthOfTree growthOfTree_Scr; // growth of tree script
    private MoneyManager moneyManager_Scr; // money manager script
    private MarketSystem marketSystem_Scr;

    private AudioSource boatHornSound;
    public AudioClip plantSound;
    
    private AudioSource GM_audioSource; // to play the audio

    public GameObject Sapling_Follow_mouse; // Sapling gameobject
    public GameObject[] saplingMouseChildren = new GameObject[5];
    
    public GameObject Axe_Follow_mouse; // Axe gameobject

    public LayerMask cellLayerMask; // for raycast only cells
    public LayerMask plotLayerMask; // For plots
    public GameObject floatingImagePrefab; // The Image that's used in the canvas.
    public Canvas uiCanvas; // The Canvas of the ui.


    void Start()
    {
        mainCamera = Camera.main; // Get the camera gameobject
        MA_Scr = GetComponent<Moving_around>(); // Get the moving around script
        marketSystem_Scr = GameObject.Find("Market System").GetComponent<MarketSystem>();
        timeOfStart = DateTime.Now; // Store the real-world start time.
        timeOfLastSession = 0;
        time = 0; // Reset time at session start.

        landManager_Scr = GameObject.Find("Land Manager").GetComponent<LandManager>(); // Get the land manager script on camera
        growthOfTree_Scr = GetComponent<GrowthOfTree>(); // get the growth of tree
        moneyManager_Scr = GameObject.Find("Money Manager").GetComponent<MoneyManager>(); // get the money manager script
        boatHornSound = GameObject.Find("Boat").GetComponent<AudioSource>(); // get the audio source on boat
        GM_audioSource = GetComponent<AudioSource>(); // get the audio source on gamemanager
    }


    public TimeSpan TimeHandler(double time) // Converts total ingame minutes to an ingame clock format.
    {
        double hours = (time / 60) % 24; // Convert total ingame minutes to hours (looping within 24-hour format).
        double minutes = time % 60;

        gameTime = new TimeSpan((int)hours, (int)minutes, 0);
        return gameTime;
    }

    int DaysPassed(double time) // Calculates the number of full ingame days passed.
    {
        return (int)(time / 1440); // 1440 ingame minutes = 1 ingame day (24 hours).
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) //Checks if Mouse is over UI and prevents further Raycasting.
        {
            // Dezoom or move with arrows or AWSD
            MA_Scr.Mouse_Scroll();
            MA_Scr.MovementAWSD();
            return;
        }
        double realMinutesPassed = (DateTime.Now - timeOfStart).TotalMinutes; // Real time paassed since session start.
        timeSinceSessionStart = (realMinutesPassed / dayCycle) * 1440; // Convert real time to ingame minutes.
        time = timeSinceSessionStart + timeOfLastSession; // Total ingame minutes since session start.

        gameTimeText.text = "Day " + DaysPassed(time).ToString() + " - " + TimeHandler(time).ToString(@"hh\:mm"); // Update clock.

        // Dezoom or move with arrows or AWSD
        MA_Scr.Mouse_Scroll();
        MA_Scr.MovementAWSD();

        HandleLefClick();
        HandleRightClick();

        playRandomHornSound();
    }

    void HandleLefClick()
    {
        if (Input.GetMouseButtonDown(0))// if left click detected
        {
            treeInfo.transform.parent.gameObject.SetActive(false); // stop showing the info screen if left clicked
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // create a ray from camera to mouse
            // if nothing is in the hand, you can move around or buy a plot
            if (!Sapling_Follow_mouse.activeSelf && !Axe_Follow_mouse.activeSelf)
            {
                BuyPlot(ray);
                MA_Scr.dragOrigin = MA_Scr.GetMousePositionAtY0();
                MA_Scr.isDragging = true;
            }
            // if a sapling is in the hand
            else if (Sapling_Follow_mouse.activeSelf)
            {
                Plant(ray);
            }
            // if the axe is in hand
            else if (Axe_Follow_mouse.activeSelf)
            {
                Chop(ray);
            }
        }
        // Check for mouse button hold (Continue dragging)
        if (Input.GetMouseButton(0) && MA_Scr.isDragging && !Sapling_Follow_mouse.activeSelf && !Axe_Follow_mouse.activeSelf)
        {
            MA_Scr.CameraDrag();
        }

        // Stop dragging when the left mouse button is released
        if (Input.GetMouseButtonUp(0) && MA_Scr.isDragging)
        {
            MA_Scr.isDragging = false;
        }
    }
    void HandleRightClick()
    {
        if (Input.GetMouseButtonDown(1)) // if right click detected
        {
            landManager_Scr.buyButton.gameObject.SetActive(false); // stop showing any buy buttons
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // ray from camera to mouse
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, cellLayerMask) && Sapling_Follow_mouse.activeSelf == false) // only care about cells and only work when no sapling in the hand
            {
                TreeInfo(hit.collider.gameObject.transform.GetChild(0).gameObject); // Gets the tree on the cell, first gets it through transform and converts it back into gameobject for the funciton
            }
            
        }
    }

    void Plant(Ray ray) // Raycast for left click.
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, cellLayerMask | plotLayerMask))
        {
            // if the hit is a cell(double check) and is owned by the player, set the planting time, health, substract the price and show the tree
            if (hit.collider.GetComponent<TagManager>().HasTag("Cell") && hit.transform.root.GetComponent<TagManager>().HasTag("isOwned")) // Check if owned happens in the LandManager.
            {
                GameObject hitObject = hit.collider.gameObject;
                Transform tree = hitObject.transform.GetChild(0);

                if (tree.gameObject.activeSelf || moneyManager_Scr.money < marketSystem_Scr.ReturnPrice(saplingTypeIdentifier)) // If a tree is already there or there is not enough money.
                {
                    return;
                }
                GrowthOfTree GOT = tree.gameObject.GetComponent<GrowthOfTree>();
                GOT.timeOfPlanting = time;
                GOT.health = 3;
                GOT.saveTypeIdentifier = saplingTypeIdentifier;
                GOT.ChangeLeavesColor(saplingTypeIdentifier);
            
                tree.gameObject.SetActive(true);
                GM_audioSource.clip = plantSound;
                GM_audioSource.Play();
                hitObject.GetComponent<TagManager>().AddTag("Occupied");
                moneyManager_Scr.RemoveMoney(marketSystem_Scr.ReturnPrice(saplingTypeIdentifier));
            }
        }
        else
        {
            MouseSaplingVisibilityHandler(false);
        }

    }

    void Chop(Ray ray) // Raycast for left click.
    {
        RaycastHit hit;
        // send a ray cast for only cells
        if (Physics.Raycast(ray, out hit, 100, cellLayerMask | plotLayerMask)) // plotlayer for the else, we only want the axe to disapear if sea is pressed
        {
            if (hit.collider.GetComponent<TagManager>().HasTag("Cell")) // we want cell not plot
            {
                GameObject hitObject = hit.collider.gameObject;
                Transform tree = hitObject.transform.GetChild(0); // get the tree on the cell(inactive or not)

                if (!tree.gameObject.activeSelf)
                { return; } // if the tree is inactive, return
                growthOfTree_Scr = tree.gameObject.GetComponent<GrowthOfTree>(); // get the script
                growthOfTree_Scr.ChopDownTree(); // chopdown
            }
        }
        // if no hit(sea), stop showing axe
        else
        {
            Axe_Follow_mouse.SetActive(false);
        }
    }

    void BuyPlot(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, cellLayerMask | plotLayerMask)) // ray casts for cells
        {
            landManager_Scr.PlotBuying(hit); // buy plot
        }
        else
        {
            // hide button when moving
            landManager_Scr.buyButton.gameObject.SetActive(false);
            treeInfo.transform.parent.gameObject.SetActive(false);
        }
    }

    public void TreeDrag()
    {
        // if a spaling is dragged, set the game pannels to false and the axe
        treeInfo.transform.parent.gameObject.SetActive(false);
        Axe_Follow_mouse.SetActive(false);
        landManager_Scr.buyButton.gameObject.SetActive(false);
        treeInfo.transform.parent.gameObject.SetActive(false);
        if (!Sapling_Follow_mouse.activeSelf && moneyManager_Scr.money > (int)marketSystem_Scr.ReturnPrice(saplingTypeIdentifier)) // if there no tree on the cell yet and enough money
        {
            MouseSaplingVisibilityHandler(true);

        }
        else
        { // else set the sapling to the inverse state
            MouseSaplingVisibilityHandler(!Sapling_Follow_mouse.activeSelf); 
        } 
    }

    public void AxeDrag()
    {
        // if the axe is dragged, set the game pannels to false and the sapling
        treeInfo.transform.parent.gameObject.SetActive(false);
        MouseSaplingVisibilityHandler(false);
        landManager_Scr.buyButton.gameObject.SetActive(false);
        treeInfo.transform.parent.gameObject.SetActive(false);
        if (!Axe_Follow_mouse.activeSelf)
        {
            Axe_Follow_mouse.SetActive(true); // show the axe if it wasn't shown

        }
        else { Axe_Follow_mouse.SetActive(!Axe_Follow_mouse.activeSelf); } // inverse the state of the axe
    }

    public void TreeInfo(GameObject tree) // Method that shows Information to the tree that was right clicked.
    {
        if (tree.activeSelf)
        {
            // create the text pannel with the info on it based on the script growth of tree
            growthOfTree_Scr = tree.gameObject.GetComponent<GrowthOfTree>();
            GrowthOfTree gOTPar = tree.GetComponentInParent<GrowthOfTree>();
            treeInfo.text = "Age: " + (int)(gOTPar.age) + " hours" + "\nGrowing: " + gOTPar.StillGrowing() + "\nType: " + treeNames[gOTPar.saveTypeIdentifier];
        }
        else
        {
            treeInfo.text = "Free Cell";
        }
        treeInfo.transform.parent.transform.position = Input.mousePosition + new Vector3(100, 100, 0); // calculate position vector
        treeInfo.transform.parent.gameObject.SetActive(true); // show the sign
    }

    public void SpawnWoodIcon(int woodamount) // Show wood animation when choping 
    {
        Vector3 startPos = Input.mousePosition; // get start pos(mouse position)
        Vector3 endPos = GameObject.Find("WoodIconGameobject").transform.position; // endpos(the wood icon in the top right)


        for (int i = 0; i < woodamount; i++)
        {
            GameObject floatingImage = Instantiate(floatingImagePrefab, startPos, Quaternion.identity, uiCanvas.transform); // get the gameobject
            floatingImage.SetActive(true); // show it
            RectTransform rectTransform = floatingImage.GetComponent<RectTransform>(); // get it recttransform component
            Vector3 random_vector = new Vector3(UnityEngine.Random.Range(-150, 150), UnityEngine.Random.Range(-150, 150), 0);
            Debug.Log("coroutine started");
            StartCoroutine(AnimateFloatingImage(rectTransform, startPos + random_vector, endPos, 1.0f)); // start the animaiton using cooroutine
        }
    }

    public IEnumerator AnimateFloatingImage(RectTransform image, Vector3 start, Vector3 end, float duration)
    {
        // set the starting values
        float elapsed = 0f;
        image.position = start;
        while (elapsed < duration)
        {
            elapsed += UnityEngine.Time.deltaTime; // specify unity engine due to errors with other libraries
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0, 1, t); // Smooth movement
            image.position = Vector3.Lerp(start, end, t); // Move the image smoothly
            yield return null;
        }
        image.position = end;
        Destroy(image.gameObject); // once done, destroy gameobject
    }

    public void CameraShaking()
    {
        StartCoroutine(ShakeCamera());
    }

    private IEnumerator ShakeCamera()
    {
        Quaternion standardRotation = mainCamera.transform.rotation;

        // shoot to the left and a bit schr�g.
        mainCamera.transform.rotation = Quaternion.Euler(47.04f, -7f, UnityEngine.Random.Range(-7, 7));
        yield return new WaitForSeconds(0.05f); // waits before it returns.

        
        float time = 0f;
        float duration = 0.3f;

        while (time < duration)
        {
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, standardRotation, time / duration); // Lerps slowly and then faster back to original pos.
            time += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.rotation = standardRotation; // Sets it back to the original, as Lerp is not exact.
    }
    

    void playRandomHornSound()
    {
        if (UnityEngine.Random.value < 0.001)
        {
            boatHornSound.Play();
        }
    }

    void MouseSaplingVisibilityHandler(bool boolean)
    {
        Sapling_Follow_mouse.SetActive(boolean); // show the tree
        foreach (GameObject child in saplingMouseChildren)
        {
            child.SetActive(false);
        }
        
        saplingMouseChildren[saplingTypeIdentifier].gameObject.SetActive(boolean);
    }
}