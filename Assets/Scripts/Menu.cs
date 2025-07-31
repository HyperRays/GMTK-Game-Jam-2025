using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameManager GM_Scr;
    public SunRotating sunRotating_Script;
    public Button menuButton;
    private bool isMenuOpen = false;

    
    public GameObject menuPanel;
    
    private MoneyManager moneyManager_Scr;
    private MarketSystem marketSystem_Scr;
    private SaveSystem saveSystem_Scr;


    public TextMeshProUGUI woodPriceText;

    public TextMeshProUGUI oakPriceText;
    public TextMeshProUGUI cherryPriceText;
    public TextMeshProUGUI maplePriceText;
    public TextMeshProUGUI wisteriaPriceText;
    public TextMeshProUGUI ginkgoPriceText;
    

    // Start is called before the first frame update
    void Start()
    {
        GM_Scr = GameObject.Find("Game Manager").GetComponent<GameManager>();
        menuPanel.SetActive(false);
        menuButton.onClick.AddListener(ToggleMenu);
        sunRotating_Script = GameObject.Find("Sun").GetComponent<SunRotating>();
        moneyManager_Scr = GameObject.Find("Money Manager").GetComponent<MoneyManager>();
        marketSystem_Scr = GameObject.Find("Market System").GetComponent<MarketSystem>();
        saveSystem_Scr = GameObject.Find("Save Manager").GetComponent<SaveSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        
        
        woodPriceText.text = "Sell Wood: " + ((int)(marketSystem_Scr.ReturnPrice(marketSystem_Scr.Wood)/2)).ToString();
        

        UpdateSaplingPriceText();
    }

    public void ToggleMenu() // I don't know what this does because there is Sidebar.cs ....
    {
        isMenuOpen = !isMenuOpen;
        menuPanel.SetActive(isMenuOpen);
        
    }


    public void UpdateSaplingPriceText() // Price show on button for each Sapling type.
    {
        oakPriceText.text = marketSystem_Scr.ReturnPrice(marketSystem_Scr.S_oak).ToString();
        if (GM_Scr.UnlockedSaplingTypes.Contains("Cherry"))
        {
            cherryPriceText.text = marketSystem_Scr.ReturnPrice(marketSystem_Scr.S_che).ToString();
        }

        if (GM_Scr.UnlockedSaplingTypes.Contains("Maple"))
        {
            maplePriceText.text = marketSystem_Scr.ReturnPrice(marketSystem_Scr.S_map).ToString();
        }

        if (GM_Scr.UnlockedSaplingTypes.Contains("Ginkgo"))
        {
            ginkgoPriceText.text = marketSystem_Scr.ReturnPrice(marketSystem_Scr.S_gin).ToString();
        }

        if (GM_Scr.UnlockedSaplingTypes.Contains("Wisteria"))
        {
            wisteriaPriceText.text = marketSystem_Scr.ReturnPrice(marketSystem_Scr.S_wis).ToString();
        }
        
    }   
    


    public void GoToHome()
    {
        saveSystem_Scr.SaveGame();
        SceneManager.LoadScene(0);
    }


}
