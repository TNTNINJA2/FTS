using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AudioSettingsScript : MonoBehaviour
{
    GameObject levelSelectScreen;
    // Start is called before the first frame update
    void Start()
    {
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToLevelSelect()
    {
        levelSelectScreen.SetActive(true);
        gameObject.SetActive(false);
    }

    public void UpdateMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void UpdateSoundEffectVolume(float volume)
    {
        PlayerPrefs.SetFloat("soundEffectVolume", volume);
    }
}
