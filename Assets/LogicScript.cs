
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text finalTimerText;
    [SerializeField] GameObject levelCompletedScreen;
    private float startTime;
    private bool levelIsComplete;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        Application.targetFrameRate = 60;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!levelIsComplete)
        {
            timerText.text = "Time: " + GetTimerString();
        }
    }

    private string GetTimerString()
    {
        string minutes = ((int)Mathf.Floor((Time.time - startTime) / 60f)).ToString();
        if (minutes.Length < 2)
        {
            minutes = "0" + minutes;
        }
        if (minutes.Length < 2)
        {
            minutes = "0" + minutes;
        }
        string seconds = ((int)(Time.time - startTime) % 60).ToString();
        if (seconds.Length < 2)
        {
            seconds = "0" + seconds;
        }
        string milliseconds = ((int)(1000 * ((Time.time - startTime) % 1))).ToString();
        if (milliseconds.Length < 3)
        {
            milliseconds = "0" + milliseconds;
        }
        if (milliseconds.Length < 3)
        {
            milliseconds = "0" + milliseconds;
        }

        return minutes + ":" + seconds + ":" + milliseconds;
    }

    public void CompleteLevel()
    {
        finalTimerText.text = "Your Time was: " + GetTimerString();
        levelCompletedScreen.SetActive(true);
        levelIsComplete = true;

    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlayNextLevel() {
        int levelNum = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(levelNum + 1);
    }
}
