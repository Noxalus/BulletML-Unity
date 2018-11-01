using UnityEngine;
using UnityEngine.UI;

public class DebugOverlay : MonoBehaviour
{
    public Text FPSCounterText;
    public Text BulletCounterText;
    public BulletManager BulletManager;

    private float _deltaTime = 0.0f;

    void Update()
    {
        // Update FPS counter
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        float ms = _deltaTime * 1000.0f;
        float fps = 1.0f / _deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", ms, fps);
        FPSCounterText.text = text;

        // Update bullet counter
        BulletCounterText.text = string.Format("Bullets: " + BulletManager.BulletsCount().ToString());
    }
}
