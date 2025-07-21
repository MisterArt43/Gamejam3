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
        state.RequireForUpdate<NetworkStreamInGame>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NetworkTime networkTime = SystemAPI.GetSingleton<NetworkTime>();
        EntitiesReferences entitiesReference = SystemAPI.GetSingleton<EntitiesReferences>();

        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        foreach ((RefRO<NetcodePlayerInputData> netcodePlayerInputData, RefRO<LocalTransform> localTransform, RefRO<GhostOwner> ghostOwner)
        in SystemAPI.Query<RefRO<NetcodePlayerInputData>, RefRO<LocalTransform>, RefRO<GhostOwner>>().WithAll<Simulate>())
        {
            if (networkTime.IsFirstTimeFullyPredictingTick)
            {
                if (netcodePlayerInputData.ValueRO.shoot.IsSet)
                {
                    // Debug.Log("Shooting action detected!" + state.World);

                    Entity bulletEntity = entityCommandBuffer.Instantiate(entitiesReference.bulletPrefabEntity);
                    entityCommandBuffer.SetComponent(bulletEntity, LocalTransform.FromPositionRotation(localTransform.ValueRO.Position, localTransform.ValueRO.Rotation));
                    entityCommandBuffer.SetComponent(bulletEntity, new GhostOwner { NetworkId = ghostOwner.ValueRO.NetworkId });
                }
            }
        }
        entityCommandBuffer.Playback(state.EntityManager);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
