using UnityEngine;

public class AddColliders : MonoBehaviour
{
    void Start()
    {
        // Find all objects with "building" in name
        GameObject[] buildings = GameObject.FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in buildings)
        {
            if (obj.name.ToLower().Contains("building") || 
                obj.name.ToLower().Contains("house") ||
                obj.name.ToLower().Contains("city"))
            {
                if (obj.GetComponent<Collider>() == null)
                {
                    obj.AddComponent<MeshCollider>();
                }
            }
        }
        
        Debug.Log("Colliders added!");
    }
}