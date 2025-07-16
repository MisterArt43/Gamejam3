using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
partial struct NetcodePlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((
            RefRO<NetcodePlayerInputData> NetcodePlayerInputData,
            RefRW<LocalTransform> localTransform)
            in SystemAPI.Query<RefRO<NetcodePlayerInputData>, RefRW<LocalTransform>>().WithAll<Simulate>())
        {
            float moveSpeed = 5f; // Speed of the player movement
            float3 moveVector = new(NetcodePlayerInputData.ValueRO.inputVector.x, 0, NetcodePlayerInputData.ValueRO.inputVector.y);
            localTransform.ValueRW.Position += moveSpeed * SystemAPI.Time.DeltaTime * moveVector;

        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
