using UnityEngine;

public class RefuelStation : MonoBehaviour
{
    public float refuelRate = 50f;
    public AudioClip refuelSound;
    
    private bool helicopterInZone = false;
    private bool isRefueling = false;
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("=== TRIGGER ENTER ===");
        Debug.Log("Object name: " + other.gameObject.name);
        Debug.Log("Object tag: " + other.tag);
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("✅ HELICOPTER DETECTED!");
            helicopterInZone = true;
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowInstruction("Press R to Refuel");
            }
        }
        else
        {
            Debug.Log("❌ Not Player tag!");
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Still in zone
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        Debug.Log("=== TRIGGER EXIT ===");
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Helicopter left zone");
            helicopterInZone = false;
            isRefueling = false;
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowInstruction("");
            }
        }
    }
    
    void Update()
    {
        if (helicopterInZone && Input.GetKey(KeyCode.R))
        {
            Refuel();
        }
        else if (helicopterInZone && Input.GetKeyUp(KeyCode.R))
        {
            isRefueling = false;
        }
    }
    
    void Refuel()
    {
        if (FuelManager.Instance != null)
        {
            float refuelAmount = refuelRate * Time.deltaTime;
            FuelManager.Instance.currentFuel += refuelAmount;
            
            if (FuelManager.Instance.currentFuel > FuelManager.Instance.maxFuel)
            {
                FuelManager.Instance.currentFuel = FuelManager.Instance.maxFuel;
            }
            
            if (!isRefueling && refuelSound != null)
            {
                AudioSource.PlayClipAtPoint(refuelSound, transform.position);
            }
            
            isRefueling = true;
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowInstruction("REFUELING...");
            }
        }
    }
}