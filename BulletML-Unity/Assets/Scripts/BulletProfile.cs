using UnityEngine;

[CreateAssetMenu(fileName = "BulletProfile", menuName = "Bullet/BulletProfile")]
public class BulletProfile : ScriptableObject
{
    [SerializeField] private Vector2 _spriteOffset;
    [SerializeField] private float _collisionRadius = 0.5f;
    [SerializeField] private Vector2 _pivot = new Vector2(0.5f, 0.5f);

    public Vector2 SpriteOffset => _spriteOffset;
    public float CollisionRadius => _collisionRadius;
    public Vector2 Pivot => _pivot;
}
