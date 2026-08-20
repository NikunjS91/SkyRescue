using UnityEngine;
using System.Collections;

public class HospitalDropZone : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip dropoffSound;
    
    [Header("Settings")]
    public float landingSpeedThreshold = 3f;
    public float soundDelayBeforeGameOver = 3f;
    
    private bool helicopterInZone = false;
    private bool isProcessingDropOff = false;
    private Rigidbody helicopterRB;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isProcessingDropOff)
        {
            helicopterInZone = true;
            helicopterRB = other.GetComponent<Rigidbody>();
            
            Debug.Log("Entered hospital zone!");
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && helicopterInZone && !isProcessingDropOff)
        {
            if (helicopterRB == null)
                helicopterRB = other.GetComponent<Rigidbody>();
            
            // Get current speed
            float currentSpeed = helicopterRB != null ? helicopterRB.linearVelocity.magnitude : 0f;
            
            // Update landing guidance (speed display handled by SpeedTracker)
            UpdateLandingGuidance(currentSpeed);
            
            // Check if slow enough to land
            if (currentSpeed < landingSpeedThreshold)
            {
                // Ready to drop off - press G
                if (Input.GetKeyDown(KeyCode.G))
                {
                    DropOffPeople();
                }
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            helicopterInZone = false;
            helicopterRB = null;
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowInstruction("Press F to Rescue | Press G to Drop Off");
            }
            
            Debug.Log("Left hospital zone");
        }
    }
    
    void UpdateLandingGuidance(float speed)
    {
        if (UIManager.Instance == null) return;
        
        // Speed display is handled by SpeedTracker - we only update instructions here
        
        // Check if player has rescued people
        if (GameManager.Instance == null || GameManager.Instance.rescuedCount <= 0)
        {
            UIManager.Instance.ShowInstruction("Rescue people first!");
            return;
        }
        
        // Show landing guidance based on speed
        if (speed < landingSpeedThreshold)
        {
            // Ready to land
            UIManager.Instance.ShowInstruction("LANDING READY - Press G to Drop Off");
        }
        else
        {
            // Too fast - need to slow down
            UIManager.Instance.ShowInstruction(
                string.Format("TOO FAST - SLOW DOWN! (Need Speed < {0:F0})", landingSpeedThreshold)
            );
        }
    }
    
    void DropOffPeople()
    {
        Debug.Log("=== DROP OFF TRIGGERED ===");
        
        if (isProcessingDropOff)
        {
            Debug.Log("Already processing!");
            return;
        }
        
        if (GameManager.Instance == null || GameManager.Instance.rescuedCount <= 0)
        {
            Debug.Log("No people to deliver!");
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowInstruction("No people rescued yet!");
            }
            return;
        }
        
        // Start drop-off
        isProcessingDropOff = true;
        helicopterInZone = false;
        
        int delivered = GameManager.Instance.rescuedCount;
        
        Debug.Log("Delivering " + delivered + " people!");
        
        // Play sound
        if (dropoffSound != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            audioSource.clip = dropoffSound;
            audioSource.volume = 1f;
            audioSource.spatialBlend = 0f;
            audioSource.Play();
            
            Debug.Log("Mission completed sound playing!");
        }
        
        // Update UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowInstruction("MISSION COMPLETE! All People Rescued!");
        }
        
        Debug.Log("Delivered " + delivered + " people to hospital!");
        
        // Delay game over
        StartCoroutine(DelayedGameOver());
    }
    
    IEnumerator DelayedGameOver()
    {
        Debug.Log("Waiting " + soundDelayBeforeGameOver + " seconds...");
        
        yield return new WaitForSeconds(soundDelayBeforeGameOver);
        
        Debug.Log("Triggering game over...");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DeliverPeople();
        }
    }
}