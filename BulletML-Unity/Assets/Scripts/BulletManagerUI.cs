using UnityEngine;
using UnityEngine.UI;

public class BulletManagerUI : MonoBehaviour {
    public BulletManager BulletManager;

    private Text _text;

    private void Start()
    {
        _text = GetComponent<Text>();
    }

    void Update()
    {
        _text.text = "Bullets: " + BulletManager.BulletsCount() + " (FPS: " + (1f / Time.deltaTime) + ")";
    }
}
