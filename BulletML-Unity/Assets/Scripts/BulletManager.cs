using BulletML;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour, IBulletManager
{
    public float Difficulty;
    public GameObject Player;

    public List<GameObject> BulletPrefabs;
    public int MaximumBullet;
    public GameObject BulletHolder;

    private List<Bullet> _bullets = new List<Bullet>();

    private Queue<GameObject> _bulletsPools;

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

    public int BulletsCount()
    {
        return _bullets.Count;
    }

    void Start()
    {
        GameManager.GameDifficulty = GetDifficulty;

        _bulletsPools = new Queue<GameObject>();

        //var circleCollider2d = BulletPrefabs[0].GetComponent<CircleCollider2D>();

        //for (int i = 0; i < MaximumBullet; i++)
        //{
        //    var newBullet = Instantiate(BulletPrefabs[0], BulletHolder != null ? BulletHolder.transform : null);
        //    newBullet.SetActive(false);
        //    _bulletsPools.Enqueue(newBullet);
        //}
    }

    public GameObject InstantiateBulletFromPool()
    {
        if (_bulletsPools.Count == 0)
            return null;

        var bullet = _bulletsPools.Dequeue();

        bullet.SetActive(true);
        _bulletsPools.Enqueue(bullet);

        return bullet;
    }

    public GameObject InstantiateBulletPrefabs(int prefabIndex)
    {
        if (prefabIndex >= BulletPrefabs.Count)
        {
            Debug.Log("No prefab for index: " + prefabIndex);
            return null;
        }

        return Instantiate(BulletPrefabs[prefabIndex]);
    }

    public IBullet CreateBullet(bool topBullet = false)
    {
        var bullet = new Bullet(this);
        bullet.Init();

        //if (_bullets.Count < MaximumBullet)
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

    public void Update()
    {
        for (int i = 0; i < _bullets.Count; i++)
            _bullets[i].Update(Time.deltaTime);

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

    public GameObject GetBulletPrefab(int spriteIndex)
    {
        if (BulletPrefabs.Count <= spriteIndex)
            return null;

        return BulletPrefabs[spriteIndex];
    }
}
