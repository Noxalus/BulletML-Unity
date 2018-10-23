using System.Collections.Generic;
using UnityEngine;
using BulletML;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class BulletManager : MonoBehaviour, IBulletManager
{
    public float Difficulty;
    public GameObject Player;
    
    public List<GameObject> BulletPrefabs;
    public int MaximumBullet;
    public GameObject BulletHolder;

    private static readonly List<Bullet> _bullets = new List<Bullet>();

    private Queue<GameObject> _bulletsPools;

    // Particles
    private const bool ENABLE_PARTICLES = false;
    public ParticleSystem ParticleSystem;
    private List<ParticleCollisionEvent> _collisionEvents;
    private ParticleSystem.Particle[] _bulletParticles;
    // these lists are used to contain the particles which match
    // the trigger conditions each frame.
    List<ParticleSystem.Particle> enterTriggerParticles = new List<ParticleSystem.Particle>();

    // ECS
    private static SpriteInstanceRenderer[] _renderers;
    private static EntityManager _entityManager;
    private static SpriteInstanceRenderer _bulletRenderer;
    private static EntityArchetype _bulletArchetype;

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

    public static List<Bullet> Bullets
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

    public IBullet CreateBullet(bool topBullet = false)
    {
        var bullet = new Bullet(this);
        bullet.Init();

        //if (_bullets.Count < MaximumBullet)
        {
            _bullets.Add(bullet);
        }

        if (ENABLE_PARTICLES)
            EmitBulletParticle(bullet);
        else
            SpawnBullet(1);

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

        if (ENABLE_PARTICLES)
            UpdateParticles();

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

    #region ECS

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        _entityManager = World.Active.GetOrCreateManager<EntityManager>();

        _bulletArchetype = _entityManager.CreateArchetype(
            typeof(Position),
            typeof(Rotation),
            typeof(TransformMatrix)
        );
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeWithScene()
    {
        //Load all the sprites we need
        var bulletSprites = new[]
        {
            Resources.Load<Texture2D>("Sprites/elephant"),
            Resources.Load<Texture2D>("Sprites/giraffe"),
            Resources.Load<Texture2D>("Sprites/zebra")
        };

        //Assign loaded sprites to sprite renderers
        _renderers = new[]
        {
            new SpriteInstanceRenderer(bulletSprites[0], bulletSprites[0].width, new float2(0.5f, 0.5f)),
            new SpriteInstanceRenderer(bulletSprites[1], bulletSprites[1].width, new float2(0.5f, 0.5f)),
            new SpriteInstanceRenderer(bulletSprites[2], bulletSprites[2].width, new float2(0.5f, 0.5f)),
        };

        ////Assign loaded sprites to sprite renderers
        //var renderers = new SpriteInstanceRenderer[BulletPrefabs.Count];

        //for (var i = 0; i < BulletPrefabs.Count; i++)
        //{
        //    var spriteRenderer = BulletPrefabs[i].GetComponent<SpriteRenderer>();
        //    var spriteInstanceRenderer = new SpriteInstanceRenderer(spriteRenderer.sprite.texture, (int)spriteRenderer.sprite.pixelsPerUnit, new float2(0.5f, 0.5f));

        //    renderers[i] = spriteInstanceRenderer;
        //}

        _bulletRenderer = _renderers[0];

        //for (int i = 0; i < 5000; i++)
        //{
        //    SpawnBullet(i);
        //}
    }

    private static void SpawnBullet(int index)
    {
        var entity = _entityManager.CreateEntity(
            ComponentType.Create<Position>(),
            ComponentType.Create<Rotation>(),
            ComponentType.Create<TransformMatrix>()
        );

        _entityManager.SetComponentData(entity, new Position
        {
            Value = new float3(0, 0, 0)
        });

        //_entityManager.SetComponentData(entity, new Rotation
        //{
        //    Value = new quaternion(0f, 0f, 0f, 0f)
        //});

        _entityManager.AddSharedComponentData(entity, _renderers[index % 3]);

        //Entity bulletEntity = _entityManager.CreateEntity(_bulletArchetype);

        //float2 direction = new float2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        //_entityManager.SetComponentData(bulletEntity, new Position2D { Value = new float2(0, 0) });
        //_entityManager.SetComponentData(bulletEntity, new Heading2D { Value = direction });
        //_entityManager.SetComponentData(bulletEntity, new MoveSpeed { speed = 1 });

        //_entityManager.AddSharedComponentData(bulletEntity, _bulletRenderer);
    }

    #endregion

    #region Particle

    private void EmitBulletParticle(Bullet bullet)
    {
        var bulletColor = new UnityEngine.Color(
            bullet.Color.R / 255f,
            bullet.Color.G / 255f,
            bullet.Color.B / 255f,
            bullet.Color.A / 255f
        );

        bulletColor = UnityEngine.Color.blue;

        var emitParams = new ParticleSystem.EmitParams
        {
            position = bullet.Position,
            startSize = 1f,
            startLifetime = 9999f,
            startColor = UnityEngine.Color.yellow
        };

        ParticleSystem.Emit(emitParams, 1);
    }

    private void UpdateParticles()
    {
        _bulletParticles = new ParticleSystem.Particle[_bullets.Count];
        ParticleSystem.GetParticles(_bulletParticles);

        for (int i = 0; i < _bulletParticles.Length; i++)
        {
            _bulletParticles[i].position = _bullets[i].Position / 100f;
            _bulletParticles[i].rotation = _bullets[i].Direction * Mathf.Rad2Deg + 180f;
            _bulletParticles[i].startColor = new UnityEngine.Color(_bullets[i].Color.R / 255f, _bullets[i].Color.G / 255f, _bullets[i].Color.B / 255f, _bullets[i].Color.A / 255f);
            _bulletParticles[i].startSize = 1f * _bullets[i].Scale;
            _bulletParticles[i].randomSeed = GetRandomFromSpriteIndex(_bullets[i].SpriteIndex);
            // 8 -> 7 | 7 -> 4 | 6 -> 5 | 5 -> 0 | 4 -> 5 | 3 -> 7 | 2 -> 2 | 1 -> 3 | 0 -> 1 | 9 -> 3
            // 9 -> 3 | 8 -> 6 | 7 -> 2 | 6 -> 5 | 5 -> 0 | 4 -> 4 | 3 -> 7 | 2 -> 2 | 1 -> 6 | 0 -> 1
            // 0 -> 5 | 1 -> 0 | 2 -> 2 (7) | 3 -> 9 | 4 -> 4 | 5 -> 6 | 6 -> 1 (8) | 7 -> 3

            //_bulletParticles[i].randomValue = 0.5f;
        }

        ParticleSystem.SetParticles(_bulletParticles, _bulletParticles.Length);
    }

    void OnParticleTrigger()
    {
        int numEnter = ParticleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterTriggerParticles);

        if (numEnter > 0)
        {
            for (int i = 0; i < numEnter; i++)
            {
                ParticleSystem.Particle p = enterTriggerParticles[i];
                p.startColor = new Color32(0, 255, 255, 255);
                p.remainingLifetime = 0f;

                var correspondingBullets = _bullets.FindAll(b => Mathf.Abs((b.Position.x / 100f) - p.position.x) < 0.1f && Mathf.Abs((b.Position.y / 100f) - p.position.y) < 0.1f);

                foreach (var bullet in correspondingBullets)
                    bullet.Used = false;

                enterTriggerParticles[i] = p;
            }

            ParticleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterTriggerParticles);
        }
    }

    private uint GetRandomFromSpriteIndex(int spriteIndex)
    {
        if (spriteIndex == 0)
            return 5u;
        if (spriteIndex == 1)
            return 0u;
        if (spriteIndex == 2)
            return 2u;
        if (spriteIndex == 3)
            return 9u;
        if (spriteIndex == 4)
            return 4u;
        if (spriteIndex == 5)
            return 6u;
        if (spriteIndex == 6)
            return 1u;
        if (spriteIndex == 7)
            return 3u;

        return 5u;
    }

    private float GetRadiusFromSpriteIndex(int spriteIndex)
    {
        if (spriteIndex == 0)
            return 0.15f;
        if (spriteIndex == 1)
            return 0.1f;
        if (spriteIndex == 2)
            return 0.2f;
        if (spriteIndex == 3)
            return 0.15f;
        if (spriteIndex == 4)
            return 0.3f;
        if (spriteIndex == 5)
            return 0.45f;
        if (spriteIndex == 6)
            return 0.7f;
        if (spriteIndex == 7)
            return 0f;

        return 1f;
    }

    #endregion
}
