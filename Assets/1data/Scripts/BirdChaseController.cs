using UnityEngine;

public class BirdChaseController : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRadius = 30f;
    public float chaseSpeed = 8f;
    public float patrolSpeed = 3f;
    
    [Header("Patrol Zone")]
    public Transform patrolCenter;
    public float patrolRadius = 20f;
    
    private Transform helicopter;
    private Rigidbody rb;
    private bool isChasing = false;
    private Vector3 patrolTarget;
    private float chaseTimeout = 5f;
    private float chaseTimer = 0f;
    
    void Start()
    {
        // Get Rigidbody
        rb = GetComponent<Rigidbody>();
        
        // Find helicopter
        GameObject heli = GameObject.FindGameObjectWithTag("Player");
        if (heli != null)
        {
            helicopter = heli.transform;
        }
        
        // Set initial patrol center
        if (patrolCenter == null)
        {
            patrolCenter = transform;
        }
        
        // Get first patrol target
        patrolTarget = GetRandomPatrolPoint();
    }
    
    void Update()
    {
        if (helicopter == null) return;
        
        float distanceToHelicopter = Vector3.Distance(transform.position, helicopter.position);
        
        // Check if should start chasing
        if (!isChasing && distanceToHelicopter < detectionRadius)
        {
            isChasing = true;
            chaseTimer = 0f;
        }
        
        // Chase behavior
        if (isChasing)
        {
            ChaseHelicopter();
            chaseTimer += Time.deltaTime;
            
            // Stop chasing after timeout or if too far
            if (chaseTimer > chaseTimeout || distanceToHelicopter > detectionRadius * 2f)
            {
                isChasing = false;
                patrolTarget = GetRandomPatrolPoint();
            }
        }
        else
        {
            // Patrol behavior
            Patrol();
        }
    }
    
    void ChaseHelicopter()
    {
        // Move towards helicopter using Rigidbody
        Vector3 direction = (helicopter.position - transform.position).normalized;
        
        if (rb != null)
        {
            rb.MovePosition(transform.position + direction * chaseSpeed * Time.deltaTime);
        }
        else
        {
            transform.position += direction * chaseSpeed * Time.deltaTime;
        }
        
        // Look at helicopter
        transform.LookAt(helicopter);
    }
    
    void Patrol()
    {
        // Move to patrol target using Rigidbody
        Vector3 newPosition = Vector3.MoveTowards(transform.position, patrolTarget, patrolSpeed * Time.deltaTime);
        
        if (rb != null)
        {
            rb.MovePosition(newPosition);
        }
        else
        {
            transform.position = newPosition;
        }
        
        // Look where moving
        Vector3 direction = patrolTarget - transform.position;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        // Get new target when reached
        if (Vector3.Distance(transform.position, patrolTarget) < 2f)
        {
            patrolTarget = GetRandomPatrolPoint();
        }
    }
    
    Vector3 GetRandomPatrolPoint()
    {
        // Random point in patrol radius
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        Vector3 point = patrolCenter.position + new Vector3(randomCircle.x, Random.Range(-5f, 5f), randomCircle.y);
        return point;
    }
    
    public void OnHitHelicopter()
    {
        // Called by BirdCollision script
        // Relocate far from helicopter
        RelocateBird();
    }
    
    void RelocateBird()
    {
        if (helicopter == null) return;
        
        // Find point far from helicopter (50-80 units away)
        Vector3 awayDirection = (transform.position - helicopter.position).normalized;
        Vector3 newPosition = helicopter.position + awayDirection * Random.Range(50f, 80f);
        
        // Keep similar height
        newPosition.y = Random.Range(25f, 40f);
        
        transform.position = newPosition;
        
        // Reset states
        isChasing = false;
        patrolTarget = GetRandomPatrolPoint();
        
        // Re-enable bird
        Invoke("ReactivateBird", 2f);
    }
    
    void ReactivateBird()
    {
        gameObject.SetActive(true);
    }
    
    // Draw detection radius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        Gizmos.color = Color.blue;
        if (patrolCenter != null)
        {
            Gizmos.DrawWireSphere(patrolCenter.position, patrolRadius);
        }
    }
}