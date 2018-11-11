using UnityEngine;
using UnityEngine.Events;

namespace UnityBulletML.Bullets
{
    public class OnCollisionEvent : UnityEvent<Bullet> { };

    public class BulletPhysics : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CircleCollider2D _playerCollider = null;
        [SerializeField] private BulletManager _bulletManager = null;

        [Header("Parameters")]
        [SerializeField] private bool _destroyBulletOnCollision = true;

        private OnCollisionEvent _onCollision = new OnCollisionEvent();

        #region Properties

        public OnCollisionEvent OnCollision => _onCollision;

        #endregion

        // Cache the player transform
        private Transform _playerTransform;

        private void Start()
        {
            _playerTransform = _playerCollider.transform;
        }

        void Update()
        {
            CheckPlayerCollision();
        }

        private void CheckPlayerCollision()
        {
            for (int i = 0; i < _bulletManager.Bullets.Count; i++)
            {
                var currentBullet = _bulletManager.Bullets[i];

                // Ignore top bullets
                if (currentBullet.IsTopBullet() || !currentBullet.Used)
                {
                    continue;
                }

                var bulletPosition = new Vector3(currentBullet.Position.x, currentBullet.Position.y, 0f) / _bulletManager.PixelPerUnit;

                var dx = (currentBullet.Position.x / _bulletManager.PixelPerUnit) - _playerTransform.position.x;
                var dy = (currentBullet.Position.y / _bulletManager.PixelPerUnit) - _playerTransform.position.y;
                var radius = (currentBullet.GetProfile().CollisionRadius * _bulletManager.BulletInitialSize) + _playerCollider.radius;

                if ((dx * dx) + (dy * dy) < radius * radius)
                {
                    if (_destroyBulletOnCollision)
                        currentBullet.Used = false;

                    _onCollision.Invoke(currentBullet);
                }
            }
        }
    }
}
