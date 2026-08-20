using UnityEngine;

public class SpeedTracker : MonoBehaviour
{
    private Rigidbody rb;
    
    void Start()
    {
        // Auto-find Rigidbody on this object or parent
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            rb = GetComponentInParent<Rigidbody>();
        }
        
        if (rb == null)
        {
            Debug.LogError("SpeedTracker: No Rigidbody found on " + gameObject.name);
        }
        else
        {
            Debug.Log("SpeedTracker: Tracking speed for " + gameObject.name);
        }
    }
    
    void Update()
    {
        if (UIManager.Instance != null && rb != null)
        {
            float currentSpeed = rb.linearVelocity.magnitude;
            UIManager.Instance.UpdateSpeed(currentSpeed);
        }
    }
}