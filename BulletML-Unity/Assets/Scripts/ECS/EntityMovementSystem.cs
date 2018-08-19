using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms2D;
using UnityEngine;

//Just gives the animals a little movement as an example
public class EntityMovementSystem : JobComponentSystem
{
    [Inject] private Data data;

    [Unity.Burst.BurstCompileAttribute]
    struct PositionJob : IJobProcessComponentData<Position2D>
    {
        public float dt;

        public void Execute(ref Position2D position)
        {
            float wobbleX = Mathf.PerlinNoise(position.Value.x, position.Value.y) - 0.5f;
            float wobbleY = Mathf.PerlinNoise(position.Value.y, position.Value.x) - 0.5f;
            position.Value += dt * new float2(wobbleX, wobbleY);
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
        var job = new PositionJob() { dt = Time.deltaTime };
        return job.Schedule(this, 64, inputDeps);
    }

    public struct Data
    {
        public readonly int Length;
        public ComponentDataArray<Position2D> Position;
        public ComponentDataArray<Heading2D> Heading;
    }
}