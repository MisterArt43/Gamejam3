using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct InputMovementISystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (data, inputs, transform) in SystemAPI.Query<RefRO<PawnData>, RefRO<InputMovementData>, RefRW<LocalTransform>>())
        {
            float3 position = transform.ValueRO.Position;
            float2 movement = inputs.ValueRO.Movement;
            float speed = data.ValueRO.Speed;
            // Ensure movement input is normalized to prevent speed scaling with diagonal movement
            // Update position based on movement input
            position.x += movement.x * speed * SystemAPI.Time.DeltaTime;
            position.z += movement.y * speed * SystemAPI.Time.DeltaTime;
            // Apply the updated position back to the transform
            transform.ValueRW.Position = position;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
