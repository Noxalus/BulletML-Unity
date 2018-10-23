using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class EntityMovementSystem : JobComponentSystem
{
    [Inject] private Data data;

    struct PositionJob : IJobParallelFor
    {
        public ComponentDataArray<Position> positions;
        public float dt;

        public void Execute(int index)
        {
            var position = positions[index];
            //float wobbleX = Mathf.PerlinNoise(position.Value.x, position.Value.y) - 0.5f;
            //float wobbleY = Mathf.PerlinNoise(position.Value.y, position.Value.x) - 0.5f;

            if (BulletManager.Bullets != null && index < BulletManager.Bullets.Count)
            {
                var bulletPosition = BulletManager.Bullets[index].Position;
                position.Value = new float3(bulletPosition.x / 100f, bulletPosition.y / 100f, 0f);
            }

            //if (BulletManager.Bullets != null && BulletManager.Bullets.Count < index)
            {
                //var bulletPosition = BulletManager.Bullets[index].Position;
                //position.Value = new float3(bulletPosition.x, bulletPosition.y, 0f);
                //positions[index] = new Position(new float3(bulletPosition.x, bulletPosition.x, 0f));
            }

            //positions[index] = new Position(new float3(wobbleX * 30f, wobbleY * 30f, 0f));
            //positions[index] = new Position(new float3(position.Value.x + 5f * dt, position.Value.x + 5f * dt, 0f));

            //position.Value = new float3(position.Value.x + 50f, position.Value.x + 50f, 0f);

            //position.Value += dt * new float3(wobbleX, wobbleY, 0f);

            positions[index] = position;
        }
    }

    //[Unity.Burst.BurstCompile]
    //struct RotationJob : IJobProcessComponentData<Rotation>
    //{
    //    public float dt;

    //    public void Execute(ref Rotation rotation)
    //    {
    //        rotation.Value.value.x += 10f * dt;
    //    }
    //}

    //protected override void OnUpdate()
    //{
    //    var dt = Time.deltaTime;

    //    for (var index = 0; index < data.Length; ++index)
    //    {
    //        var position = data.Position[index].Value;

    //        float wobbleX = Mathf.PerlinNoise(position.x, position.y) - 0.5f;
    //        float wobbleY = Mathf.PerlinNoise(position.y, position.x) - 0.5f;
    //        position += dt * new float3(wobbleX, wobbleY, 0);

    //        data.Position[index] = new Position { Value = position };
    //    }
    //}

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var positionJob = new PositionJob()
        {
            positions = data.Position,
            dt = Time.deltaTime
        };

        return positionJob.Schedule(data.Length, 64, inputDeps);

        //var rotationJob = new RotationJob() { dt = Time.deltaTime };
        //return rotationJob.Schedule(this, 64, inputDeps);
    }

    //struct AddOneJob
    //{
    //    public NativeArray<int> SomeInts;
    //    public int value;

    //    public void Execute(int index)
    //    {
    //        // for this job, index is the index in the original array [0,5)
    //        SomeInts[index] = SomeInts[index] + value;
    //    }
    //}

    //public NativeArray<int> m_SomeInts = new NativeArray<int>();

    //protected override JobHandle OnUpdate(JobHandle inputDeps)
    //{
    //    var len = m_SomeInts.Length;
    //    if (len == 0) return inputDeps;

    //    var addOneJobHandle = new AddOneJob
    //    {
    //        SomeInts = m_SomeInts,
    //        Value = 5,
    //    }.Schedule(len, 32, inputDeps); // will process each add in batches of 32 or remaining len elements, can tune to find optimum batch size, if have multiple batches, can be split across multiple cores by scheduler. Cores Will work-steal from unprocessed batches if the opportunity opens up

    //    return addOneJobHandle;
    //}

    public struct Data
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<Rotation> Rotation;
    }
}