using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerTxt;
    private float _timeRemaining;
    private bool _isRunning;

    void Update()
    {
        if(!_isRunning) return;

        _timeRemaining -= Time.deltaTime;
        
        if(_timeRemaining <= 0)
        {
            _timeRemaining = 0;
            _isRunning = false;
            UpdateUI();
            
            return;
        }
        UpdateUI();
    }

    public void StartTimer(float seconds)
    {
        _isRunning = true;
        _timeRemaining = seconds;
        
        UpdateUI();
    }

    public void UpdateUI()
    {   
        int minutes = Mathf.FloorToInt(_timeRemaining / 60);
        int seconds = Mathf.FloorToInt(_timeRemaining % 60);


        _timerTxt.text = minutes.ToString() + ": " + seconds.ToString();
    }

    public void AddBonusTime()
    {
        _timeRemaining += 120f;
    }
}
