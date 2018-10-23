using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BulletSystem : ComponentSystem
{
    struct Group
    {
        // Define what components are required for this 
        // ComponentSystem to handle them.
        public Position Position;
        public Transform TransformMatrix;
        //public Bullet Bullet;
    }

    override protected void OnUpdate()
    {
        // We can immediately see a first optimization.
        // We know delta time is the same between all rotators,
        // so we can simply keep it in a local variable 
        // to get better performance.
        float deltaTime = Time.deltaTime;

        // ComponentSystem.GetEntities<Group> 
        // lets us efficiently iterate over all GameObjects
        // that have both a Transform & Rotator component 
        // (as defined above in Group struct).
        //var entities = GetEntities<Group>();
        //for (int i = 0; i < entities.Length; i++)
        //{
        //    var currentEntity = entities[i];
        //    //var newPosition = currentEntity.Position.Value;
        //    //newPosition.x += 5f * deltaTime;
        //    //currentEntity.Position.Value = new float3(newPosition.x, newPosition.y, newPosition.z);
        //}
    }
}
