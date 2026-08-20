using UnityEngine;

public class BirdCollision : MonoBehaviour
{
    public float timePenalty = 10f;
    public AudioClip collisionSound;
    private bool hasCollided = false;
    
    void OnTriggerEnter(Collider other)  // Changed from OnCollisionEnter
    {
        if (!hasCollided && other.CompareTag("Player"))
        {
            hasCollided = true;
            
            // Apply time penalty
            if (GameManager.Instance != null)
            {
                GameManager.Instance.timeRemaining -= timePenalty;
                if (GameManager.Instance.timeRemaining < 0)
                    GameManager.Instance.timeRemaining = 0;
            }
            
            // Play collision sound
            if (collisionSound != null)
            {
                AudioSource.PlayClipAtPoint(collisionSound, transform.position);
            }
            
            // Show message
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowInstruction("BIRD HIT! -10 SECONDS!");
            }
            
            // Tell chase controller to relocate
            BirdChaseController chaseController = GetComponent<BirdChaseController>();
            if (chaseController != null)
            {
                chaseController.OnHitHelicopter();
            }
            
            // Reset collision flag for next time
            hasCollided = false;
        }
    }
}