using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Unlocked : MonoBehaviour
{
    public Button button;
    public int unlockCost;
    public bool unlocked = false;
    public Sprite lockedButtonSprite;
    public Sprite possibleButtonSprite;
    public Sprite unlockedButtonSprite;
    public TextMeshProUGUI moneyText;

    private string sapButtonName;
    private int typeIdentifier;

    private MoneyManager moneyManager_Scr;
    private GameManager gameManager_Scr;
    private MarketSystem mS_S; // marketSystem_Script

    // Start is called before the first frame update
    void Start()
    {
        sapButtonName = gameObject.name; // Inefficient and two different methods for Unlock System and planting, but it's how it is -> don't want to change it anymore. Ask Guillaume if you have questions.
        mS_S = FindObjectOfType<MarketSystem>();
        moneyManager_Scr = FindObjectOfType<MoneyManager>();
        gameManager_Scr = FindObjectOfType<GameManager>();

        if (sapButtonName == "Oak") // So that GameManager knows which sapling button was pressed and sets correct color of tree and it's sell price usw. (TreeDrag and Plant)
        {
            typeIdentifier = mS_S.S_oak;
        }
        else if (sapButtonName == "Cherry")
        {
            typeIdentifier = mS_S.S_che;
        }

        else if (sapButtonName == "Maple")
        {
            typeIdentifier = mS_S.S_map;
        }

        else if (sapButtonName == "Ginkgo")
        {
            typeIdentifier = mS_S.S_gin;
        }
        else if (sapButtonName == "Wisteria")
        {
            typeIdentifier = mS_S.S_wis;
        }

        if (unlocked) // logic for oak being unlocked at the beginning.
        {
            moneyText.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moneyManager_Scr.money >= unlockCost && !unlocked)
        {
            button.image.sprite = possibleButtonSprite;
        }

        else if (!unlocked)
        {
            button.image.sprite = lockedButtonSprite;
        }

        if (gameManager_Scr.UnlockedSaplingTypes.Contains(sapButtonName)) // Unlocks the sapling. -> Has to be in Update somehow because in Start() it'll probably be too early or smt.
        {
            UnlockSapling();
            gameManager_Scr.UnlockedSaplingTypes.Remove(sapButtonName);
        }
    }

    public void OnSaplingButtonClick()
    {
        if (moneyManager_Scr.money >= unlockCost && !unlocked)
        { 
            UnlockSapling();
            moneyManager_Scr.RemoveMoney(unlockCost); // I've put remove money here, so that I can use unlock in the Start().
        }

        else if (unlocked)
        {
            gameManager_Scr.saplingTypeIdentifier = typeIdentifier;
            gameManager_Scr.TreeDrag();
        }
    }


    void UnlockSapling()
    {
         unlocked = true;
         button.image.sprite = unlockedButtonSprite;
         moneyText.gameObject.SetActive(true);
         gameManager_Scr.UnlockedSaplingTypes.Add(sapButtonName);
    }
}
