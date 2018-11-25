using UnityBulletML.Bullets.Data;
using UnityEngine;

namespace UnityBulletML.Bullets
{
    public class Bullet : BulletML.Bullet
    {
        #region Properties

        private BulletManager _bulletManager;
        private bool _topBullet;
        private Matrix4x4 _transformMatrix = Matrix4x4.identity;

        private float _x;
        private float _y;

        #endregion

        #region Getters/Setters

        public Vector2 Position
        {
            get
            {
                return new Vector2(_x, _y);
            }
        }

        public Matrix4x4 TransformMatrix
        {
            get
            {
                _transformMatrix.SetTRS(
                    Position,
                    Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * Rotation),
                    Vector3.one * Scale
                );

                return _transformMatrix;
            }
        }

        public bool Hidden = false;

        public bool Used { get; set; }

        #endregion

        #region BulletML.Bullet override

        public override float X
        {
            get { return _x * _bulletManager.PixelPerUnit; }
            set { _x = value / _bulletManager.PixelPerUnit; }
        }

        public override float Y
        {
            get { return _y * _bulletManager.PixelPerUnit; }
            set { _y = value / _bulletManager.PixelPerUnit; }
        }

        #endregion

        public Bullet(BulletML.IBulletManager bulletManager) : base(bulletManager)
        {
            _bulletManager = bulletManager as BulletManager;
        }

        public void Init(bool topBullet)
        {
            Used = true;
            Hidden = false;
            _topBullet = topBullet;
        }

        // X/Y setters should only be used by the BulletML library as
        // they don't take into account the Unity pixel per unit value
        public void SetPosition(Vector2 position)
        {
            X = position.x * _bulletManager.PixelPerUnit;
            Y = position.y * _bulletManager.PixelPerUnit;
        }

        public void SetDirection(float direction)
        {
            Rotation = Mathf.Deg2Rad * direction;
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
            var screenSpacePosition = Camera.main.WorldToViewportPoint(Position);

            return
                !(
                   screenSpacePosition.x >= _bulletManager.BulletsWidthBoundary.x &&
                   screenSpacePosition.x <= _bulletManager.BulletsWidthBoundary.y &&
                   screenSpacePosition.y >= _bulletManager.BulletsHeightBoundary.x &&
                   screenSpacePosition.y <= _bulletManager.BulletsHeightBoundary.y
                );
        }

        public bool IsTopBullet()
        {
            return _topBullet;
        }
    }
}