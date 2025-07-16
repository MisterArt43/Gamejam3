using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
partial struct ShootSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NetworkTime networkTime = SystemAPI.GetSingleton<NetworkTime>();
        EntitiesReferences entitiesReference = SystemAPI.GetSingleton<EntitiesReferences>();

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach ((RefRO<NetcodePlayerInputData> netcodePlayerInputData, RefRO<LocalTransform> localTransform)
        in SystemAPI.Query<RefRO<NetcodePlayerInputData>, RefRO<LocalTransform>>()
            .WithAll<Simulate>())
        {

            if (networkTime.IsFirstTimeFullyPredictingTick)
            {
                if (netcodePlayerInputData.ValueRO.shoot.IsSet)
                {
                    Debug.Log("Shooting action detected!" + state.World);

                    Entity bulletEntity = entityCommandBuffer.Instantiate(entitiesReference.bulletPrefabEntity);
                    // entityCommandBuffer.SetComponent(bulletEntity, LocalTransform.FromPosition());
                }
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
