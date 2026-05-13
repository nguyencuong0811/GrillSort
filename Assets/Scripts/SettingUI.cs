using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        // add listener
        musicSlider.onValueChanged.AddListener(OnMusicChange);
        sfxSlider.onValueChanged.AddListener(OnSFXChange);
    }

    private void OnMusicChange(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSFXChange(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }
}