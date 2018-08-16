using System.Collections.Generic;
using UnityEngine;
using BulletML;
using System.Linq;

public class BulletManager : MonoBehaviour, IBulletManager
{
    public float Difficulty;
    public GameObject Player;
    public List<GameObject> BulletPrefabs;
    public int MaximumBullet;
    public GameObject BulletHolder;

    private readonly List<Bullet> _bullets = new List<Bullet>();

    private Queue<GameObject> _bulletsPools;

    // Particles
    public ParticleSystem ParticleSystem;
    private List<ParticleCollisionEvent> _collisionEvents;
    private ParticleSystem.Particle[] _bulletParticles;
    // these lists are used to contain the particles which match
    // the trigger conditions each frame.
    List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();
    List<ParticleSystem.Particle> outside = new List<ParticleSystem.Particle>();
    List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
    List<ParticleSystem.Particle> exit = new List<ParticleSystem.Particle>();

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

    public int BulletsCount()
    {
        return _bullets.Count;
    }

    void Start()
    {
        GameManager.GameDifficulty = GetDifficulty;

        _bulletsPools = new Queue<GameObject>();
        _collisionEvents = new List<ParticleCollisionEvent>();

        var circleCollider2d = BulletPrefabs[0].GetComponent<CircleCollider2D>();

        for (int i = 0; i < MaximumBullet; i++)
        {
            var newBullet = Instantiate(BulletPrefabs[0], BulletHolder != null ? BulletHolder.transform : null);
            newBullet.SetActive(false);
            _bulletsPools.Enqueue(newBullet);
        }
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

    /// <summary>
    /// Get a normalized direction from an angle in degrees
    /// </summary>
    /// <param name="angle">Angle in degrees</param>
    /// <returns></returns>
    public static UnityEngine.Vector2 AngleToDirection(float angle)
    {
        // Convert the angle in radians
        angle = (angle + 90f) * Mathf.Deg2Rad;

        var direction = new UnityEngine.Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        direction.Normalize(); // make sure the direction is normalized

        return direction;
    }

    public IBullet CreateBullet(bool topBullet = false)
    {
        var bullet = new Bullet(this);
        bullet.Init();

        //if (_bullets.Count < MaximumBullet)
        {
            var bulletColor = new UnityEngine.Color(
                bullet.Color.R / 255f,
                bullet.Color.G / 255f,
                bullet.Color.B / 255f,
                bullet.Color.A / 255f
            );

            _bullets.Add(bullet);
            bulletColor = UnityEngine.Color.blue;

            var emitParams = new ParticleSystem.EmitParams
            {
                position = bullet.Position,
                rotation = bullet.Direction,
                startSize = 0.25f,
                startLifetime = 9999f,
                startColor = UnityEngine.Color.yellow
            };

            ParticleSystem.Emit(emitParams, 1);
        }

        return bullet;
    }

    public void RemoveBullet(IBullet deadBullet)
    {
        var bullet = deadBullet as Bullet;

        if (bullet != null)
            bullet.Used = false;
    }

    void OnParticleTrigger()
    {
        // get the particles which matched the trigger conditions this frame
        int numInside = ParticleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, inside);
        int numOutside = ParticleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Outside, outside);
        int numEnter = ParticleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        int numExit = ParticleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Exit, exit);

        // iterate through the particles which entered the trigger and make them red
        if (numEnter > 0)
        {
            for (int i = 0; i < numEnter; i++)
            {
                ParticleSystem.Particle p = enter[i];
                p.startColor = new Color32(0, 255, 255, 255);
                //p.remainingLifetime = 0f;

                var correspondingBullets = _bullets.FindAll(b => Mathf.Abs((b.Position.x / 100f) - p.position.x) < 0.1f && Mathf.Abs((b.Position.y / 100f) - p.position.y) < 0.1f);

                foreach (var bullet in correspondingBullets)
                    bullet.Used = false;

                enter[i] = p;
            }

            ParticleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        }

        if (numExit > 0)
        {
            // iterate through the particles which exited the trigger and make them green
            for (int i = 0; i < numExit; i++)
            {
                ParticleSystem.Particle p = exit[i];
                p.startColor = new Color32(0, 255, 0, 255);
                exit[i] = p;
            }

            ParticleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Exit, exit);
        }
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("On Particle Collision");

        int numCollisionEvents = ParticleSystem.GetCollisionEvents(other, _collisionEvents);

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        int i = 0;

        while (i < numCollisionEvents)
        {
            if (rb)
            {
                Vector3 pos = _collisionEvents[i].intersection;
                Vector3 force = _collisionEvents[i].velocity * 10;
                force = new Vector3(Random.Range(0f, 10f), Random.Range(0f, 10f), 0);
                rb.AddForce(force);
            }
            i++;
        }
    }

    public void Update()
    {
        for (int i = 0; i < _bullets.Count; i++)
        {
            _bullets[i].Update(Time.deltaTime);
        }

        _bulletParticles = new ParticleSystem.Particle[_bullets.Count];
        ParticleSystem.GetParticles(_bulletParticles);

        for (int i = 0; i < _bulletParticles.Length; i++)
        {
            _bulletParticles[i].position = _bullets[i].Position / 100f;
            _bulletParticles[i].rotation = _bullets[i].Direction * Mathf.Rad2Deg + 90f;
            //_bulletParticles[i].startColor = new UnityEngine.Color(_bullets[i].Color.R / 255f, _bullets[i].Color.G / 255f, _bullets[i].Color.B / 255f, _bullets[i].Color.A / 255f);
            //_bulletParticles[i].startSize = 0.25f;
        }

        ParticleSystem.SetParticles(_bulletParticles, _bulletParticles.Length);

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
