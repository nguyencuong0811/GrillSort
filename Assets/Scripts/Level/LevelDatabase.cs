using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelDatabase", menuName ="Game/Level Database")]
public class LevelDatabase : ScriptableObject
{
    [Header("ListLevel")]
    [SerializeField] private List<LevelData> _levels;

    public List<LevelData> Levels => _levels;
}
