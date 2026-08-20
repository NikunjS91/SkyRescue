using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel;
    public Button settingsButton;
    public Button closeButton;
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityLabel;
    
    [Header("Settings")]
    public float helicopterSensitivity = 1f;
    
    public static SettingsManager Instance;
    
    void Awake()
    {
        // Singleton (but don't persist across scenes)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Load saved settings
        LoadSettings();
    }
    
    void Start()
    {
        // Hide settings panel at start
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        
        // Setup buttons
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSettings);
        
        // Setup slider
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = helicopterSensitivity;
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            UpdateSensitivityLabel(helicopterSensitivity);
        }
    }
    
    void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }
    
    void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        
        SaveSettings();
    }
    
    void OnSensitivityChanged(float value)
    {
        helicopterSensitivity = value;
        UpdateSensitivityLabel(value);
    }
    
    void UpdateSensitivityLabel(float value)
    {
        if (sensitivityLabel != null)
        {
            sensitivityLabel.text = "Helicopter Sensitivity: " + value.ToString("F1");
        }
    }
    
    void SaveSettings()
    {
        PlayerPrefs.SetFloat("HelicopterSensitivity", helicopterSensitivity);
        PlayerPrefs.Save();
        Debug.Log("Settings saved! Sensitivity: " + helicopterSensitivity);
    }
    
    void LoadSettings()
    {
        if (PlayerPrefs.HasKey("HelicopterSensitivity"))
        {
            helicopterSensitivity = PlayerPrefs.GetFloat("HelicopterSensitivity");
            Debug.Log("Settings loaded! Sensitivity: " + helicopterSensitivity);
        }
        else
        {
            helicopterSensitivity = 1f;
        }
    }
    
    public float GetSensitivity()
    {
        return helicopterSensitivity;
    }
}