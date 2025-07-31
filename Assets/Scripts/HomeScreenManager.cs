using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class HomeScreenManager : MonoBehaviour
{
    public GameObject startButton;
    public GameObject settingsButton;
    public GameObject backButton;
    public GameObject instructionsButton;
    public GameObject instBackButton;
    public GameObject exitButton;
    private GameObject pressedButton;

    public GameObject resetButton;
    public GameObject deactivateSoundeffects; // doesn't do anything atm
    public GameObject sensitivity; // doesn't do anything atm


    private SaveSystem saveSystem_Scr;

    private Camera mainCamera;


    Vector3 targetPosition;
    Vector3 cameraTargetPosition;
    float smoothTime = 0.3f; // Duration in second of the smoothing when reaching end of Smoothamp.
    private Vector3 buttonVelocity = Vector3.zero; // Direction.
    private Vector3 cameraVelocity = Vector3.zero;
    bool isMoving = false;
    bool isCameraMoving = false;
    bool movingDown = true; // Tracks movement direction.
    private bool isMuted = false; // Bool for muting
    public Color mutedColor; // for muting 
    public Color unmutedColor; // for muting 
    public GameObject muteText;
    public GameObject unmuteText;
    private AudioSource audioSource;
    public AudioClip buttonClickSound;

    public GameObject difficultyButton;
    public GameObject easyText;
    public GameObject mediumText;
    public GameObject hardText;
    public GameObject impossibleText;
    private int difficultyLevel = 0;
    private int maxDifficulty = 3;
    public float fireSpreadRate;
    public Color easyColor;
    public Color medColor;
    public Color hardColor;
    public Color impColor;

    public GameObject rainbowTree;
    public Material[] leavesMaterials = new Material[5];


    private float bDnPos = 0.09f; // Button moving down final pos.
    private float bUpPos = 0.21f; // Button moving up final pos.
    private float PosY;

    private FireSpreading fireSpreadingScript;

    private void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null && !isMuted)
        {
            audioSource.PlayOneShot(buttonClickSound); // Play sound when a button is pressed
        }
    }

    private void Start()
    {

        audioSource = GetComponent<AudioSource>();
        saveSystem_Scr = GetComponent<SaveSystem>();
        mainCamera = Camera.main;

        StartCoroutine(RainbowTree());

        int savedMute = PlayerPrefs.GetInt("Muted", 1);

        if (savedMute == 1)
        {
            isMuted = true;
            AudioListener.volume = 0f;
            deactivateSoundeffects.GetComponent<Renderer>().material.color = unmutedColor;
        }
        else
        {
            isMuted = false;
            AudioListener.volume = 1f;
            deactivateSoundeffects.GetComponent<Renderer>().material.color = mutedColor;
        }
        difficultyLevel = PlayerPrefs.GetInt("DifficultyLevel", 1);
        UpdateMuteText();
        UpdateDifficultyText();

    }

    void Update()
    {
        // When left mouse button is clicked and the button is at the start position.
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObj = hit.collider.gameObject;

                if (isButton(hitObj))
                {
                    SetPressedButton(hitObj);
                    PlayButtonClickSound();
                }
            }
        }

        if (isMoving)
        {
            pressedButton.transform.position = Vector3.SmoothDamp(pressedButton.transform.position, targetPosition, ref buttonVelocity, smoothTime);

            // Stop movement when close enough.
            if (Mathf.Abs(pressedButton.transform.position.y - targetPosition.y) < 0.001f)
            {
                pressedButton.transform.position = targetPosition; // Set position to exact targetPosition to end SmoothDamp. (Avoid overshooting)

                if (movingDown)
                {
                    // After reaching bottom, switching to moving up.
                    targetPosition = new Vector3(pressedButton.transform.position.x, 0.21f, pressedButton.transform.position.z);
                }
                else
                {
                    isMoving = false; // Stop when reaching the top.
                }

                movingDown = !movingDown; // Toggle direction.
            }
        }


        // What should the button do?

        if (pressedButton == startButton && !isMoving)
        {
            SceneManager.LoadScene(1); // Loads the Main Scene (SampleScene)
        }

        else if (pressedButton == settingsButton)
        {
            cameraTargetPosition = new Vector3(4.32f, 2.5f, -8.51f);
            isCameraMoving = true;
            mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, cameraTargetPosition, ref cameraVelocity, smoothTime);

            if (Mathf.Abs(pressedButton.transform.position.y - targetPosition.y) < 0.01f)
            {
                pressedButton.transform.position = targetPosition;
                isCameraMoving = false;
            }
        }

        else if (pressedButton == backButton || pressedButton == instBackButton)
        {
            cameraTargetPosition = new Vector3(0f, 2.5f, -10f);
            mainCamera.transform.rotation = Quaternion.Euler(10.25f, 0, 0f);
            isCameraMoving = true;
            mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, cameraTargetPosition, ref cameraVelocity, smoothTime);

            if (Mathf.Abs(pressedButton.transform.position.y - targetPosition.y) < 0.01f)
            {
                pressedButton.transform.position = targetPosition;
                isCameraMoving = false;
            }
        }

        else if (pressedButton == instructionsButton)
        {
            cameraTargetPosition = new Vector3(-4.35f, 4.54f, -2.85f);
            isCameraMoving = true;
            mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, cameraTargetPosition, ref cameraVelocity, smoothTime);
            mainCamera.transform.rotation = Quaternion.Euler(90f, 0, 0f);


            if (Mathf.Abs(pressedButton.transform.position.y - targetPosition.y) < 0.01f)
            {
                pressedButton.transform.position = targetPosition;
                isCameraMoving = false;
            }
        }

        else if (pressedButton == exitButton)
        {
            Application.Quit();
        }

        else if (pressedButton == resetButton)
        {

            PlayerPrefs.SetInt("ResetActivated", 1); // 1 is true, 0 is false, PlayerPrefs can't save bools.
            pressedButton.GetComponent<Renderer>().material.color = Color.black;
        }
        else if (pressedButton == deactivateSoundeffects && !isMoving)
        {
            isMuted = !isMuted;

            if (isMuted)
            {
                AudioListener.volume = 0; //Mutes
                deactivateSoundeffects.GetComponent<Renderer>().material.color = unmutedColor; // Changes hue when hte button is pressed
                PlayerPrefs.SetInt("Muted", 1);

            }
            else
            {
                AudioListener.volume = 1; // unmutes
                deactivateSoundeffects.GetComponent<Renderer>().material.color = mutedColor; // changes hue 
                PlayerPrefs.SetInt("Muted", 0);
            }
            UpdateMuteText();
            pressedButton = null; // prevents mute unmute toggeling - for some reason once hte button was pressed it kept on switching rapidly between mute and unmute
        }
        else if (pressedButton == difficultyButton && !isMoving)
        {
            difficultyLevel++;
            if (difficultyLevel > maxDifficulty)
            {
                difficultyLevel = 0;
            }
            PlayerPrefs.SetInt("DifficultyLevel", difficultyLevel);
            pressedButton = null;
            UpdateDifficultyText();

        }

    }


    private bool isButton(GameObject obj) // Which button is being pressed 
    {
        return obj == startButton || obj == settingsButton || obj == backButton || obj == instructionsButton || obj == exitButton || obj == instBackButton || obj == resetButton || obj == deactivateSoundeffects || obj == difficultyButton;
    }

    private void SetPressedButton(GameObject button) // Set the Position for the button going up or going down.
    {
        pressedButton = button;
        isMoving = true;

        if (movingDown)
        {
            PosY = bDnPos;
        }
        else
        {
            PosY = bUpPos;
        }


        targetPosition = new Vector3(pressedButton.transform.position.x, PosY, pressedButton.transform.position.z);
    }
    private void UpdateMuteText()
    {
        if (isMuted)
        {
            // Show "Unmute" text and hide "Mute" text
            muteText.SetActive(false);
            unmuteText.SetActive(true);
        }
        else
        {
            // Show "Mute" text and hide "Unmute" text
            muteText.SetActive(true);
            unmuteText.SetActive(false);
        }
    }
    private void UpdateDifficultyText()
    {
        easyText.SetActive(false); // setting all text to false 
        mediumText.SetActive(false);
        hardText.SetActive(false);
        impossibleText.SetActive(false);
        if (difficultyLevel == 1)
        {
            easyText.SetActive(true); //changes text to easy 
            fireSpreadRate = 0f; //The lightning strikes but the fire wont spread to neighboring trees 
            difficultyButton.GetComponent<Renderer>().material.color = easyColor; // changes color of button 
        }
        else if (difficultyLevel == 2)
        {
            mediumText.SetActive(true);
            fireSpreadRate = 1.25f; // fire spreads 
            difficultyButton.GetComponent<Renderer>().material.color = medColor;
        }
        else if (difficultyLevel == 3)
        {
            hardText.SetActive(true);
            fireSpreadRate = 1.5f; // fire spreads quicker 
            difficultyButton.GetComponent<Renderer>().material.color = hardColor;
        }
        else
        {
            impossibleText.SetActive(true);
            fireSpreadRate = 2.0f; // wild fire season 
            difficultyButton.GetComponent<Renderer>().material.color = impColor;
        }
        PlayerPrefs.SetFloat("fireSpreadingSpeed", fireSpreadRate); //stores the altered value to the PlayerPrefs to be applicable for other scene 
    }


    private IEnumerator RainbowTree()
    {
        int i = 0;
        while (true)
        {
            rainbowTree.GetComponent<Renderer>().material.color = leavesMaterials[i].color;
            yield return new WaitForSeconds(0.2f);
            i = (i + 1) % 5;
        }
    }
}

