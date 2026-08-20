using UnityEngine;

public class RescuePerson : MonoBehaviour
{
    [Header("References")]
    public GameObject rescueMarker; // The spinning arrow
    
    [Header("Audio")]
    public AudioClip pickupSound;
    
    [Header("Visual Effects")]
    public GameObject rescueParticlePrefab;
    
    private bool isRescued = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isRescued)
        {
            Debug.Log("Press F to rescue!");
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isRescued)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                PickupPerson();
            }
        }
    }
    
    void PickupPerson()
    {
        isRescued = true;

        // Play pickup sound
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
        
        // Spawn particle effect
        if (rescueParticlePrefab != null)
        {
            GameObject particles = Instantiate(rescueParticlePrefab, transform.position, Quaternion.identity);
            Destroy(particles, 2f); // Destroy after 2 seconds
            Debug.Log("Rescue particles spawned!");
        }
        
        // Disable collider first!
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
        
        // Disable the marker's collider too
        if (rescueMarker != null)
        {
            Collider markerCol = rescueMarker.GetComponent<Collider>();
            if (markerCol != null)
                markerCol.enabled = false;
        }
        
        // Hide person and marker
        gameObject.SetActive(false);
        if (rescueMarker != null)
            rescueMarker.SetActive(false);
        
        Debug.Log("Person rescued!");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddRescuedPerson();
        }
    }
}