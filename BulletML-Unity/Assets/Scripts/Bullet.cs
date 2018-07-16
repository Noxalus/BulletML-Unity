using UnityEngine;

public class Bullet : BulletML.Bullet
{
    public Texture2D Texture;
    public Vector2 Position;

    private GameObject _gameObject;
    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _collider;
    private int _currentSpriteIndex;

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

    public Bullet(BulletML.IBulletManager bulletManager) : base(bulletManager)
    {
        _bulletManager = bulletManager as BulletManager;
    }

    public void Init()
    {
        Used = true;
        _currentSpriteIndex = SpriteIndex;

        //_bulletManager.DestroyGameObject(_gameObject);
        //_gameObject = _bulletManager.InstantiateBulletPrefabs(SpriteIndex);
        _gameObject = _bulletManager.InstantiateBulletFromPool();

        if (_gameObject)
        {
            _spriteRenderer = _gameObject.GetComponent<SpriteRenderer>();
            _collider = _gameObject.GetComponent<CircleCollider2D>();
        }

        UpdateBaseData();
    }

    private void UpdateBaseData()
    {
        var bulletPrefab = _bulletManager.GetBulletPrefab(_currentSpriteIndex);

        var circleCollider = bulletPrefab.GetComponent<CircleCollider2D>();
        var spriteRenderer = bulletPrefab.GetComponent<SpriteRenderer>();

        _spriteRenderer.sprite = spriteRenderer.sprite;
        _collider.radius = circleCollider.radius;
    }

    private void UpdateData()
    {
        if (_gameObject)
        {
            _gameObject.transform.position = Position / _spriteRenderer.sprite.pixelsPerUnit;
            _gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
            _gameObject.transform.Rotate(0, 0, Direction * Mathf.Rad2Deg);

            _gameObject.transform.localScale = new Vector3(Scale, Scale, Scale);
            _spriteRenderer.color = new Color(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f);

            if (_currentSpriteIndex != SpriteIndex)
            {
                _currentSpriteIndex = SpriteIndex;
                UpdateBaseData();
            }
        }
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        UpdateData();
    }
}
