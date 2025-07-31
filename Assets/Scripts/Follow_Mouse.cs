using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_Mouse : MonoBehaviour
{
    private Camera mainCamera;
    void Update()
    {
        mainCamera = Camera.main;
        transform.position = GetMousePositionAtY0();
    }


    private Vector3 GetMousePositionAtY0()
    {
        // create an invisible plane and create a ray cast from camera to mouse pointer
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);// create ray
        Plane plane = new Plane(Vector3.up, new Vector3 (0,0.5f, 0));  // plane defined by normal (0, 1, 0) and point at (0, 0, 0)
        
        float distance = 50f;// set distance to 50
            if (plane.Raycast(ray, out distance))
            {
                // get the hitpoint and return hitpoint
                Vector3 hitPoint = ray.GetPoint(distance);
                return hitPoint;
            }
        else
        {
            return new Vector3(ray.origin.x, 0, ray.origin.z);// else return the camera position if no hit(shouldn't happen, but unity not happy if nothing is returned)
        }
        }
    }
