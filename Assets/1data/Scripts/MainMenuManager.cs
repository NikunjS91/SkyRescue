using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string gameSceneName = "level1"; // Your game scene name
    public GameObject controlsPanel;
    public AudioSource menuMusic; // Moved inside class
    
    void Update()
    {
        // Press Space to start
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }
    
    void StartGame()
    {
        LevelManager.ResetToLevel1();
        // Stop menu music
        if (menuMusic != null)
        {
            menuMusic.Stop();
        }
        
        // Hide controls panel before loading
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
        
        // Load main game scene
        SceneManager.LoadScene(gameSceneName);
    }
}