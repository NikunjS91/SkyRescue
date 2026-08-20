using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float distance = 12f;
    public float height = 5f;
    public float smoothSpeed = 5f;
    public float rotationSmoothSpeed = 2f;
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Position camera behind and above helicopter
        Vector3 desiredPosition = target.position - (target.right * distance) + (Vector3.up * height);

        

        
        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // Look at helicopter
        Vector3 lookTarget = target.position + Vector3.up * 2;
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime);
    }
}