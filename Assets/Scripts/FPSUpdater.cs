using UnityEngine;
using TMPro;

public class FPSUpdater : MonoBehaviour
{
    float fps;
    float updateTimer = 0.2f;

    [SerializeField] TextMeshProUGUI fpsTitle;

    private void UpdateFPSDisplay()
    {
        updateTimer -= Time.deltaTime;
        if (updateTimer <= 0f)
        {
            fps = 1f / Time.unscaledDeltaTime;
            fpsTitle.text = "FPS: " + Mathf.Round(fps);
            updateTimer = 0.2f;
        }
    }

    private void Update()
    {
        UpdateFPSDisplay();
    }
}
