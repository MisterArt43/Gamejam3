using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
partial struct TestNetcodeEntitiesClientSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        bool hasInput = false;
        foreach (var input in SystemAPI.Query<RefRO<NetcodePlayerInputData>>())
        {
            if (input.ValueRO.inputVector.x != 0 || input.ValueRO.inputVector.y != 0)
            {
                hasInput = true;
                break;
            }
        }

        if (hasInput)
        {
            Entity rpcEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(rpcEntity, new SimpleRpc
            {
                value = 56
            });
            state.EntityManager.AddComponentData(rpcEntity, new SendRpcCommandRequest());
            // Debug.Log("RPC sent...");
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
