using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public int rescuedCount = 0;
    public int totalToRescue = 5;
    
    public float timeRemaining = 90f;
    public bool timerRunning = true;
    public bool gameOver = false;
    
    [Header("High Score")]
    public float bestTimeRemaining = 0f;
    private bool isNewRecord = false;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        LoadHighScore();
    }
    
    void Update()
    {
        if (timerRunning && !gameOver)
        {
            timeRemaining -= Time.deltaTime;
            
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateTimer(timeRemaining);
            
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                timerRunning = false;
                GameOver(false);
            }
        }
    }
    
    public void AddRescuedPerson()
    {
        rescuedCount++;
        
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateRescueCount(rescuedCount, totalToRescue);
    }
    
    public void DeliverPeople()
    {
        if (rescuedCount >= totalToRescue)
        {
            GameOver(true);
            // Removed auto-loading - player uses buttons now!
        }
        else
        {
            rescuedCount = 0;
        }
    }
    
    void GameOver(bool won)
    {
        gameOver = true;
        timerRunning = false;
        
        if (won)
        {
            SaveHighScore(timeRemaining);
            LevelManager.SaveLevelStats(LevelManager.currentLevel, timeRemaining, rescuedCount);
            
            if (UIManager.Instance != null)
                UIManager.Instance.ShowWinScreen();
        }
        else
        {
            if (UIManager.Instance != null)
                UIManager.Instance.ShowLoseScreen();
        }
    }
    
    public void ReturnToMainMenu()
    {
        LevelManager.ReturnToMenu();
    }
    
    public float GetBestTime()
    {
        return bestTimeRemaining;
    }
    
    public bool IsNewRecord()
    {
        return isNewRecord;
    }
    
    void LoadHighScore()
    {
        bestTimeRemaining = PlayerPrefs.GetFloat("BestTimeRemaining", 0f);
    }
    
    void SaveHighScore(float timeRemaining)
    {
        if (timeRemaining > bestTimeRemaining)
        {
            bestTimeRemaining = timeRemaining;
            isNewRecord = true;
            PlayerPrefs.SetFloat("BestTimeRemaining", bestTimeRemaining);
            PlayerPrefs.Save();
        }
        else
        {
            isNewRecord = false;
        }
    }
}