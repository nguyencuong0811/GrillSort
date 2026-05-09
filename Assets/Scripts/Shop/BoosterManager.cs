using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoosterManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI magnetAmount;
    [SerializeField] private TextMeshProUGUI swapAmount;
    [SerializeField] private TextMeshProUGUI timerAmount;
    [SerializeField] private TextMeshProUGUI addgrillAmount;
    
    void Start()
    {
        UpdateUI();
        BoosterSystem.OnBoosterChanged += UpdateUI;
    }

    public void UpdateUI()
    {
        magnetAmount.text = BoosterSystem.GetBoosterAmount(BoosterType.Magnet).ToString();
        swapAmount.text = BoosterSystem.GetBoosterAmount(BoosterType.Swap).ToString();
        timerAmount.text = BoosterSystem.GetBoosterAmount(BoosterType.Timer).ToString();
        addgrillAmount.text = BoosterSystem.GetBoosterAmount(BoosterType.AddGrill).ToString();
    }
}
