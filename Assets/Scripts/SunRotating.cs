using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotating : MonoBehaviour
{
    public GameManager GM_Scr;
    public float sunStartAngle = 270;
    private float moonStartAngle = 90;
    public float timeMinutes;
    private float dayProgress;




    // Start is called before the first frame update
    void Start()
    {
        GM_Scr = GameObject.Find("Game Manager").GetComponent<GameManager>();

    }
    // Update is called once per frame
    void Update()
    {
        
        timeMinutes = (float)(GM_Scr.TimeHandler(GM_Scr.time).TotalMinutes); // Get ingame minutes.

        
        dayProgress = timeMinutes / 1440f; // 1440 ingame minutes per full cycle.

        if (GetComponent<TagManager>().HasTag("Sun"))
        {
            transform.rotation = Quaternion.Euler(sunStartAngle - (dayProgress * 360f), 0, 0);
        }
        else // Moon.
        {
            transform.rotation = Quaternion.Euler(moonStartAngle - (dayProgress * 360f), 0, 0);
        }
    }
}
