using BulletML;
using UnityEngine;

namespace UnityBulletML.Bullets
{
    public class BulletEmitter : MonoBehaviour
    {
        #region Serialize fields

        [Header("References")]
        [SerializeField] private TextAsset _patternFile = null;
        [SerializeField] private BulletManager _bulletManager = null;

        [Header("Repeat")]
        [SerializeField] private bool _repeat = false;
        [SerializeField] private float _repeatFrequency = 1f;

        #endregion

        #region Fields

        private BulletPattern _pattern;
        private Bullet _rootBullet;
        private float _repeatTimer;

        #endregion

        #region Properties

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
                _pattern = _bulletManager.LoadPattern(PatternFile);
                AddBullet();
            }

            _repeatTimer = _repeatFrequency;
        }

        void Update()
        {
            // Make sure the pattern follows its related GameObject position
            if (_rootBullet != null && transform.hasChanged)
            {
                _rootBullet.SetPosition(transform.position);
            }

            if (_repeat)
            {
                _repeatTimer -= Time.deltaTime;

                if (_repeatTimer < 0)
                {
                    _repeatTimer = _repeatFrequency;
                    AddBullet();
                }
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
                _rootBullet.SetDirection(transform.rotation.eulerAngles.z);
                _rootBullet.SetPosition(transform.position);
                _rootBullet.InitTopNode(_pattern.RootNode);
            }
        }

        public void SetPattern(BulletPattern pattern)
        {
            _pattern = pattern;
        }
    }
}