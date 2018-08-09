using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveSystem : ComponentSystem
{
    public struct Data
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
    }
    [Inject] private Data data;

    protected override void OnUpdate()
    {
        // iterate over all entities and send them to the sky
        //for (int i = 0; i < data.Length; i++)
        //{
        //    var pos = data.Position[i];
        //    pos.Value += new float3(0, 1, 0);
        //    data.Position[i] = pos;
        //}
    }
}
