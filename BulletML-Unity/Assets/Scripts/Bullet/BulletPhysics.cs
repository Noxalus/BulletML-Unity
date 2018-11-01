using System.Collections.Generic;
using UnityEngine;

public class BulletPhysics : MonoBehaviour
{
    public CircleCollider2D PlayerBox;
    public BulletManager BulletManager;

    // Cache the player transform
    private Transform _playerTransform;

    private void Start()
    {
        _playerTransform = PlayerBox.transform;
    }

    void Update()
    {
        CheckPlayerCollision();
    }

    private void CheckPlayerCollision()
    {
        List<Bullet> BulletToRemove = new List<Bullet>();

        for (int i = 0; i < BulletManager.BulletsCount(); i++)
        {
            var currentBullet = BulletManager.Bullets[i];
            var bulletPosition = new Vector3(currentBullet.Position.x, currentBullet.Position.y, 0f) / BulletManager.PixelPerUnit();

            var dx = (currentBullet.Position.x / BulletManager.PixelPerUnit()) - _playerTransform.position.x;
            var dy = (currentBullet.Position.y / BulletManager.PixelPerUnit()) - _playerTransform.position.y;
            var radius = currentBullet.Profile().CollisionRadius + PlayerBox.radius;

            if ((dx * dx) + (dy * dy) < radius * radius)
            {
                BulletToRemove.Add(currentBullet);
            }
        }

        foreach(var bullet in BulletToRemove)
        {
            BulletManager.Bullets.Remove(bullet);
        }
    }
}
