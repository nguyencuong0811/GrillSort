using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoosterType
{
    Magnet,
    Swap,
    Timer,
    AddGrill
}

[System.Serializable]
public class BoosterReward
{
    public BoosterType type;
    public Sprite icon;
    public int amount;
}


