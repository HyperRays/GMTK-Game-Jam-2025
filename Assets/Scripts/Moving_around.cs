using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving_around : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera mainCamera;
    private LandManager landManager_Scr;
    private GameManager gameManager_Scr;
    public float speed = 10.0f;
    public float forwardInput;
    public float verticalInput;
    public float zoomSpeed = 2f;
    public float scrollInput;
    private int maxY = 10;
    private int minY = 5;
    private int minX = -110;
    private int maxX = 110;
    private int minZ = -120;
    private int maxZ = 119;
    public Vector3 dragOrigin;
    public bool isDragging = false;
    public bool isOnBorder=false;

    void Start()
    {
        mainCamera = Camera.main;
        landManager_Scr = GameObject.Find("Land Manager").GetComponent<LandManager>();
        gameManager_Scr = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }
    public void Mouse_Scroll()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  // from the camera to the mouse
        Vector3 worldMousePos = ray.origin + ray.direction * 10;
        Vector3 direction = (worldMousePos - mainCamera.transform.position).normalized;
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        // Calculate the potential new position
        Vector3 newPosition = mainCamera.transform.position + (direction * 100 * scrollInput) * Time.deltaTime;
        // Lock position
        if (newPosition.y <= minY)
        {
            newPosition = new Vector3(mainCamera.transform.position.x, minY, mainCamera.transform.position.z);
        }
        else if (newPosition.y >= maxY)
        {
            newPosition = new Vector3(mainCamera.transform.position.x, maxY, mainCamera.transform.position.z);
        }
        mainCamera.transform.position = newPosition;
    }

    public void MovementAWSD()
    {
        verticalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");
        if ((verticalInput + forwardInput) != 0)
        {
            // Compute the new position
            Vector3 newPosition = mainCamera.transform.position;
            newPosition += Vector3.right * Time.deltaTime * speed * verticalInput; // Move forward and backward
            newPosition += Vector3.back * Time.deltaTime * speed * -forwardInput; // Move forward and backward
            mainCamera.transform.position = isPositionGood(newPosition);
            // hide button when moving
            landManager_Scr.buyButton.gameObject.SetActive(false); 
            gameManager_Scr.treeInfo.gameObject.SetActive(false);
        }
        
    }

        

    public void CameraDrag()
    {
        // Update camera position while dragging
        if (isDragging)
        {
            OnMouseDrag();
        }
    }

    public void OnMouseDrag()
    {
        if (isDragging && !isOnBorder)
        {
            
            Vector3 difference = dragOrigin - GetMousePositionAtY0();
            if (difference != Vector3.zero)
            {
                Vector3 newPosition = mainCamera.transform.position;
                newPosition += difference;
                mainCamera.transform.position = isPositionGood(newPosition);
                landManager_Scr.buyButton.gameObject.SetActive(false); // hide button when moving
                gameManager_Scr.treeInfo.gameObject.SetActive(false); // hide tree info too
            }
            
        }
    }

    public Vector3 GetMousePositionAtY0()
{
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    Plane plane = new Plane(Vector3.up, Vector3.zero);  // plane defined by normal (0, 1, 0) and point at (0, 0, 0)
    
    float distance;
    if (plane.Raycast(ray, out distance))
    {
        Vector3 hitPoint = ray.GetPoint(distance);
        return hitPoint;
    }
    return new Vector3(ray.origin.x, 0, ray.origin.z);
}


    Vector3 isPositionGood(Vector3 newPosition)
    {
        if (newPosition.x <= minX)
        {
            newPosition = new Vector3(minX, newPosition.y, newPosition.z);
            isOnBorder = true;
        }
        if (newPosition.x >= maxX)
        {
            newPosition = new Vector3(maxX, newPosition.y, newPosition.z);
            isOnBorder = true;
        }
        if (newPosition.z <= minZ)
        {
            newPosition = new Vector3(newPosition.x, newPosition.y, minZ);
            isOnBorder = true;
        }
        if (newPosition.z >= maxZ)
        {
            newPosition = new Vector3(newPosition.x, newPosition.y, maxZ);
            isOnBorder = true;
        }
        else{ isOnBorder = false; }
        return newPosition;
    }

}
