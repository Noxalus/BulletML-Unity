using UnityEngine;

public class Bullet : BulletML.Bullet
{
    public Vector2 Position;

    public override float X
    {
        get { return Position.x; }
        set { Position.x = value; }
    }

    public override float Y
    {
        get { return Position.y; }
        set { Position.y = value; }
    }

    public Matrix4x4 TransformMatrix
    {
        get
        {
            return Matrix4x4.TRS(
                Position / _bulletManager.BulletsTexture.pixelsPerUnit, 
                Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * Direction + 180f), 
                Vector3.one * Scale
            );
        }
    }

    public bool Used { get; set; }

    private BulletManager _bulletManager;

    public Bullet(BulletML.IBulletManager bulletManager) : base(bulletManager)
    {
        _bulletManager = bulletManager as BulletManager;
    }

    public void Init()
    {
        Used = true;
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if (CheckOutOfBound())
            Used = false;
    }

    public bool CheckOutOfBound()
    {
        var screenSpacePosition = Camera.main.WorldToViewportPoint(Position / _bulletManager.BulletsTexture.pixelsPerUnit);

        return 
            !((screenSpacePosition.x >= 0 && screenSpacePosition.x <= 1) &&
            (screenSpacePosition.y >= 0 && screenSpacePosition.y <= 1));
    }
}
