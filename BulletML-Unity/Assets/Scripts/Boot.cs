using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class Boot
{
    static int StartEntitiesCount = 10000; // how many entities we'll spawn on scene start
    public static EntityArchetype archetype1 { get; private set; }
    public static EntityArchetype archetype2 { get; private set; }
    public static MeshInstanceRenderer entityLook { get; private set; }

    // This attribute allows us to not use MonoBehaviours to instantiate entities
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializeBeforeScene()
    {
        var em = World.Active.GetOrCreateManager<EntityManager>(); // EntityManager manages all entities in world
        CreateArchetypes(em); // we need to set archetype first
    }

    // This attribute allows us to not use MonoBehaviours to instantiate entities
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeAfterScene()
    {
        var em = World.Active.GetOrCreateManager<EntityManager>(); // EntityManager manages all entities in world
        //entityLook = GameObject.Find("GameObjectWithMeshInstanceRenderer").GetComponent<MeshInstanceRendererComponent>().Value; // no other graphic api for now (06.2018)
        CreateEntities(em, StartEntitiesCount);
    }

    private static void CreateArchetypes(EntityManager em)
    {
        // ComponentType.Create<> is slightly more efficient than using typeof()
        em.CreateArchetype(typeof(Position), typeof(Heading), typeof(MoveSpeed));
        var pos = ComponentType.Create<Position>();
        var heading = ComponentType.Create<Heading>();

        archetype1 = em.CreateArchetype(pos, heading);
    }

    private static void CreateEntities(EntityManager em, int count)
    {
        // if you spawn more entities, it's more performant to do it with NativeArray
        // if you want to spawn just one entity, do:
        // var entity = em.CreateEntity(archetype1);
        NativeArray<Entity> entities = new NativeArray<Entity>(count, Allocator.Temp);
        em.CreateEntity(archetype1, entities); // Spawns entities and attach to them all components from archetype1

        // If we don't set components, their values will be default
        for (int i = 0; i < count; i++)
        {
            // Heading is build in Unity component and you need to set it
            // because default is float3(0, 0, 0), which is position
            // where you can't look towards, so you'll get error from TransformSystem.
            em.SetComponentData(entities[i], new Heading() { Value = new float3(0, 1, 0) });
            em.SetComponentData(entities[i], new Position() { Value = 1 });
        }
        entities.Dispose(); // all NativeArrays you need to dispose manually, it won't destroy our entities, just dispose not used anymore array
                            // that's it, entities exists in world and are ready to be injected into systems
    }
}