
using System;
using UnityEngine;

public static class BoosterSystem
{
    private const string GOLD_KEY = "PLAYER_GOLD";

    public static Action OnGoldChanged;

    public static Action OnBoosterChanged;

    public static Action<String> OnNotify;
    public static int Gold
    {
        get => PlayerPrefs.GetInt(GOLD_KEY, 9999);

        set
        {
            PlayerPrefs.SetInt(GOLD_KEY, value);

            PlayerPrefs.Save();

            OnGoldChanged?.Invoke();
        }
    }

    public static bool BuyItem(ShopItemData data)
    {
        if(Gold < data.price)
        {
            Debug.Log("Khong du tien");

            return false;
        }

        Gold -= data.price;

        foreach(BoosterReward reward in data.boosterRewards)
        {
            AddBooster(reward.type, reward.amount);
        }
        
        Debug.Log("Mua thanh cong");

        return true;
    }

    public static void AddBooster(BoosterType type, int amount)
    {
        int current = GetBoosterAmount(type);

        current += amount;

        PlayerPrefs.SetInt(GetBoosterKey(type), current);

        PlayerPrefs.Save();

        OnBoosterChanged?.Invoke();
    }

    public static bool UseBooster(BoosterType type)
    {
        int current = GetBoosterAmount(type);

        if(current <= 0)
        {
            OnNotify?.Invoke("Booster không khả dụng!");

            return false;
        }

        current --;

        PlayerPrefs.SetInt(GetBoosterKey(type), current);
        PlayerPrefs.Save();

        OnBoosterChanged?.Invoke();

        return true;
    }

    public static int GetBoosterAmount(BoosterType type)
    {
        return PlayerPrefs.GetInt(GetBoosterKey(type), 0);
    }

    public static string GetBoosterKey(BoosterType type)
    {
        return "BOOSTER_" + type;
    }


}
