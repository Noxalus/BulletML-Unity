using BulletML;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class BulletManager : MonoBehaviour, IBulletManager
{
    public const int MAX_BATCH_AMOUNT = 1023;
    public const int MAX_BULLET_AMOUNT = 4096;

    public float Difficulty;
    public GameObject Player;
    public Sprite BulletsTexture;
    public BulletProfile[] BulletProfiles;

    private List<Bullet> _bullets = new List<Bullet>();
    // Store transform data of all bullets in arrays for optimization
    private List<Matrix4x4[]> _bulletMatricesBatches = new List<Matrix4x4[]>();
    public Vector4[] BulletSpriteOffsets = new Vector4[MAX_BULLET_AMOUNT];

    private float GetDifficulty()
    {
        return Difficulty;
    }

    public BulletML.Vector2 PlayerPosition(IBullet targettedBullet)
    {
        if (!Player)
            return BulletML.Vector2.Zero;

        return new BulletML.Vector2(Player.transform.position.x, Player.transform.position.y);
    }

    public List<Bullet> Bullets
    {
        get { return _bullets; }
    }

    public List<Matrix4x4[]> BulletMatrices
    {
        get { return _bulletMatricesBatches; }
    }

    public int BulletsCount()
    {
        return _bullets.Count;
    }

    void Start()
    {
        GameManager.GameDifficulty = GetDifficulty;
    }

    public IBullet CreateBullet(bool topBullet = false)
    {
        var bullet = new Bullet(this);
        bullet.Init();

        if (_bullets.Count < MAX_BULLET_AMOUNT)
        {
            _bullets.Add(bullet);
        }

        return bullet;
    }

    public void RemoveBullet(IBullet deadBullet)
    {
        var bullet = deadBullet as Bullet;

        if (bullet != null)
            bullet.Used = false;
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < _bullets.Count; i++)
        {
            _bullets[i].Update(Time.fixedDeltaTime);

            int batchIndex = i / MAX_BATCH_AMOUNT;
            int matrixIndex = i % MAX_BATCH_AMOUNT;

            if (_bulletMatricesBatches.Count <= batchIndex)
            {
                _bulletMatricesBatches.Add(new Matrix4x4[MAX_BATCH_AMOUNT]);
            }

            _bulletMatricesBatches[batchIndex][matrixIndex] = _bullets[i].RenderData;
            //var spriteOffset = BulletProfiles[_bullets[i].SpriteIndex].SpriteOffset;
            //BulletSpriteOffsets[i] = new Vector4(0.25f, 0.25f, spriteOffset.x, spriteOffset.y);
        }

        ClearDeadBullets();
    }

    private void ClearDeadBullets()
    {
        for (int i = 0; i < _bullets.Count; i++)
        {
            if (!_bullets[i].Used)
            {
                _bullets.Remove(_bullets[i]);
                i--;
            }
        }
    }

    public void Clear()
    {
        _bullets.Clear();
    }

    public void DestroyGameObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}
