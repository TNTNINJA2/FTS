using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StatisticsScript : MonoBehaviour
{
    [SerializeField] TMP_Text levelButtonText;
    private int currentlySelectedLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        
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
        currentlySelectedLevel = Mathf.Clamp(currentlySelectedLevel, 1, SceneManager.sceneCountInBuildSettings -1);
        UpdateText();
    }
    public void PreviousLevel()
    {
        currentlySelectedLevel--;
        currentlySelectedLevel = Mathf.Clamp(currentlySelectedLevel, 1, SceneManager.sceneCountInBuildSettings -1);
        UpdateText();
    }
}
