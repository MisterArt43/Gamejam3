using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
// [UpdateAfter(typeof(GhostInputSystemGroup))]
partial struct PlayerMouseLookSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // state.RequireForUpdate<Camera>();
        state.RequireForUpdate<GhostOwnerIsLocal>();
    }

    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;


        foreach ((RefRW<LocalTransform> localTransform, RefRO<GhostOwner> ghostOwner, RefRO<NetcodePlayerInputData> netcodePlayerInputData)
        in SystemAPI.Query<RefRW<LocalTransform>, RefRO<GhostOwner>, RefRO<NetcodePlayerInputData>>().WithAll<Simulate, GhostOwnerIsLocal>())
        {
            Ray ray = mainCamera.ScreenPointToRay(netcodePlayerInputData.ValueRO.mousePosition);

            float3 playerPosition = localTransform.ValueRW.Position;
            Plane groundPlane = new Plane(Vector3.up, playerPosition.y);

            if (groundPlane.Raycast(ray, out float distance))
            {
                float3 worldMousePosition = ray.GetPoint(distance);

                float3 direction = math.normalize(worldMousePosition - playerPosition);
                direction.y = 0; // Keep the direction horizontal

                if (math.lengthsq(direction) > 0.001f)
                {
                    quaternion targetRotation = quaternion.LookRotationSafe(direction, math.up());

                    localTransform.ValueRW.Rotation = targetRotation;
                }
            }
        }
    }


    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}