using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryManager : MonoBehaviour
{
    public TextMeshProUGUI statsText;
    
    void Start()
    {
            AudioSource[] allAudio = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in allAudio)
    {
        if (audio.gameObject.name != "VictoryMusic")
            audio.Stop();
    }
        DisplayStats();
    }
    
    void DisplayStats()
    {
        if (statsText != null)
        {
            float totalTime = LevelManager.GetTotalTime();
            int minutes = Mathf.FloorToInt(totalTime / 60f);
            int seconds = Mathf.FloorToInt(totalTime % 60f);
            
            float l1Time = LevelManager.level1Time;
            int l1Min = Mathf.FloorToInt(l1Time / 60f);
            int l1Sec = Mathf.FloorToInt(l1Time % 60f);
            
            float l2Time = LevelManager.level2Time;
            int l2Min = Mathf.FloorToInt(l2Time / 60f);
            int l2Sec = Mathf.FloorToInt(l2Time % 60f);
            
            float l3Time = LevelManager.level3Time;
            int l3Min = Mathf.FloorToInt(l3Time / 60f);
            int l3Sec = Mathf.FloorToInt(l3Time % 60f);
            
            statsText.text = string.Format(
                "TOTAL RESCUES: {0}/15\n\n" +
                "TOTAL TIME: {1:00}:{2:00}\n\n" +
                "LEVEL 1: {3:00}:{4:00}\n" +
                "LEVEL 2: {5:00}:{6:00}\n" +
                "LEVEL 3: {7:00}:{8:00}\n\n" +
                "CONGRATULATIONS!",
                LevelManager.totalRescues,
                minutes, seconds,
                l1Min, l1Sec,
                l2Min, l2Sec,
                l3Min, l3Sec
            );
        }
    }
    
    public void PlayAgain()
    {
        LevelManager.currentLevel = 1;
        SceneManager.LoadScene("Level1");
    }
    
    public void ReturnToMainMenu()
    {
        LevelManager.ReturnToMenu();
    }
}