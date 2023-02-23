using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AudioManagerScript : MonoBehaviour
{
    public static AudioManagerScript instance;
    [SerializeField] GameObject audioSettingScreen;
    [SerializeField] GameObject levelSelectScreen;
    [SerializeField] Scrollbar soundEffectScrollBar;
    [SerializeField] Scrollbar musicScrollBar;
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource effectSource;

    [SerializeField] AudioClip[] musicClips; //i=0 is the menu and each subsequent one is a level
    


    private void Start()
    {

        if (PlayerPrefs.HasKey("musicVolume") && PlayerPrefs.HasKey("soundEffectVolume"))
        {
            musicScrollBar.value = PlayerPrefs.GetFloat("musicVolume");
            musicSource.volume = musicScrollBar.value;

            soundEffectScrollBar.value = PlayerPrefs.GetFloat("soundEffectVolume");
            effectSource.volume = soundEffectScrollBar.value;

        }
        else
        {
            PlayerPrefs.SetFloat("musicVolume", musicScrollBar.value);
            PlayerPrefs.SetFloat("soundEffectVolume", soundEffectScrollBar.value);
        }


        soundEffectScrollBar.onValueChanged.AddListener(ctx => UpdateSoundEffectVolume());
        musicScrollBar.onValueChanged.AddListener(ctx => UpdateMusicVolume());


        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(musicSource.gameObject);
        DontDestroyOnLoad(effectSource.gameObject);
        PlayMusic();
        if (instance != null)
        { 
            Destroy(instance.gameObject);
        }
        instance = this;
    }

    public void ReturnToLevelSelect()
    {
        levelSelectScreen.SetActive(true);
        audioSettingScreen.SetActive(false);
    }

    public void UpdateMusicVolume()
    {
        PlayerPrefs.SetFloat("musicVolume", musicScrollBar.value);
        musicSource.volume = musicScrollBar.value;
        PlayMusic();
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
    public void PlayMusic()
    {
        musicSource.Stop();
        musicSource.clip = musicClips[SceneManager.GetActiveScene().buildIndex];
        musicSource.Play();
        Debug.Log("Playing the clip: " + musicSource.clip.name);
    }

    private void OnDestroy()
    {
        Destroy(musicSource);
        Destroy(effectSource);
    }




}
