using UnityEngine;

namespace UnityBulletML.Bullets
{
    public class BulletPhysics : MonoBehaviour
    {
        [SerializeField] private CircleCollider2D _playerCollider = null;
        [SerializeField] private BulletManager _bulletManager = null;

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
                var bulletPosition = new Vector3(currentBullet.Position.x, currentBullet.Position.y, 0f) / _bulletManager.PixelPerUnit;

                var dx = (currentBullet.Position.x / _bulletManager.PixelPerUnit) - _playerTransform.position.x;
                var dy = (currentBullet.Position.y / _bulletManager.PixelPerUnit) - _playerTransform.position.y;
                var radius = currentBullet.GetProfile().CollisionRadius + _playerCollider.radius;

                if ((dx * dx) + (dy * dy) < radius * radius)
                {
                    currentBullet.Used = false;
                }
            }
        }
    }
}
