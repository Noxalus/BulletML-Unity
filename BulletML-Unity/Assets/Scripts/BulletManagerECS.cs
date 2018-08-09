using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

//public class BulletManagerECS : MonoBehaviour {
//    public float Difficulty;
//    public GameObject Player;
//    public List<GameObject> BulletPrefabs;
//    public int MaximumBullet;

//    private readonly List<Bullet> _bullets = new List<Bullet>();
//    private readonly List<Bullet> _topLevelBullets = new List<Bullet>();
//}

//public class Position2D : Component
//{
//    public float2 Position;
//}

//public class Heading2D : Component
//{
//    public float2 Heading;
//}

//class BulletMoveSystem : ComponentSystem
//{
//    struct BulletManagerGroup
//    {
//        public readonly List<Bullet> Bullets;
//    }

//    [Inject] BulletManagerGroup data;

//    protected override void OnUpdate()
//    {
//        for (int i = 0; i < data.Bullets.Count; i++)
//        {
//        }
//    }
//}