using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HomeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _cointxt;
    public void GoToGame() => LoadingScene.Instance.GoToGame();

    void Start()
    {
        UpdateUI();
        BoosterSystem.OnGoldChanged += UpdateUI;
    }

    public void UpdateUI()
    {
        _cointxt.text = BoosterSystem.Gold.ToString();
    }
}
