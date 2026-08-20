using UnityEngine;

public class SpinMarker : MonoBehaviour
{
    public float spinSpeed = 100f;
    
    void Update()
    {
       transform.Rotate(spinSpeed * Time.deltaTime, 0, 0);
    }
}