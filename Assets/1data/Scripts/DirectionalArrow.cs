using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DirectionalArrow : MonoBehaviour
{
    [Header("Targets")]
    public Transform hospital;
    public List<GameObject> rescueLocations = new List<GameObject>();
    
    [Header("Settings")]
    public float rotationSpeed = 5f;
    public float heightAboveHelicopter = 3f;
    
    private Transform currentTarget;
    private Transform helicopter;
    private Vector3 localOffset;
    
    void Start()
    {
        // Find helicopter
        helicopter = GetHelicopter();
        
        if (helicopter != null)
        {
            // Store initial local offset
            localOffset = transform.localPosition;
        }
        
        // Update rescue list
        UpdateRescueList();
        FindNearestRescue();
    }
    
    Transform GetHelicopter()
    {
        // Try parent first
        if (transform.parent != null)
            return transform.parent;
        
        // Otherwise find by tag
        GameObject heli = GameObject.FindGameObjectWithTag("Player");
        if (heli != null)
            return heli.transform;
        
        return null;
    }
    
    void LateUpdate()
    {
        // Update target
        UpdateTarget();
        
        // Keep arrow at fixed local position relative to helicopter
        if (helicopter != null)
        {
            transform.localPosition = localOffset;
        }
        
        // Point at target
        if (currentTarget != null)
        {
            PointAtTarget();
        }
    }
    
    void UpdateTarget()
    {
        if (GameManager.Instance == null) return;
        
        // Update rescue list
        UpdateRescueList();
        
        // If all rescued, point to hospital
        if (rescueLocations.Count == 0 && GameManager.Instance.rescuedCount > 0)
        {
            currentTarget = hospital;
        }
        else if (rescueLocations.Count > 0)
        {
            // Point to nearest rescue
            FindNearestRescue();
        }
    }
    
    void UpdateRescueList()
    {
        rescueLocations.RemoveAll(item => item == null || !item.activeInHierarchy);
    }
    
    void FindNearestRescue()
    {
        if (rescueLocations.Count == 0 || helicopter == null)
        {
            currentTarget = hospital;
            return;
        }
        
        GameObject nearest = rescueLocations
            .OrderBy(x => Vector3.Distance(helicopter.position, x.transform.position))
            .FirstOrDefault();
        
        if (nearest != null)
        {
            currentTarget = nearest.transform;
        }
    }
    
    void PointAtTarget()
    {
        // Direction to target (horizontal only)
        Vector3 direction = currentTarget.position - transform.position;
        direction.y = 0;
        
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            // Only rotate Y axis, keep arrow pointing down
            Vector3 currentEuler = transform.localEulerAngles;
            Vector3 targetEuler = targetRotation.eulerAngles;
            
            transform.rotation = Quaternion.Euler(
                currentEuler.x, 
                Mathf.LerpAngle(transform.eulerAngles.y, targetEuler.y, rotationSpeed * Time.deltaTime),
                currentEuler.z
            );
        }
    }
}
