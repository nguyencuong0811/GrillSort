using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HomeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _cointxt;
    [SerializeField] private TextMeshProUGUI _cointxt2;


    void Start()
    {
        UpdateUI();
        BoosterSystem.OnGoldChanged += UpdateUI;
    }

    public void UpdateUI()
    {
        _cointxt.text = BoosterSystem.Gold.ToString();
        _cointxt2.text = BoosterSystem.Gold.ToString();
    }
}
