using BulletML;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class BulletManager : MonoBehaviour, IBulletManager
{
    public const int MAX_BATCH_AMOUNT = 1023;
    public const int MAX_BULLET_AMOUNT = 10000;

    public float Difficulty;
    public GameObject Player;
    public Sprite BulletsTexture;
    public BulletProfile[] BulletProfiles;

    private List<Bullet> _bullets = new List<Bullet>(MAX_BULLET_AMOUNT);
    // Store transform data of all bullets in arrays for optimization
    private List<Matrix4x4[]> _bulletMatricesBatches = new List<Matrix4x4[]>();
    public List<Vector4[]> _bulletSpriteOffsetsBatches = new List<Vector4[]>();
    public List<Vector4[]> _bulletColorsBatches = new List<Vector4[]>();


    private float GetDifficulty()
    {
        return Difficulty;
    }

    public float PixelPerUnit()
    {
        return BulletsTexture.pixelsPerUnit;
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

    public List<Matrix4x4[]> BulletTransformMatrices => _bulletMatricesBatches;
    public List<Vector4[]> BulletSpriteOffsetsBatches => _bulletSpriteOffsetsBatches;
    public List<Vector4[]> BulletColorsBatches => _bulletColorsBatches;

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
            Bullet currentBullet = _bullets[i];

            currentBullet.Update(Time.fixedDeltaTime);

            int batchIndex = i / MAX_BATCH_AMOUNT;
            int elementIndex = i % MAX_BATCH_AMOUNT;

            if (_bulletMatricesBatches.Count <= batchIndex)
            {
                _bulletMatricesBatches.Add(new Matrix4x4[MAX_BATCH_AMOUNT]);
                _bulletSpriteOffsetsBatches.Add(new Vector4[MAX_BATCH_AMOUNT]);
                _bulletColorsBatches.Add(new Vector4[MAX_BATCH_AMOUNT]);
            }

            _bulletMatricesBatches[batchIndex][elementIndex] = currentBullet.TransformMatrix;
            var textureOffset = GetTextureOffset(currentBullet.SpriteIndex);
            _bulletSpriteOffsetsBatches[batchIndex][elementIndex] = textureOffset;
            _bulletColorsBatches[batchIndex][elementIndex] = new Vector4(
                currentBullet.Color.R / 255f,
                currentBullet.Color.G / 255f,
                currentBullet.Color.B / 255f,
                currentBullet.Color.A / 255f
            );
        }

        ClearDeadBullets();
    }

    private Vector4 GetTextureOffset(int spriteIndex)
    {
        int elementPerLine = 4;
        float tiling = 1f / elementPerLine;

        Vector4 textureOffset = new Vector4(tiling, tiling, 0, 0);
        int textureColumnIndex = spriteIndex % elementPerLine;
        int textureLineIndex = spriteIndex / elementPerLine;

        textureOffset.z = textureColumnIndex * tiling;
        textureOffset.w = (1f - tiling) - (textureLineIndex * tiling);

        return textureOffset;
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
