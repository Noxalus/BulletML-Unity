using BulletML;
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

        #region Getters/Setters

        public TextAsset PatternFile => _patternFile;

        public BulletManager BulletManager
        {
            get
            {
                return _bulletManager;
            }
            set
            {
                _bulletManager = value;
            }
        }

        #endregion

        void Start()
        {
            if (PatternFile != null)
            {
                LoadPattern();
                AddBullet();
            }
        }

        void Update()
        {
            // Make sure the pattern follows its related GameObject position
            if (_rootBullet != null && transform.hasChanged)
            {
                _rootBullet.SetPosition(transform.position);
            }
        }

        public void AddBullet(bool clear = false)
        {
            if (clear)
                _bulletManager.Clear();

            if (_pattern == null)
                throw new System.Exception("No pattern assigned to the emitter.");

            _rootBullet = (Bullet)_bulletManager.CreateBullet(true);

            if (_rootBullet != null)
            {
                _rootBullet.InitTopNode(_pattern.RootNode);
                _rootBullet.SetDirection(transform.localRotation.eulerAngles.z);
                _rootBullet.SetPosition(transform.position);
            }
        }

        public void SetPatternFile(TextAsset patternFile)
        {
            _patternFile = patternFile;
            LoadPattern();
        }

        public void SetPattern(BulletPattern pattern)
        {
            _pattern = pattern;
        }

        public void LoadPattern()
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(_patternFile.text))
            {
                Normalization = false,
                XmlResolver = null
            };

            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(_patternFile.text ?? ""));

            _pattern = new BulletPattern();
            _pattern.ParseStream(_patternFile.name, fileStream);
            //loadedPattern.ParsePattern(reader, patternFile.name);

            Debug.Log("Pattern loaded: " + _pattern.Filename);
        }
    }
}