using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioManagerScript : MonoBehaviour
{
    public static AudioManagerScript instance;
    [SerializeField] GameObject levelSelectScreen;
    [SerializeField] Scrollbar soundEffectScrollBar;
    [SerializeField] Scrollbar musicScrollBar;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource effectSource;


    private void Start()
    {
        instance = this;
        
        soundEffectScrollBar.onValueChanged.AddListener(ctx => UpdateSoundEffectVolume());
        musicScrollBar.onValueChanged.AddListener(ctx => UpdateMusicVolume());

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(musicSource.gameObject);
        DontDestroyOnLoad(effectSource.gameObject);
    }

    public void ReturnToLevelSelect()
    {
        levelSelectScreen.SetActive(true);
        gameObject.SetActive(false);
    }

    public void UpdateMusicVolume()
    {
        PlayerPrefs.SetFloat("musicVolume", musicScrollBar.value);
        musicSource.volume = musicScrollBar.value;
    }

    public void UpdateSoundEffectVolume()
    {
        effectSource.volume = soundEffectScrollBar.value;
        PlayerPrefs.SetFloat("soundEffectVolume", soundEffectScrollBar.value);
    }

    public void PlayEffect(AudioClip clip)
    {
        musicSource.PlayOneShot(clip);
    }
    public void PlayMusic(AudioClip clip)
    {
        musicSource.Stop();
        musicSource.PlayOneShot(clip);
    }

    
}
