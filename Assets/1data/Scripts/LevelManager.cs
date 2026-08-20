using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int currentLevel = 1;
    
    public static float level1Time = 0f;
    public static float level2Time = 0f;
    public static float level3Time = 0f;
    public static int totalRescues = 0;
    
    public static void LoadNextLevel()
    {
        currentLevel++;
        
        Debug.Log("LoadNextLevel called! currentLevel = " + currentLevel);
        
        if (currentLevel == 2)
        {
            SceneManager.LoadScene("Level2");
        }
        else if (currentLevel == 3)
        {
            SceneManager.LoadScene("Level3");
        }
        else if (currentLevel == 4)
        {
            SceneManager.LoadScene("VictoryScene");
            currentLevel = 1; // Reset after victory
        }
        else
        {
            currentLevel = 1;
            SceneManager.LoadScene("MainMenu");
        }
    }
    
    public static void ReturnToMenu()
    {
        Debug.Log("ReturnToMenu - Resetting currentLevel to 1");
        currentLevel = 1;
        
        level1Time = 0f;
        level2Time = 0f;
        level3Time = 0f;
        totalRescues = 0;
        
        SceneManager.LoadScene("MainMenu");
    }
    
    public static void ResetToLevel1()
    {
        currentLevel = 1;
        Debug.Log("Reset to Level 1");
    }
    
    public static void SaveLevelStats(int level, float timeRemaining, int rescues)
    {
        if (level == 1)
            level1Time = 90f - timeRemaining;
        else if (level == 2)
            level2Time = 75f - timeRemaining;
        else if (level == 3)
            level3Time = 60f - timeRemaining;
        
        totalRescues += rescues;
    }
    
    public static float GetTotalTime()
    {
        return level1Time + level2Time + level3Time;
    }
}