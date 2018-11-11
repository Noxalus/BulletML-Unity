using BulletML;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityBulletML.Bullets.Data;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace UnityBulletML.Bullets
{
    public class BulletManager : MonoBehaviour, IBulletManager
    {
        // DrawMeshInstanced is limited to array of maximum 1023 elements
        public const int MAX_BATCH_AMOUNT = 1023;

        #region Serialized fields

        [Header("Gameplay")]
        [SerializeField] private float _difficulty = 0.5f;

        [Header("Pattern files")]
        [SerializeField] private string _patternFilesFolder = "";

        [Header("References")]
        [SerializeField] private Transform _playerTransform = null;

        [Header("Bullet's data")]
        [SerializeField] private int _maxBulletsAmount = 10000;
        [SerializeField] private float _bulletInitialSize = 1;
        [Tooltip("Min/Max in screen coordinates")]
        [SerializeField] private Vector2 _bulletsWidthBoundary = new Vector2(0f, 1f);
        [Tooltip("Min/Max in screen coordinates")]
        [SerializeField] private Vector2 _bulletsHeightBoundary = new Vector2(0f, 1f);
        [SerializeField] private Sprite _bulletsTexture = null;
        [SerializeField] private Vector2 _bulletsTextureTiling = new Vector2(0.25f, 0.25f);
        [SerializeField] private BulletProfile[] _bulletProfiles = null;

        #endregion

        #region Properties

        // Store bullets data in arrays to use DrawMeshInstanced method for rendering optimization
        private List<Matrix4x4[]> _bulletMatricesBatches = new List<Matrix4x4[]>();
        private List<Vector4[]> _bulletSpriteOffsetsBatches = new List<Vector4[]>();
        private List<Vector4[]> _bulletColorsBatches = new List<Vector4[]>();

        private List<Bullet> _bullets;
        private Queue<Bullet> _unusedBullets = new Queue<Bullet>();
        private Dictionary<string, BulletPattern> _bulletPatterns = new Dictionary<string, BulletPattern>();

        public float BulletInitialSize => _bulletInitialSize;
        public Vector2 BulletsWidthBoundary => _bulletsWidthBoundary;
        public Vector2 BulletsHeightBoundary => _bulletsHeightBoundary;

        #endregion

        #region Getters

        public List<Matrix4x4[]> BulletTransformMatrices => _bulletMatricesBatches;
        public List<Vector4[]> BulletSpriteOffsetsBatches => _bulletSpriteOffsetsBatches;
        public List<Vector4[]> BulletColorsBatches => _bulletColorsBatches;

        public float PixelPerUnit => _bulletsTexture.pixelsPerUnit;
        public List<Bullet> Bullets => _bullets;
        public BulletProfile[] BulletProfiles => _bulletProfiles;

        #endregion

        private float GetDifficulty()
        {
            return _difficulty;
        }

        void Awake()
        {
            GameManager.GameDifficulty = GetDifficulty;
            _bullets = new List<Bullet>(_maxBulletsAmount);
        }

        public void LoadPatterns()
        {
            var directoryInfo = new DirectoryInfo(_patternFilesFolder);
            var filesInfo = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);

            var resourcesFolder = "Resources/";
            var pathFromResourcesFolder = _patternFilesFolder.Substring(_patternFilesFolder.IndexOf(resourcesFolder) + resourcesFolder.Length);

            foreach (var fileInfo in filesInfo)
            {
                var fileExtension = Path.GetExtension(fileInfo.Name);

                if (fileExtension.Equals(".xml"))
                {
                    var subFolder = fileInfo.DirectoryName.Substring(fileInfo.DirectoryName.IndexOf(pathFromResourcesFolder) + pathFromResourcesFolder.Length);
                    subFolder += Path.DirectorySeparatorChar;

                    TextAsset patternFile = Resources.Load<TextAsset>(pathFromResourcesFolder + subFolder + Path.GetFileNameWithoutExtension(fileInfo.Name));

                    XmlTextReader reader = new XmlTextReader(new StringReader(patternFile.text))
                    {
                        Normalization = false,
                        XmlResolver = null
                    };

                    var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(patternFile.text ?? ""));

                    var pattern = new BulletPattern();
                    pattern.ParseStream(patternFile.name, fileStream);

                    _bulletPatterns.Add(patternFile.name, pattern);
                    Debug.Log("Found: " + fileInfo);
                }
            }
        }

        public BulletPattern GetPattern(string patternName)
        {
            if (!_bulletPatterns.ContainsKey(patternName))
                throw new System.Exception("No pattern found for this name: " + patternName);

            return _bulletPatterns[patternName];
        }

        #region IBulletManager implementation

        public BulletML.Vector2 PlayerPosition(IBullet targettedBullet)
        {
            return new BulletML.Vector2(_playerTransform.position.x * PixelPerUnit, _playerTransform.position.y * PixelPerUnit);
        }

        public IBullet CreateBullet(bool topBullet = false)
        {
            if (_bullets.Count >= _maxBulletsAmount)
                return null;

            Bullet bullet;

            if (_unusedBullets.Count > 0)
            {
                bullet = _unusedBullets.Dequeue();
            }
            else
            {
                bullet = new Bullet(this);
                _bullets.Add(bullet);
            }

            bullet.Init(topBullet);

            return bullet;
        }

        public void RemoveBullet(IBullet deadBullet)
        {
            var bullet = deadBullet as Bullet;

            if (bullet != null)
                bullet.Used = false;
        }

        #endregion

        public void FixedUpdate()
        {
            var _bulletsToRemove = new List<Bullet>();

            for (int i = 0; i < _bullets.Count; i++)
            {
                Bullet currentBullet = _bullets[i];

                currentBullet.Update(Time.fixedDeltaTime);

                int batchIndex = i / MAX_BATCH_AMOUNT;
                int elementIndex = i % MAX_BATCH_AMOUNT;

                // Do we need to create a new batch?
                if (_bulletMatricesBatches.Count <= batchIndex)
                {
                    _bulletMatricesBatches.Add(new Matrix4x4[MAX_BATCH_AMOUNT]);
                    _bulletSpriteOffsetsBatches.Add(new Vector4[MAX_BATCH_AMOUNT]);
                    _bulletColorsBatches.Add(new Vector4[MAX_BATCH_AMOUNT]);
                }

                if (!currentBullet.Used)
                {
                    _bulletsToRemove.Add(currentBullet);
                }
                else
                {
                    // We don't want to render top bullets
                    if (!currentBullet.IsTopBullet())
                    {
                        // Update current bullet's data arrays
                        _bulletMatricesBatches[batchIndex][elementIndex] = currentBullet.TransformMatrix;
                        _bulletSpriteOffsetsBatches[batchIndex][elementIndex] = GetTextureOffset(currentBullet.SpriteIndex);
                        _bulletColorsBatches[batchIndex][elementIndex] = new Vector4(
                            currentBullet.Color.R / 255f,
                            currentBullet.Color.G / 255f,
                            currentBullet.Color.B / 255f,
                            currentBullet.Color.A / 255f
                        );
                    }
                }
            }

            // Enqueue unused bullets
            foreach (var bullet in _bulletsToRemove)
            {
                // TODO: Make it work!
                //_unusedBullets.Enqueue(bullet);
            }

            _bulletsToRemove.Clear();
        }

        public void Clear()
        {
            _bullets.Clear();
            _bulletMatricesBatches.Clear();
            _bulletSpriteOffsetsBatches.Clear();
            _bulletColorsBatches.Clear();
        }

        #region Utils

        private Vector4 GetTextureOffset(int spriteIndex)
        {
            Vector4 textureOffset = new Vector4(_bulletsTextureTiling.x, _bulletsTextureTiling.y, 0, 0);
            int textureLineIndex = spriteIndex / Mathf.RoundToInt(1f / _bulletsTextureTiling.x);
            int textureColumnIndex = spriteIndex % Mathf.RoundToInt(1f / _bulletsTextureTiling.y);

            textureOffset.z = textureColumnIndex * _bulletsTextureTiling.y;
            textureOffset.w = (1f - _bulletsTextureTiling.x) - (textureLineIndex * _bulletsTextureTiling.x);

            return textureOffset;
        }

        #endregion
    }
}