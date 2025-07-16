using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
partial struct NetcodePlayerInputSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkStreamInGame>();
        state.RequireForUpdate<NetcodePlayerInputData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (
            RefRW<NetcodePlayerInputData> netcodePlayerInputData
            in SystemAPI.Query<RefRW<NetcodePlayerInputData>>().WithAll<GhostOwnerIsLocal>())
        {
            float2 inputVector = new float2();
            if (Input.GetKey(KeyCode.W))
            {
                inputVector.y += 1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                inputVector.y -= 1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inputVector.x -= 1f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                inputVector.x += 1f;
            }
            netcodePlayerInputData.ValueRW.inputVector = inputVector;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
            {
                Debug.Log("Shooting!");
                netcodePlayerInputData.ValueRW.shoot.Set();
            }
            else
            {
                netcodePlayerInputData.ValueRW.shoot = default;
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
