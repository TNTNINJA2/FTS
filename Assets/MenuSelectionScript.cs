using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuSelectionScript : MonoBehaviour
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
        Mathf.Clamp(currentlySelectedLevel, 1, 10);
        UpdateText();
    }
    public void PreviousLevel()
    {
        currentlySelectedLevel--;
        Mathf.Clamp(currentlySelectedLevel, 1, 10);
        UpdateText();
    }

    public void LoadSelectedScene()
    {
        SceneManager.LoadScene("Level " + currentlySelectedLevel, LoadSceneMode.Single);
    }
}
