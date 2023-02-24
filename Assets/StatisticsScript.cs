using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StatisticsScript : MonoBehaviour
{
    [SerializeField] TMP_Text levelButtonText;
    [SerializeField] TMP_Text statsText;
    [SerializeField] GameObject levelSelectScreen;
    [SerializeField] GameObject levelSelectFirstButton;
    private int currentlySelectedLevel = 1;


    // Start is called before the first frame update
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateLevelText()
    {
        levelButtonText.text = "Level " + currentlySelectedLevel;
    }
    public void UpdateStatsText()
    {
        string fastestTime;
        int timesCompleted;
            fastestTime = "NA";
            timesCompleted = 0;
        if (PlayerPrefs.HasKey("level" + currentlySelectedLevel + "FastestTimeString")) {
            fastestTime = PlayerPrefs.GetString("level" + currentlySelectedLevel + "FastestTimeString");
            timesCompleted = PlayerPrefs.GetInt("level" + currentlySelectedLevel + "TimesCompleted");
        } else
        {
        }
        statsText.text = "Fastest Time: " + fastestTime + "\nTimes Completed: " + timesCompleted;
    }

    public void NextLevel()
    {
        currentlySelectedLevel++;
        currentlySelectedLevel = Mathf.Clamp(currentlySelectedLevel, 1, SceneManager.sceneCountInBuildSettings -1);
        UpdateLevelText();
        UpdateStatsText();
    }
    public void PreviousLevel()
    {
        currentlySelectedLevel--;
        currentlySelectedLevel = Mathf.Clamp(currentlySelectedLevel, 1, SceneManager.sceneCountInBuildSettings -1);
        UpdateLevelText();
        UpdateStatsText();
    }

    public void ReturnToLevelSelect()
    {
        levelSelectScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(levelSelectFirstButton);

        gameObject.SetActive(false);
    }
}
