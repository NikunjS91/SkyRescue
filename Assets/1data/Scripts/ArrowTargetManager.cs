using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ArrowTargetManager : MonoBehaviour
{
    [Header("Arrow")]
    public LookAtTargetController arrowController;
    
    [Header("Targets")]
    public Transform hospital;
    
    [Header("Empty Target (for positioning)")]
    public Transform dynamicTarget;
    
    private Transform helicopter;
    private List<GameObject> activeRescuePeople = new List<GameObject>();
    
    void Start()
    {
        helicopter = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Create dynamic target if not assigned
        if (dynamicTarget == null)
        {
            GameObject targetObj = new GameObject("ArrowTarget");
            dynamicTarget = targetObj.transform;
        }
        
        // Find all rescue people at start
        FindAllRescuePeople();
        UpdateArrowTarget();
    }
    
    void Update()
    {
        UpdateArrowTarget();
    }
    
    void FindAllRescuePeople()
    {
        // Find all objects with RescuePerson script
        RescuePerson[] allPeople = FindObjectsOfType<RescuePerson>();
        
        activeRescuePeople.Clear();
        foreach (RescuePerson person in allPeople)
        {
            if (person.gameObject.activeInHierarchy)
            {
                activeRescuePeople.Add(person.gameObject);
            }
        }
        
        Debug.Log("Found " + activeRescuePeople.Count + " rescue people");
    }
    
    void UpdateArrowTarget()
    {
        if (GameManager.Instance == null || helicopter == null) return;
        
        // Update list - remove inactive (rescued) people
        activeRescuePeople.RemoveAll(item => item == null || !item.activeInHierarchy);
        
        Transform targetToPoint;
        
        // If all rescued, point to hospital
        if (activeRescuePeople.Count == 0 && GameManager.Instance.rescuedCount > 0)
        {
            targetToPoint = hospital;
            Debug.Log("Arrow pointing to HOSPITAL");
        }
        else if (activeRescuePeople.Count > 0)
        {
            // Point to nearest rescue person
            targetToPoint = FindNearestRescue();
            Debug.Log("Arrow pointing to nearest person. " + activeRescuePeople.Count + " people remaining");
        }
        else
        {
            targetToPoint = hospital;
        }
        
        // Update dynamic target position
        if (targetToPoint != null && dynamicTarget != null)
        {
            dynamicTarget.position = targetToPoint.position;
            
            // Set arrow to look at dynamic target
            if (arrowController != null)
            {
                arrowController.Target = dynamicTarget;
            }
        }
    }
    
    Transform FindNearestRescue()
    {
        if (activeRescuePeople.Count == 0) return hospital;
        
        GameObject nearest = activeRescuePeople
            .OrderBy(x => Vector3.Distance(helicopter.position, x.transform.position))
            .FirstOrDefault();
        
        return nearest != null ? nearest.transform : hospital;
    }
}