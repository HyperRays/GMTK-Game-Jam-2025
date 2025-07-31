using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hover : MonoBehaviour
{
    private Renderer lastHitRenderer;
    public Material NormalColour;
    public Material Stone;
    public LayerMask cellLayerMask;

    void Update()
    {
        // Calculate raycast from cam to mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, cellLayerMask)) // if it hits smth
        {
            if (hit.collider.CompareTag("Cell"))
            {
                Renderer currentRenderer = hit.collider.GetComponent<Renderer>(); // store the current cell
                // if the last renderer isn't the current one, switch the colors, sand for the old one, stone for the new one.
                if (lastHitRenderer != currentRenderer)
                {
                    if (lastHitRenderer != null) // if the last game object is a real gameobject, not null, change the material to sand
                    {
                        lastHitRenderer.material = NormalColour;
                    }

                    NormalColour = currentRenderer.material; // get the colour right now, remember it
                    currentRenderer.material = Stone; // change the current colour
                    lastHitRenderer = currentRenderer; // remember the last hit colour to not mess up the colours
                }
            }
        }
        // reset the colour of the previous cell if the last hit isn't null
        else
        {
            if (lastHitRenderer != null)
            {
                lastHitRenderer.material = NormalColour;
                lastHitRenderer = null;
            }
        }
        
    }
}
