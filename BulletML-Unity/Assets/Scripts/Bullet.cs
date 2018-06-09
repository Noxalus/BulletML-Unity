using UnityEngine;

public class Bullet : BulletML.Bullet
{
    public Texture2D Texture;
    public Vector2 Position;
    public GameObject Parent;

    private GameObject _gameObject;
    private SpriteRenderer _spriteRenderer;
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

    public Bullet(BulletML.IBulletManager bulletManager, GameObject parent) : base(bulletManager)
    {
        Parent = parent;
        _bulletManager = bulletManager as BulletManager;
    }

    public void Init()
    {
        Used = true;
        ChangePrefab();
    }

    private void ChangePrefab()
    {
        _currentSpriteIndex = SpriteIndex;
        _bulletManager.DestroyGameObject(_gameObject);
        _gameObject = _bulletManager.InstantiateBulletPrefabs(SpriteIndex);
        //_gameObject = _bulletManager.InstantiateBulletFromPool(SpriteIndex);

        if (_gameObject)
            _spriteRenderer = _gameObject.GetComponent<SpriteRenderer>();
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if (_gameObject)
        {
            _gameObject.transform.position = Position / _spriteRenderer.sprite.pixelsPerUnit;
            //_gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
            //_gameObject.transform.Rotate(0, 0, Direction * Mathf.Rad2Deg);

            //_gameObject.transform.localScale = new Vector3(Scale, Scale, Scale);
            //_spriteRenderer.color = new Color(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f);

            //if (_currentSpriteIndex != SpriteIndex)
            //    ChangePrefab();
        }
    }
}
