using System.Collections.Generic;
using UnityEngine;

public class FireSpreading : MonoBehaviour
{
    public bool fire = false;
    public GameObject Fire_Sound; // sound

    private Vector3[] directions; // for the raycast around the tree on fire
    public GameManager gameManagerScr; // gamemanager script
    public LayerMask cellLayer; // mask with only cells for raycast

    // values for fire spreading
    private float lastFireSpreadingTime = 0f;
    private float fireSpreadInterval = 4f;
    private float fireSpreadingSpeed;
    private float lastFireStartingFireTime = 0;
    private float StartingFireInterval = 30;

    void Start()
    {
        fireSpreadingSpeed = PlayerPrefs.GetFloat("fireSpreadingSpeed", 0); // get the difficulty from the homescript in the other scene

        // Easy mode has no fire
        if (fireSpreadingSpeed == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
        
        gameManagerScr = gameManagerScr.GetComponent<GameManager>(); // get gamemanager script
        // pls don't touch these vectors 
        directions = new Vector3[]
        {
            new Vector3(1.73f, -1, 0),
            new Vector3(0.86f, -1, 1.5f),
            new Vector3(-0.86f, -1, 1.5f),
            new Vector3(-1.73f, -1, 0.0f),
            new Vector3(-0.86f, -1, -1.5f),
            new Vector3(0.86f, -1, -1.5f)
        };        
    }

    void Update()
    {
        
        if (!fire)//check if not fire
        {
            Fire_Sound.SetActive(false);
            // after the interval, start a new fire and reset the lastfiretime
            if (Time.time - lastFireStartingFireTime > StartingFireInterval)
            {
                StartFire();
                lastFireStartingFireTime = Time.time;
            }
        }
        else
        {
            // if fire, wait until spread interval is readched and run spread fire
            Fire_Sound.SetActive(true);
            if (Time.time - lastFireSpreadingTime > fireSpreadInterval / fireSpreadingSpeed)
            {
                SpreadFire();
                lastFireSpreadingTime = Time.time; // set teh last fire spreading time
            }
        }
    }

    private void StartFire()
    {
        GameObject[] allTrees = GameObject.FindGameObjectsWithTag("Tree"); // get all the trees on map
        List<GameObject> cellsWithActiveTree = new List<GameObject>();

        // convert the list of active trees into a list with their parent cells
        foreach (GameObject tree in allTrees)
        {
            if (tree.activeSelf && tree.transform.parent != null)
            {
                cellsWithActiveTree.Add(tree.transform.parent.gameObject);
            }
        }

        if (cellsWithActiveTree.Count > 0) // check if trees
        {
            // choose a random cell and set it on fire
            GameObject randomCell = cellsWithActiveTree[Random.Range(0, cellsWithActiveTree.Count)];

            //get and run the animations
            Transform lightning = randomCell.transform.Find("Lightning");
            Transform fireEffect = randomCell.transform.Find("Fire_Yellow");

            lightning.gameObject.SetActive(true);
            fireEffect.gameObject.SetActive(true);
            Fire_Sound.SetActive(true);
            randomCell.GetComponent<TagManager>().AddTag("OnFire");
            fire = true;
            lastFireSpreadingTime = Time.time;//reset the last firespreading time
        }
        else
        {
            Fire_Sound.SetActive(false); // if no trees, set the firesound to false(just for security)
        }
    }

    private void SpreadFire()
    {
        GameObject[] allFires = GameObject.FindGameObjectsWithTag("fire");// get all the burnign trees
        List<GameObject> burningCells = new List<GameObject>();

        //convert the list into their parent list, the cells
        foreach (GameObject fire in allFires)
        {
            if (fire.activeSelf)
            {
                burningCells.Add(fire.transform.parent.gameObject);
            }
        }
        // if no burning cells found, stop fire sound music and stop the function
        if (burningCells.Count == 0)
        {
            fire = false;
            Fire_Sound.SetActive(false);
            return;
        }

        List<GameObject> potentialNewFires = new List<GameObject>();
        // find potential new fire cells by raycasting around the burning cells
        foreach (GameObject burningCell in burningCells)
        {
            foreach (Vector3 dir in directions)
            {
                Ray ray = new Ray(burningCell.transform.position + new Vector3(0, 1, 0), dir*3);// set a raycast
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, cellLayer))// only care about cells
                {
                    // if hit, is occupied and isn't already in the list, add the cell to the list
                    GameObject hitCell = hit.collider.gameObject;
                    if (hitCell.GetComponent<TagManager>().HasTag("Occupied") && !potentialNewFires.Contains(hitCell))
                    {
                        potentialNewFires.Add(hitCell);
                    }
                }
            }
        }
        if (potentialNewFires.Count == 0)
        {
            return;
        }

        foreach (GameObject cell in potentialNewFires)
        {
            if (Random.value <= 0.5f)// randomise the fire spreading
            {
                // set fire active and add tag onfire, starting coroutine waituntildeath in growth of tree
                Transform fireEffect = cell.transform.Find("Fire_Yellow");
                if (fireEffect != null)
                {
                    fireEffect.gameObject.SetActive(true);
                }
                TagManager tagManager = cell.GetComponent<TagManager>();
                if (tagManager != null)
                {
                    tagManager.AddTag("OnFire");
                }
            }
        }
    }
}
