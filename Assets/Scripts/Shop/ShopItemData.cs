using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop/ Shop Item")]
public class ShopItemData : ScriptableObject
{
    public int price;
    public List<BoosterReward> boosterRewards;
}
