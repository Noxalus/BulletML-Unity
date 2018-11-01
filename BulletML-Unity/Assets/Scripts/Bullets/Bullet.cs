using UnityBulletML.Bullets.Data;
using UnityEngine;

namespace UnityBulletML.Bullets
{
    public class Bullet : BulletML.Bullet
    {
        #region Properties

        private Vector2 _position;
        private BulletManager _bulletManager;

        #endregion

        #region Getters/Setters

        public Vector2 Position => _position;

        public override float X
        {
            get { return _position.x; }
            set { _position.x = value; }
        }

        public override float Y
        {
            get { return _position.y; }
            set { _position.y = value; }
        }

        public Matrix4x4 TransformMatrix
        {
            get
            {
                return Matrix4x4.TRS(
                    _position / _bulletManager.PixelPerUnit,
                    Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * Direction + 180f),
                    Vector3.one * Scale
                );
            }
        }

        public bool Used { get; set; }

        #endregion

        public Bullet(BulletML.IBulletManager bulletManager) : base(bulletManager)
        {
            _bulletManager = bulletManager as BulletManager;
        }

        public void Init()
        {
            Used = true;
        }

        // X/Y setters should only be used by the BulletML library as
        // they don't take into account the Unity pixel per unit value
        public void SetPosition(Vector2 position)
        {
            X = position.x * _bulletManager.PixelPerUnit;
            Y = position.y * _bulletManager.PixelPerUnit;
        }

        // TODO: Make it work
        public void SetDirection(float direction)
        {
            Direction = Mathf.Deg2Rad * (direction - 180f);
        }

        public BulletProfile GetProfile()
        {
            return _bulletManager.BulletProfiles[SpriteIndex];
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (IsOutOfBound())
                Used = false;
        }

        public bool IsOutOfBound()
        {
            var screenSpacePosition = Camera.main.WorldToViewportPoint(_position / _bulletManager.PixelPerUnit);

            return
                !((screenSpacePosition.x >= 0 && screenSpacePosition.x <= 1) &&
                (screenSpacePosition.y >= 0 && screenSpacePosition.y <= 1));
        }
    }
}