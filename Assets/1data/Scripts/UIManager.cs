using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI rescueText;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI speedText;
    public UnityEngine.UI.Image fuelBar; 
    public TextMeshProUGUI fuelText;
    
    [Header("Screens")]
    public GameObject winScreen;
    public GameObject loseScreen;
    
    [Header("Win Screen Elements")]
    public TextMeshProUGUI timeRemainingText;
    public TextMeshProUGUI bestTimeText;
    public GameObject newRecordText;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        if (winScreen != null) winScreen.SetActive(false);
        if (loseScreen != null) loseScreen.SetActive(false);
    }
    
    void Update()
    {
        // Space key to return to menu from win/lose screens
        if ((winScreen != null && winScreen.activeSelf) || (loseScreen != null && loseScreen.activeSelf))
        {
            if (Input.GetKeyDown(KeyCode.Space))
                ReturnToMainMenu();
        }
    }
    
    public void UpdateTimer(float time)
    {
        if (timerText == null) return;
        
        int seconds = Mathf.CeilToInt(time);
        timerText.text = seconds.ToString();
        
        if (seconds <= 10)
            timerText.color = Color.red;
        else if (seconds <= 30)
            timerText.color = Color.yellow;
        else
            timerText.color = Color.white;
    }
    
    public void UpdateRescueCount(int rescued, int total)
    {
        if (rescueText != null)
            rescueText.text = rescued + "/" + total;
    }
    
    public void ShowInstruction(string message)
    {
        if (instructionText != null)
            instructionText.text = message;
    }
    
    public void UpdateSpeed(float speed)
    {
        if (speedText == null) return;
        
        speedText.text = string.Format("Speed: {0:F1}", speed);
        
        if (speed > 5f)
            speedText.color = Color.red;
        else if (speed > 3f)
            speedText.color = Color.yellow;
        else
            speedText.color = Color.green;
    }
    
    public void ShowWinScreen()
    {
        if (winScreen == null) return;
        
        winScreen.SetActive(true);
        
        if (GameManager.Instance != null)
        {
            if (timeRemainingText != null)
                timeRemainingText.text = string.Format("Time Remaining: {0:F1}s", GameManager.Instance.timeRemaining);
            
            if (bestTimeText != null)
                bestTimeText.text = string.Format("Best Time: {0:F1}s", GameManager.Instance.GetBestTime());
            
            if (newRecordText != null)
                newRecordText.SetActive(GameManager.Instance.IsNewRecord());
        }
    }
    
    public void ShowLoseScreen()
    {
        if (loseScreen != null)
            loseScreen.SetActive(true);
    }

    public void UpdateFuel(float current, float max)
    {
        if (fuelBar != null)
        {
            float percentage = current / max;
            fuelBar.fillAmount = percentage;
            
            if (percentage > 0.5f)
                fuelBar.color = Color.green;
            else if (percentage > 0.25f)
                fuelBar.color = Color.yellow;
            else
                fuelBar.color = Color.red;
        }
        
        if (fuelText != null)
            fuelText.text = string.Format("Fuel: {0:F0}%", (current / max) * 100);
    }
    
    // Button functions
    public void ReplayLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void NextLevel()
    {
        Time.timeScale = 1;
        LevelManager.LoadNextLevel();
    }
    
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        LevelManager.ReturnToMenu();
    }
}