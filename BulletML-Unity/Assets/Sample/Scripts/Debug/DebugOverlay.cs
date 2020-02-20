using TMPro;
using UnityBulletML.Bullets;
using UnityEngine;

namespace UnityBulletMLSample
{
    public class DebugOverlay : MonoBehaviour
    {
        public TextMeshProUGUI FPSCounterText;
        public TextMeshProUGUI BulletCounterText;
        public BulletManager BulletManager;
        public CanvasGroup CanvasGroup;

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
            BulletCounterText.text = string.Format("Bullets: " + BulletManager.Bullets.Count.ToString());
        }

        public void Show(bool value)
        {
            CanvasGroup.alpha = value ? 1 : 0;
        }
    }
}