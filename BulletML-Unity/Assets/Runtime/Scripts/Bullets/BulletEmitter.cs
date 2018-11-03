using BulletML;
using System;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

namespace UnityBulletML.Bullets
{
    public class BulletEmitter : MonoBehaviour
    {
        #region Serialize fields

        [SerializeField] private TextAsset _patternFile = null;
        [SerializeField] private BulletManager _bulletManager = null;

        #endregion

        #region Properties

        private BulletPattern _pattern;
        private Bullet _rootBullet;
        
        #endregion

        #region Getters

        public TextAsset PatternFile => _patternFile;
        public BulletManager BulletManager => _bulletManager;

        #endregion

        void Start()
        {
            if (PatternFile == null)
                throw new System.Exception("No pattern assigned to the emitter.");

            LoadPattern();
            AddBullet();
        }

        void Update()
        {
            // Make sure the pattern follows its related GameObject position
            if (transform.hasChanged)
            {
                _rootBullet.SetPosition(transform.position);
            }
        }

        public void AddBullet(bool clear = false)
        {
            if (clear)
                _bulletManager.Clear();

            _rootBullet = (Bullet)_bulletManager.CreateBullet(true);

            if (_rootBullet != null)
            {
                _rootBullet.InitTopNode(_pattern.RootNode);
                _rootBullet.SetDirection(transform.localRotation.eulerAngles.z);
                _rootBullet.SetPosition(transform.position);
            }
        }

        public void SetPattern(TextAsset patternFile)
        {
            _patternFile = patternFile;
            LoadPattern();
        }

        public void LoadPattern()
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(_patternFile.text));
            reader.Normalization = false;
            reader.XmlResolver = null;

            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(_patternFile.text ?? ""));

            _pattern = new BulletPattern();
            _pattern.ParseStream(_patternFile.name, fileStream);
            //loadedPattern.ParsePattern(reader, patternFile.name);

            Debug.Log("Pattern loaded: " + _pattern.Filename);
        }
    }
}