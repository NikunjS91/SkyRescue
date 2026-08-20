using UnityEngine;

public class SimpleBirdFly : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 8f;
    public float turnSpeed = 2f;
    public float heightVariation = 5f;
    
    [Header("Boundaries")]
    public float roamRadius = 50f;
    public float minHeight = 20f;
    public float maxHeight = 50f;
    
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        
        startPosition = transform.position;
        PickNewTarget();
    }
    
    void FixedUpdate()
    {
        if (rb == null) return;
        
        // Move towards target
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 newPosition = transform.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
        
        // Smoothly rotate towards movement direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
        }
        
        // Check if reached target
        if (Vector3.Distance(transform.position, targetPosition) < 3f)
        {
            PickNewTarget();
        }
        
        // Keep within height bounds
        if (transform.position.y < minHeight || transform.position.y > maxHeight)
        {
            Vector3 correctedPos = transform.position;
            correctedPos.y = Mathf.Clamp(correctedPos.y, minHeight, maxHeight);
            transform.position = correctedPos;
            PickNewTarget();
        }
    }
    
    void PickNewTarget()
    {
        // Pick random point within roam radius from start position
        Vector2 randomCircle = Random.insideUnitCircle * roamRadius;
        
        targetPosition = startPosition + new Vector3(
            randomCircle.x,
            Random.Range(-heightVariation, heightVariation),
            randomCircle.y
        );
        
        // Clamp height
        targetPosition.y = Mathf.Clamp(targetPosition.y, minHeight, maxHeight);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply penalty
            if (GameManager.Instance != null)
            {
                GameManager.Instance.timeRemaining -= 10f;
                if (GameManager.Instance.timeRemaining < 0)
                    GameManager.Instance.timeRemaining = 0;
            }
            
            // Show message
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowInstruction("BIRD STRIKE! -10 SECONDS!");
            }
            
            // Relocate bird far away
            RelocateBird();
        }
    }
    
    void RelocateBird()
    {
        // Find helicopter
        GameObject heli = GameObject.FindGameObjectWithTag("Player");
        
        if (heli != null)
        {
            // Move far from helicopter
            Vector3 awayDirection = (transform.position - heli.transform.position).normalized;
            startPosition = heli.transform.position + awayDirection * Random.Range(60f, 100f);
            startPosition.y = Random.Range(minHeight, maxHeight);
            transform.position = startPosition;
        }
        else
        {
            // Random relocation
            startPosition = new Vector3(
                Random.Range(-80f, 80f),
                Random.Range(minHeight, maxHeight),
                Random.Range(-80f, 80f)
            );
            transform.position = startPosition;
        }
        
        // Pick new target
        PickNewTarget();
    }
}