
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text finalTimerText;
    [SerializeField] GameObject levelCompletedScreen;
    [SerializeField] PlayerScript playerScript;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] AudioClip music = null;

    private float timeRunning = 0;
    private bool levelIsComplete;
    private bool isPaused;
    private PlayerControls controls;
    private AudioManagerScript audioManagerScript;


    private void Awake()
    {
        audioManagerScript = AudioManagerScript.instance;

        controls = new PlayerControls();

        controls.UI.Pause.performed += ctx =>
        {
            SetPauseLevel(true);
        };
    }
    void Start()
    {
        Application.targetFrameRate = 60;

        playerScript = GameObject.Find("Player").GetComponent<PlayerScript>();
        audioManagerScript.PlayMusic();


    }

    // Update is called once per frame
    void Update()
    {
        if (!levelIsComplete)
        {
            timerText.text = "Time: " + GetTimerString();
        }
        if (!isPaused)
        {
            timeRunning += Time.deltaTime;
        }
    }

    private string GetTimerString()
    {
        string minutes = ((int)Mathf.Floor((timeRunning) / 60f)).ToString();
        if (minutes.Length < 2)
        {
            minutes = "0" + minutes;
        }
        if (minutes.Length < 2)
        {
            minutes = "0" + minutes;
        }
        string seconds = ((int)(timeRunning) % 60).ToString();
        if (seconds.Length < 2)
        {
            seconds = "0" + seconds;
        }
        string milliseconds = ((int)(1000 * ((timeRunning) % 1))).ToString();
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
        timerText.text = "Time: " + GetTimerString();
        finalTimerText.text = "Your Time was: " + GetTimerString();
        levelCompletedScreen.SetActive(true);
        levelIsComplete = true;

        if (!PlayerPrefs.HasKey("level" + SceneManager.GetActiveScene().buildIndex + "FastestTimeFloat") ||
            PlayerPrefs.GetFloat("level" + SceneManager.GetActiveScene().buildIndex + "FastestTimeFloat") > timeRunning)
        {
            PlayerPrefs.SetFloat("level" + SceneManager.GetActiveScene().buildIndex + "FastestTimeFloat", timeRunning);
            PlayerPrefs.SetString("level" + SceneManager.GetActiveScene().buildIndex + "FastestTimeString", GetTimerString());
        }
        if (PlayerPrefs.HasKey("level" + SceneManager.GetActiveScene().buildIndex + "TimesCompleted"))
        {
            PlayerPrefs.SetInt("level" + SceneManager.GetActiveScene().buildIndex + "TimesCompleted", PlayerPrefs.GetInt("level" + SceneManager.GetActiveScene().buildIndex + "TimesCompleted") + 1);
        } else
        {
            PlayerPrefs.SetInt("level" + SceneManager.GetActiveScene().buildIndex + "TimesCompleted", 1);
        }
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

    public void SetPauseLevel(bool value)
    {
        isPaused = value;
        playerScript.SetPause(value);
        pauseScreen.SetActive(value);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
