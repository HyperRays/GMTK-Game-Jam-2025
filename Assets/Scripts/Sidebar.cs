using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sidebar : MonoBehaviour
{
    public RectTransform sidebar; // Assign your sidebar panel
    public RectTransform button;  // Assign your button
    private float slideSpeed = 5; // Speed of the slide
    private Vector2 sidebarOnscreenPosition = new Vector2(-155, 0);
    private Vector2 sidebarOffscreenPosition = new Vector2(327,0);
    private Vector2 buttonOnscreenPosition = new Vector2(-472, 150);
    private Vector2 buttonOffscreenPosition = new Vector2(-150, 150);

    public GameObject graph;
    public AudioSource swooshSound;
    public GameObject homeButton;

    private bool isOpen = false;

    private void Update()
    {
        swooshSound = GameObject.Find("Sidebar").GetComponent<AudioSource>();
    }

    public void ToggleSidebar()
    {

        StopAllCoroutines();
        if (!isOpen)
        {
            swooshSound.Play();
            StartCoroutine(MoveUIElement(sidebar, sidebarOnscreenPosition));
            StartCoroutine(MoveUIElement(button, buttonOnscreenPosition));
            graph.SetActive(true);
            homeButton.SetActive(false);
            

        }
        else
        {
            swooshSound.Play();
            StartCoroutine(MoveUIElement(sidebar, sidebarOffscreenPosition));
            StartCoroutine(MoveUIElement(button, buttonOffscreenPosition));
            graph.SetActive(false);
            homeButton.SetActive(true);
        }
        isOpen = !isOpen;
    }

    private IEnumerator MoveUIElement(RectTransform uiElement, Vector2 targetPosition)
    {
        Vector2 initialPosition = uiElement.anchoredPosition;

        // Move smoothly to the target position
        while (Vector2.Distance(uiElement.anchoredPosition, targetPosition) > 0.1f)
        {
            uiElement.anchoredPosition = Vector2.Lerp(uiElement.anchoredPosition, targetPosition, slideSpeed * Time.deltaTime);
            yield return null; // Wait until the next frame
        }

        // Ensure the position is exactly at the target after the loop
        uiElement.anchoredPosition = targetPosition;
    }
}
