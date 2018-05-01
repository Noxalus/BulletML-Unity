using BulletML;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

public class BulletEmitter : MonoBehaviour
{
    public TextAsset PatternFile;
    public BulletManager BulletManager;

    private BulletPattern _pattern;
    private Bullet _rootBullet;

    void Start()
    {
        if (PatternFile == null)
            throw new System.Exception("No pattern assigned to the emitter.");

        ParsePattern();

        AddBullet();
    }

    private void AddBullet(bool clear = false)
    {
        if (clear)
            BulletManager.Clear();

        _rootBullet = (Bullet)BulletManager.CreateBullet(true);
        _rootBullet.Position = transform.position;
        _rootBullet.InitTopNode(_pattern.RootNode);
    }

    public void ParsePattern()
    {
        _pattern = LoadPattern(PatternFile);
    }

    public static BulletPattern LoadPattern(TextAsset patternFile)
    {
        BulletPattern loadedPattern = null;

        XmlTextReader reader = new XmlTextReader(new StringReader(patternFile.text));
        reader.Normalization = false;
        reader.XmlResolver = null;

        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(patternFile.text ?? ""));

        Debug.Log("Pattern file content: " + patternFile.text);

        loadedPattern = new BulletPattern();
        loadedPattern.ParseStream(patternFile.text, fileStream);
        //loadedPattern.ParsePattern(reader, patternFile.name);

        Debug.Log("Pattern loaded: " + loadedPattern.Filename);
        Debug.Log("Pattern loaded: " + loadedPattern.Filename);
        Debug.Log("Pattern loaded root node: " + loadedPattern.RootNode.Label);

        return loadedPattern;
    }

    private void Update()
    {
        _rootBullet.Update();
    }
}
