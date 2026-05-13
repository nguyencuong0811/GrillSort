using TMPro;
using UnityEngine;
using UnityEngine.UI;

//======================================================
// SHOP ITEM UI
//======================================================

public class ShopItemUI : MonoBehaviour
{
    [Header("Reward UI")]
    [SerializeField]
    private Image[] rewardIcons;

    [SerializeField]
    private TMP_Text rewardAmounts;

    [Header("Price")]
    [SerializeField]
    private TMP_Text priceText;

    [Header("Button")]
    [SerializeField]
    private Button buyButton;

    [Header("Data")]
    [SerializeField]
    private ShopItemData itemData;

    private void Start()
    {
        Setup();

        buyButton.onClick.AddListener(Buy);
    }

    // SETUP

    private void Setup()
    {
        priceText.text = itemData.price.ToString();

        for (int i = 0; i < rewardIcons.Length; i++)
        {
            rewardIcons[i].gameObject.SetActive(false);

            rewardAmounts.gameObject.SetActive(false);
        }

        for (int i = 0;
            i < itemData.boosterRewards.Count;
            i++)
        {
            rewardIcons[i].gameObject.SetActive(true);

            rewardAmounts.gameObject.SetActive(true);

            rewardIcons[i].sprite =
                itemData.boosterRewards[i].icon;

            rewardAmounts.text =
                "x" + itemData.boosterRewards[i].amount;
        }
    }

    // BUY

    public void Buy()
    {
        bool success =
            BoosterSystem.BuyItem(itemData);

        if (!success)
            return;
        
        AudioManager.Instance?.PlayBuyClick();
        Debug.Log("Buy Success");
    }
}