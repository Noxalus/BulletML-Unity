using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine;

//Just gives the animals a little movement as an example
public class EntityMovementSystem : JobComponentSystem
{
    [Inject] private Data data;

    [Unity.Burst.BurstCompile]
    struct PositionJob : IJobProcessComponentData<Position>
    {
        public float dt;

        public void Execute(ref Position position)
        {
            float wobbleX = Mathf.PerlinNoise(position.Value.x, position.Value.y) - 0.5f;
            float wobbleY = Mathf.PerlinNoise(position.Value.y, position.Value.x) - 0.5f;

            //position.Value.x += dt * 0.01f;

            position.Value += dt * new float3(wobbleX, wobbleY, 0f);
        }
    }

    [Unity.Burst.BurstCompile]
    struct RotationJob : IJobProcessComponentData<Rotation>
    {
        public float dt;

        public void Execute(ref Rotation rotation)
        {
            //rotation.Value.value.x += 10f * dt;
        }
    }

    // protected override void OnUpdate()
    // {
    // 	var dt = Time.deltaTime;

    // 	for (var index = 0; index < data.Length; ++index)
    // 	{
    // 		var position = data.Position[index].Value;
    // 		var heading = data.Heading[index].Value;

    // 		float wobbleX = Mathf.PerlinNoise(position.x, position.y) - 0.5f;
    // 		float wobbleY = Mathf.PerlinNoise(position.y, position.x) - 0.5f;
    // 		position += dt * new float2(wobbleX, wobbleY);

    // 		data.Position[index] = new Position2D {Value = position};
    // 		data.Heading[index] = new Heading2D {Value = heading};
    // 	}
    // }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var positionJob = new PositionJob() { dt = Time.deltaTime };
        return positionJob.Schedule(this, 64, inputDeps);

        //var rotationJob = new RotationJob() { dt = Time.deltaTime };
        //return rotationJob.Schedule(this, 64, inputDeps);
    }

    public struct Data
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<Rotation> Rotation;
    }
}