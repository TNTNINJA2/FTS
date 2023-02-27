using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectionScript : MonoBehaviour
{

    [SerializeField] TMP_Text levelButtonText;
    [SerializeField] GameObject statisticsScreen;
    [SerializeField] GameObject audioSettingsScreen;
    [SerializeField] GameObject levelSelectButton, statisticsFirstButton, audioFirstButton;
    private int currentlySelectedLevel = 1;

    float timeLastValidNavigate = 0;

    [SerializeField] PlayerControls controls;
    void Awake()
    {

        
        controls = new PlayerControls();

        controls.UI.Navigate.performed += ctx =>
        {
            if (Time.timeSinceLevelLoad > 0.1)
            {
                if (SceneManager.GetActiveScene().buildIndex == 0 && EventSystem.current.currentSelectedGameObject.Equals(levelSelectButton))
                {
                    if (timeLastValidNavigate + 0.1 < Time.time)
                    {
                        timeLastValidNavigate = Time.time;
                        if (ctx.ReadValue<Vector2>().x > 0)
                        {
                            NextLevel();
                        }
                        else if (ctx.ReadValue<Vector2>().x < 0)
                        {
                            PreviousLevel();
                        }
                    }
                }
            }
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

    // Update is called once per frame
    void Update()
    {
        
    }

    

    private void UpdateText()
    {
        levelButtonText.text = "Level " + currentlySelectedLevel;
    }

    public void NextLevel()
    {
        currentlySelectedLevel++;
        currentlySelectedLevel = Mathf.Clamp(currentlySelectedLevel, 1, SceneManager.sceneCountInBuildSettings - 1);
        UpdateText();
    }
    public void PreviousLevel()
    {
        currentlySelectedLevel--;
        currentlySelectedLevel = Mathf.Clamp(currentlySelectedLevel, 1, SceneManager.sceneCountInBuildSettings - 1);
        UpdateText();
    }

    public void LoadSelectedScene()
    {
        SceneManager.LoadScene(currentlySelectedLevel);
    }
    
    public void OpenStatistics()
    {
        statisticsScreen.GetComponent<StatisticsScript>().UpdateStatsText();
        statisticsScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(statisticsFirstButton);
        gameObject.SetActive(false);
    }

    public void OpenAudioSettings()
    {
        audioSettingsScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(audioFirstButton);
        Debug.Log(EventSystem.current.currentSelectedGameObject.name + " is currently selected");
        gameObject.SetActive(false);
    }
}
