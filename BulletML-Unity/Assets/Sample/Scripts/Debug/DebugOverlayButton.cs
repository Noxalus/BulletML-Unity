using UnityEngine;
using UnityEngine.UI;

namespace UnityBulletML
{
    public class DebugOverlayButton : MonoBehaviour
    {
        public DebugOverlay DebugOverlay;
        public Text ButtonText;

        private bool _isDebugOverlayVisible;

        public void Start()
        {
            _isDebugOverlayVisible = true;
            ButtonText.text = "Hide";
        }

        public void OnButtonClick()
        {
            _isDebugOverlayVisible = !_isDebugOverlayVisible;
            DebugOverlay.Show(_isDebugOverlayVisible);
            ButtonText.text = _isDebugOverlayVisible ? "Hide" : "Show";
        }
    }
}
