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
    [SerializeField] GameObject backButton;
    private int currentlySelectedLevel = 1;

    PlayerControls controls;


    private void Awake()
    {
        controls = new PlayerControls();

        controls.UI.Navigate.performed += ctx =>
        {
            if (EventSystem.current.currentSelectedGameObject.Equals(backButton))
            {
                if (ctx.ReadValue<Vector2>().x > 0)
                {
                    NextLevel();
                }
                else if (ctx.ReadValue<Vector2>().x < 0)
                {
                    PreviousLevel();
                }
            }
        };
        controls.UI.Cancel.performed += ctx =>
        {
            ReturnToLevelSelect();
        };
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
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
