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
    private bool _isPaused;

    void Update()
    {
        if (!_isRunning || _isPaused) return;

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
        _isPaused = false;
        _timeRemaining = seconds;
        
        UpdateUI();
    }

    public void PauseTimer()
    {
        if (!_isRunning) return;
        _isPaused = true;
    }

    public void ResumeTimer()
    {
        if (!_isRunning) return;
        _isPaused = false;
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
