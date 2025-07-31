using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoneyManager : MonoBehaviour
{
    public int money = 200; // Money in account.
    public int wood = 0; // wood in account. -> gets updated accordingly to MarketSystem in Update() here at the bottom.
    public MarketSystem marketSystem_Scr;
    

    public TextMeshProUGUI WoodQuantity;
    public TextMeshProUGUI MoneyQuantity;
    public RectTransform moneyIcon;
    public RectTransform woodIcon;
    private bool holdingDown = false;
    public GameObject floatingImagePrefab; // UI image prefab to show
    public Canvas uiCanvas; // Assign your UI canvas here
    public AudioClip MoneySoundEffect;
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        MoneyQuantity.text = "Money: " + money.ToString();
        WoodQuantity.text = "Wood: " + wood.ToString();
        marketSystem_Scr = GameObject.Find("Market System").GetComponent<MarketSystem>();
        audioSource = GameObject.Find("Game Manager").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void PlayMoneySoundEffect()
    {
        audioSource.clip = MoneySoundEffect;
        audioSource.Play();
    }

    public void AddWood(int newWood) // Add wood to account.
    {
        wood += newWood;
        WoodQuantity.text = "Wood:" + wood.ToString();
    }

    public void AddMoney(int newMoney) // Add money to account.
    {

        money += newMoney;
        MoneyQuantity.text = "Money: "+money.ToString();

    }

    public void RemoveMoney(int priceOfSapling) // Remove money from account.
    {
        
        
        money -= priceOfSapling;
        MoneyQuantity.text = "Money: " + money.ToString();
        
        
            
    }


    public void SellingWood() // Start coroutine when pressing sell button. Stops when SELL button is clicked again.
    {
        if (!holdingDown)
        {
            holdingDown = true;
            StartCoroutine(SellWood());
        }
        else
        {
            holdingDown = false;
        }
    }
    
    public IEnumerator SellWood() // Coroutine. --> Takes the Wood price from market system.
    {
        while (holdingDown)
        {
            if (wood > 0)
            {
                wood--;
                WoodQuantity.text = "Wood:" + wood.ToString();
                AddMoney(marketSystem_Scr.ReturnPrice(marketSystem_Scr.S_che)); // TODO: its cherry wood price atm!

                // Instantiate and animate the coin
                Vector3 startPos = new Vector3(UnityEngine.Random.Range(1000, 1500), UnityEngine.Random.Range(500, 1000), 0);
                Vector3 endPos = GameObject.Find("CoinIconGameObject").transform.position;
                PlayMoneySoundEffect();
                GameObject floatingImage = Instantiate(floatingImagePrefab, startPos, Quaternion.identity, uiCanvas.transform);
                floatingImage.SetActive(true);
                RectTransform rectTransform = floatingImage.GetComponent<RectTransform>();
                StartCoroutine(MoveCoin(rectTransform, startPos, endPos, 1.0f));
                
                yield return new WaitForSeconds(0.15f);
            }
            else
            {
                holdingDown = false;
               
            }

        }
    }

    private IEnumerator MoveCoin(RectTransform rectTransform, Vector3 startPos, Vector3 endPos, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            rectTransform.position = Vector3.Lerp(startPos, endPos, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.position = endPos;

        Destroy(rectTransform.gameObject); // Destroy the coin after it reaches the end
    }

    void UpdateIconPositions()
    {
        int digitCountMoney = money.ToString().Length; //Convert money to string and count its digits
        int digitCountWood = wood.ToString().Length;

        // Calculate new X position (-1980 is the ideal position of the coin when there's two digits, 90 is the width adjustment when a digit is added)
        float newXMoney = -1980 + (digitCountMoney-2) * 90;
        float newXWood = -2050 + (digitCountWood - 2) * 90;

        // Apply new position
        moneyIcon.anchoredPosition = new Vector2(newXMoney, moneyIcon.anchoredPosition.y);
        woodIcon.anchoredPosition = new Vector2(newXWood, woodIcon.anchoredPosition.y);
    }

    void Update()
    {
        WoodQuantity.text = "Wood:" + wood.ToString();
        MoneyQuantity.text = "Money: " + money.ToString();
        UpdateIconPositions();
    }
}
