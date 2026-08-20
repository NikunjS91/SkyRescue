using UnityEngine;
using TMPro;

public class BlinkText : MonoBehaviour
{
    public float blinkSpeed = 1f;
    private TextMeshProUGUI text;
    private float timer;
    
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    
    void Update()
    {
        timer += Time.deltaTime * blinkSpeed;
        
        // Fade in and out
        float alpha = (Mathf.Sin(timer) + 1f) / 2f; // 0 to 1
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
}