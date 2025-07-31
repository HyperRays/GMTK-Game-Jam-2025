using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GrowthOfTree : MonoBehaviour
{
    private GameManager GM_Scr; // gamemanager script
    private MoneyManager moneyManager_Scr;
    public double timeOfPlanting = 0;
    public int age; // in RL minutes.
    public int stage;
    private float stageDuration = 4.0f; // Stage duration in ingame hours.
    public bool growing = false;
    private bool StartedCoroutine = false;
    public int health; // amount of times you have to hit the tree to chop it down.
    public GameObject[] growthStages;
    public Material red; // stage colours
    public Material yellow;// stage colours
    public Material orange; // stage colours
    private GameObject fire; // stage colours
    public AudioClip chop1; // chop sound
    public AudioClip chop2; // chop sound
    public AudioClip chop3; // chop sound
    private AudioSource audioSource; 
    public ParticleSystem leafDropping;
    public Material[] allTypesMaterials = new Material[5]; // Material array of the trees.

    public int saveTypeIdentifier;
    private int[] woodAmountGet = new int[] { 2, 3, 5, 4, 1 }; // How much money you get depending on tree type. Order is weird because I programmed the typeIdentifier badly. Sorry.

    //  DateTime start = DateTime.Now (in Start() Funktion, wenn das Tree objekt erstellt wird) -> ist folgendes Format: new DateTime(2024, 2, 25, 14, 30, 0)

    void Start()
    {
        GM_Scr = GameObject.Find("Game Manager").GetComponent<GameManager>(); // get gamemanager script
        moneyManager_Scr = GameObject.Find("Money Manager").GetComponent<MoneyManager>();
        fire = gameObject.transform.parent.Find("Fire_Yellow").gameObject; // efficiency right there(get the parent (transform)and get the child gameobject)
        if (timeOfPlanting == 0)
        {
            timeOfPlanting = GM_Scr.time;  // When was the tree planted in ingame minutes since first startup of game.
        }
        growing = true; // set growing true
        health = 3; // health is 3
        audioSource = GameObject.Find("Game Manager").GetComponent<AudioSource>();
        ChangeLeavesColor(saveTypeIdentifier);
    }

    public void PlayClip1() // play chop sound 1
    {
        audioSource.clip = chop1;
        audioSource.Play();
    }

    public void PlayClip2() // play chop sound 2
    {
        audioSource.clip = chop2;
        audioSource.Play();
    }

    public void PlayClip3() // play chop sound 3
    {
        audioSource.clip = chop3;
        audioSource.Play();
    }

    void LateUpdate() // trying not to kill the game with to many update loops
    {
        if (growing)
        {
            age = (int)((GM_Scr.time - timeOfPlanting) / 60); // Convert ingame minutes difference since planting to ingame hours.
            stage = Math.Min((int)(age / stageDuration), growthStages.Length - 1); // Makes sure, that stages doesn't exceed stages limit when age grows bigger.

            for (int i = 0; i < growthStages.Length; i++)
            {
                growthStages[i].SetActive(i == stage); // Sets the stages of the tree active/unactive depending what stage the tree has.
            }

            if (stage == growthStages.Length - 1) // When tree gets to last stages, set growing false and stop checking.
            {
                growing = false;
            }
        }
        if (fire.activeSelf && !StartedCoroutine)
        {
            StartCoroutine(WaitUntilDeath());
        }
    }

    IEnumerator WaitUntilDeath()
    {
        StartedCoroutine = true;
        float delay = UnityEngine.Random.Range(5f, 10f);
        yield return new WaitForSeconds(delay); // change this to delay later
        StartedCoroutine = false;
        health = 0;
        ChopDownTree();
    }

    public void ChopDownTree()
    {
        GameObject parent = gameObject.transform.parent.gameObject;

        if (growing || health <= 0) // if it's growing or health is 0 kill the tree
        {
            if (!parent.GetComponent<TagManager>().HasTag("OnFire"))
            {
                // play animations
                PlayClip1();
                leafDropping.Play();
                GM_Scr.CameraShaking();
                GM_Scr.SpawnWoodIcon(woodAmountGet[saveTypeIdentifier]);
                moneyManager_Scr.AddWood(woodAmountGet[saveTypeIdentifier]);
            }


            // Reset all the values to default
            timeOfPlanting = 0;
            age = 0;
            stage = 0;
            growing = true;
            fire.SetActive(false);
            growthStages[0].SetActive(true); // Reset to first growth stage
            parent.GetComponent<TagManager>().RemoveTag("OnFire");
            parent.GetComponent<TagManager>().RemoveTag("Occupied");
            // at last, set the game object inactive
            gameObject.SetActive(false);
        }
        else if (health == 3)
        {
            // Play animations
            PlayClip1();
            GM_Scr.CameraShaking();
            GM_Scr.SpawnWoodIcon(woodAmountGet[saveTypeIdentifier]);
            leafDropping.Play();
            moneyManager_Scr.AddWood(woodAmountGet[saveTypeIdentifier]);
        }
        else if (health == 2)
        {
            // Play animations
            PlayClip2();
            GM_Scr.CameraShaking();
            GM_Scr.SpawnWoodIcon(woodAmountGet[saveTypeIdentifier]);
            leafDropping.Play();
            moneyManager_Scr.AddWood(woodAmountGet[saveTypeIdentifier]);
        }
        else if (health == 1)
        {
            // Play animations
            PlayClip3();
            GM_Scr.CameraShaking();
            GM_Scr.SpawnWoodIcon(woodAmountGet[saveTypeIdentifier]);
            leafDropping.Play();
            moneyManager_Scr.AddWood(woodAmountGet[saveTypeIdentifier]);
        }
        health -= 1; // reduce health by one
    }

    public string StillGrowing()
    {
       if (growing)
       {
            return "Yes";
       }
       else
       {
            return "No";
       }
    }

    public void ChangeLeavesColor(int typeIdentifier)
    {
        saveTypeIdentifier = typeIdentifier;
        for (int i = 0; i < growthStages.Length; i++)
        {
            gameObject.transform.GetChild(i).GetChild(1).GetComponent<Renderer>().material = allTypesMaterials[typeIdentifier];

        }
        var main = leafDropping.main; // Why is Unity so weird? I wanted to do ps[2].main.startColor directly but it wouldn't let me.. smh
        main.startColor = new ParticleSystem.MinMaxGradient(allTypesMaterials[typeIdentifier].color);
    }
}
