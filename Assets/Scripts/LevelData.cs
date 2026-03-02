using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Level Infor")]
    [SerializeField] private int _levelIndex;

    [Header("GamePlay")]
    [SerializeField] private int _allFood;
    [SerializeField] private int _totalFoods;
    [SerializeField] private int _totalGrill;
    [SerializeField] private int _totalLidGrill;

    [Header("Random Range")]
    [SerializeField] private float _minAvgTray;
    [SerializeField] private float _maxAvgTray;


    public int Level => _levelIndex;
    public int AllFood => _allFood;
    public int TotalFood => _totalFoods;
    public int TotalLidGrill => _totalLidGrill;
    public int TotalGrill => _totalGrill;
    public float MinAvgTray => _minAvgTray;
    public float MaxAvgTray => _maxAvgTray;
}
