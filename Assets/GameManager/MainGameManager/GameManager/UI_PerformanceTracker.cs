
using UnityEngine;
using UnityEngine.UI;

public class UI_PerformanceTracker : MonoBehaviour
{
    public Text fpsText;
    public Text memoryText;

    private float deltaTime = 0;
    [HideInInspector]
    public float fps;

    [HideInInspector]
    public long memoryUsage;

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        fps = Mathf.RoundToInt( 1.0f / deltaTime );

        memoryUsage = System.GC.GetTotalMemory(false) / (1024 * 1024);


        fpsText.text = $"FPS : {fps}";
        memoryText.text = $"Memory : {memoryUsage}";
    }
}
