using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // used for List.min()

public class ShipMovement : MonoBehaviour
{
    public List<Vector3> plotPositions = new List<Vector3>();
    private List<float> plotDistance = new List<float>();

    private float angle;
    private float rotationSpeed = 0.1f;
    private float distance = 20f;

    // Start is called before the first frame update
    void Start()
    {
        CollectIslandSize();
        
    }

    // Update is called once per frame
    void Update()
    {
        

        angle += rotationSpeed * Time.deltaTime;
        
        Movement();
        
    }


    public void CollectIslandSize() // To be called everytime a plot is bought.
    {
        plotPositions.Clear();
        foreach (Plot plot in FindObjectsOfType<Plot>()) // Collects positions of all instatiated plots.
        {
            plotPositions.Add(plot.transform.position);
        }
    }

    void Movement()
    {
        plotDistance.Clear();
        foreach (Vector3 pos in plotPositions) // Takes all plot positions and calculates the distance from the ship to the plot as vecto3.
        {
            plotDistance.Add(Vector3.Distance(transform.position, pos));
        }

        float minDistance = plotDistance.Min(); // Gets the nearest distance and then plot.
        int minDistanceIndex = plotDistance.IndexOf(plotDistance.Min());
        
        
        // Absolute monstrosity of a line.. 
        // calculates the new transform position based on angle. Smoothly lerps the ship to it, as i can't get it to work otherwise.
        transform.position = Vector3.Lerp(transform.position, new Vector3(plotPositions[minDistanceIndex].x + Mathf.Sin(angle) * distance, transform.position.y, plotPositions[minDistanceIndex].z + Mathf.Cos(angle) * distance), 1f * Time.deltaTime);

        // Orientates the ship tangential to the plot circle. I tried other ways but it never did how I excpected it to.
        Vector3 distanceVector = (transform.position - plotPositions[minDistanceIndex]).normalized;
        Vector3 tangent = Quaternion.AngleAxis(90, Vector3.up) * distanceVector;
        transform.rotation = Quaternion.LookRotation(tangent, Vector3.up);
    } 

}
