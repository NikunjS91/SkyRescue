using UnityEngine;

public class FuelManager : MonoBehaviour
{
    public static FuelManager Instance;
    
    [Header("Fuel Settings")]
    public float maxFuel = 100f;
    [HideInInspector] public float currentFuel = 100f;
    public float fuelConsumptionRate = 10f; // per minute
    public float ascentFuelMultiplier = 2f; // uses more fuel going up
    
    private bool isOutOfFuel = false;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Update()
    {
        if (!isOutOfFuel && GameManager.Instance != null && !GameManager.Instance.gameOver)
        {
            // Consume fuel over time
            float consumption = (fuelConsumptionRate / 60f) * Time.deltaTime;
            
            // Extra consumption when ascending
            if (Input.GetKey(KeyCode.LeftShift))
            {
                consumption *= ascentFuelMultiplier;
            }
            
            currentFuel -= consumption;
            currentFuel = Mathf.Max(0, currentFuel);
            
            // Update UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateFuel(currentFuel, maxFuel);
            }
            
            // Out of fuel
            if (currentFuel <= 0 && !isOutOfFuel)
            {
                OutOfFuel();
            }
        }
    }
    
    void OutOfFuel()
    {
        isOutOfFuel = true;
        
        // Helicopter loses power
        if (GameManager.Instance != null)
        {
            GameManager.Instance.gameOver = true;
            GameManager.Instance.timerRunning = false;
        }
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowInstruction("OUT OF FUEL! MISSION FAILED!");
        }
        
        // Show lose screen after delay
        Invoke("ShowLoseScreen", 2f);
    }
    
    void ShowLoseScreen()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowLoseScreen();
        }
    }
    
    public void SetStartingFuel(float amount)
    {
        maxFuel = amount;
        currentFuel = amount;
    }
}