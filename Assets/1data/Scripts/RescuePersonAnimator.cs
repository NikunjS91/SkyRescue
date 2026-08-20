using UnityEngine;

public class RescuePersonAnimator : MonoBehaviour
{
    public Animator animator;
    public float waveDistance = 15f;
    
    private Transform helicopter;
    
    void Start()
    {
        GameObject heli = GameObject.FindGameObjectWithTag("Player");
        if (heli != null)
            helicopter = heli.transform;
            
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }
    
    void Update()
    {
        if (helicopter == null || animator == null) return;
        
        float distance = Vector3.Distance(transform.position, helicopter.position);
        
        // Look at helicopter when close
        if (distance < waveDistance)
        {
            Vector3 lookDir = helicopter.position - transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDir);
            
            animator.SetBool("isWaving", true);
        }
        else
        {
            animator.SetBool("isWaving", false);
        }
    }
}