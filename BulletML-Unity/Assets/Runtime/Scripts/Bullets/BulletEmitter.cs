using BulletML;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

namespace UnityBulletML.Bullets
{
    public class BulletEmitter : MonoBehaviour
    {
        public TextAsset PatternFile;
        public BulletManager BulletManager;

        private BulletPattern _pattern;
        private Bullet _rootBullet;
        // Cache transform
        private Transform _transform = null;

        void Start()
        {
            if (PatternFile == null)
                throw new System.Exception("No pattern assigned to the emitter.");

            _transform = transform;

            ParsePattern();
            AddBullet();
        }

        void Update()
        {
            if (_transform != null && transform.hasChanged)
            {
                _rootBullet.SetPosition(transform.position);
            }
        }

        public void AddBullet(bool clear = false)
        {
            if (clear)
                BulletManager.Clear();

            _rootBullet = (Bullet)BulletManager.CreateBullet(true);

            if (_rootBullet != null)
            {
                _rootBullet.InitTopNode(_pattern.RootNode);
                _rootBullet.SetDirection(transform.localRotation.eulerAngles.z);
                _rootBullet.SetPosition(transform.position);
            }
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

            loadedPattern = new BulletPattern();
            loadedPattern.ParseStream(patternFile.name, fileStream);
            //loadedPattern.ParsePattern(reader, patternFile.name);

            Debug.Log("Pattern loaded: " + loadedPattern.Filename);

            return loadedPattern;
        }
    }
}