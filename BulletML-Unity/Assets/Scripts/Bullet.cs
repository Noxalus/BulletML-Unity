using UnityEngine;

public class Bullet : BulletML.Bullet
{
    public Texture2D Texture;
    public Vector2 Position;
    public GameObject Parent;

    private GameObject _gameObject;

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

    public bool Used { get; set; }

    private BulletManager _bulletManager;

    public Bullet(BulletML.IBulletManager bulletManager, GameObject parent) : base(bulletManager)
    {
        Parent = parent;
        _bulletManager = bulletManager as BulletManager;
    }

    public void Init()
    {
        Used = true;
        _gameObject = _bulletManager.InstantiateBulletPrefabs(SpriteIndex);
    }

    public override void Update()
    {
        base.Update();

        // TODO: Handle sprite change (change the prefab instance)

        if (_gameObject != null)
        {
            _gameObject.transform.position = Position;
            _gameObject.transform.rotation = Quaternion.identity;
            _gameObject.transform.Rotate(0, 0, (Direction * Mathf.Rad2Deg) + 180);
        }
    }
}
