using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string CURRENT_LEVEL_KEY = "CurrentLevel";
    public static int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 0);
    }
    public static void SetCurrentLevel(int level)
    {
        PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, level);
        PlayerPrefs.Save();
    }
    public static void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }
}
