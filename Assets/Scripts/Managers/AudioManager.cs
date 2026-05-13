using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    [SerializeField] private AudioClip _buyClick;


    [SerializeField] private AudioMixer _mixer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if(_musicSource.clip == clip) return;

        _musicSource.clip = clip;
        _musicSource.loop = loop;
        _musicSource.Play();
    }

    public void StartMusic()
    {
        _musicSource.Play();
    }

    public void FadeInMusic(AudioClip clip, float duration = 1f)
    {
        StartCoroutine(FadeIn(clip, duration));
    }

    public void FadeOutMusic(float duration = 1f)
    {
        StartCoroutine(FadeOut(duration));
    }

    private IEnumerator FadeIn(AudioClip clip, float duration)
    {
        _musicSource.clip = clip;
        _musicSource.volume = 0;
        _musicSource.Play();

        float t = 0;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            _musicSource.volume = t / duration;
            yield return null;
        }

        _musicSource.volume = 1;
    }



    private IEnumerator FadeOut(float duration)
    {
        float startVol = _musicSource.volume;
        float t = 0;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            _musicSource.volume = Mathf.Lerp(startVol, 0, t / duration);
            yield return null;
        }

        _musicSource.Stop();
        _musicSource.volume = startVol;
    }

    //set volume

    public void SetMusicVolume(float value)
    {
        _musicSource.volume = value;
    }

    public void SetSFXVolume(float value)
    {
        _sfxSource.volume = value;
    }

    //sfx

    public void PlaySFX(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        _sfxSource.PlayOneShot(clip, volume);
    }

    //mute

    public void MuteMusic(bool isMute)
    {
        _musicSource.mute = isMute;
    }

    public void MuteSFX(bool isMute)
    {
        _sfxSource.mute = isMute;
    }

    public void PlayBuyClick()
    {
        PlaySFX(_buyClick);
    }
}
