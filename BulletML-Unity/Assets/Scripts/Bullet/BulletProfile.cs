using UnityEngine;

[CreateAssetMenu(fileName = "BulletProfile", menuName = "Bullet/BulletProfile")]
public class BulletProfile : ScriptableObject
{
    [SerializeField] private float _collisionRadius = 0.5f;
    [SerializeField] private Vector2 _pivot = new Vector2(0.5f, 0.5f);

    public float CollisionRadius => _collisionRadius;
    public Vector2 Pivot => _pivot;
}
