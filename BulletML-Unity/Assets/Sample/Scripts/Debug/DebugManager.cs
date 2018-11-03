using UnityBulletML.Bullets;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    public BulletEmitter BulletEmitter;

    public Text CurrentPatternText;

    public void Start()
    {
        CurrentPatternText.text = BulletEmitter.PatternFile.name;
    }

    public void NextPattern()
    {
    }

    public void PreviousPattern()
    {

    }
}
