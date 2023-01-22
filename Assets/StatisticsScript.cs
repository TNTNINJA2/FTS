using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StatisticsScript : MonoBehaviour
{
    [SerializeField] TMP_Text levelButtonText;
    [SerializeField] TMP_Text statsText;
    [SerializeField] GameObject levelSelectSCreen;
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
        if (PlayerPrefs.HasKey("level" + currentlySelectedLevel + "FastestTime")) {
            fastestTime = PlayerPrefs.GetString("level" + currentlySelectedLevel + "FastestTime");
            timesCompleted = PlayerPrefs.GetInt("level" + currentlySelectedLevel + "TimesCompleted");
        } else
        {
            fastestTime = "NA";
            timesCompleted = 0;
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
        levelSelectSCreen.SetActive(true);
        gameObject.SetActive(false);
    }
}
