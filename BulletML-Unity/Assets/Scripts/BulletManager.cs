using System.Collections.Generic;
using UnityEngine;
using BulletML;

public class BulletManager : MonoBehaviour, IBulletManager
{
    public float Difficulty;
    public GameObject Player;
    public List<GameObject> BulletPrefabs;

    private readonly List<Bullet> _bullets = new List<Bullet>();
    private readonly List<Bullet> _topLevelBullets = new List<Bullet>();

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

    void Start()
    {
        GameManager.GameDifficulty = GetDifficulty;
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
        var bullet = new Bullet(this, gameObject);
        bullet.Init();

        if (topBullet)
            _topLevelBullets.Add(bullet);
        else
            _bullets.Add(bullet);

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

        for (int i = 0; i < _topLevelBullets.Count; i++)
            _topLevelBullets[i].Update(Time.deltaTime);

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

        for (int i = 0; i < _topLevelBullets.Count; i++)
        {
            if (_topLevelBullets[i].TasksFinished())
            {
                _topLevelBullets.RemoveAt(i);
                i--;
            }
        }
    }

    public void Clear()
    {
        _bullets.Clear();
        _topLevelBullets.Clear();
    }

    public void DestroyGameObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}
