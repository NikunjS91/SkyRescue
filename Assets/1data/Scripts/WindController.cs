using UnityEngine;

public class WindController : MonoBehaviour
{
    [Header("Wind Settings")]
    public Vector3 windDirection = new Vector3(1, 0, 0);
    public float windStrength = 2f;
    public float gustStrength = 5f;
    public float gustFrequency = 3f;
    
    private Rigidbody helicopterRB;
    private float nextGustTime;
    
    void Start()
    {
        GameObject heli = GameObject.FindGameObjectWithTag("Player");
        if (heli != null)
        {
            helicopterRB = heli.GetComponent<Rigidbody>();
        }
        
        nextGustTime = Time.time + gustFrequency;
    }
    
    void FixedUpdate()
    {
        if (helicopterRB != null)
        {
            // Constant wind force
            Vector3 windForce = windDirection.normalized * windStrength;
            helicopterRB.AddForce(windForce, ForceMode.Force);
            
            // Random gusts
            if (Time.time >= nextGustTime)
            {
                ApplyGust();
                nextGustTime = Time.time + Random.Range(gustFrequency * 0.5f, gustFrequency * 1.5f);
            }
        }
    }
    
    void ApplyGust()
    {
        Vector3 gustDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-0.5f, 0.5f),
            Random.Range(-1f, 1f)
        ).normalized;
        
        Vector3 gustForce = gustDirection * gustStrength;
        helicopterRB.AddForce(gustForce, ForceMode.Impulse);
    }
}