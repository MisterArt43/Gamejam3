using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
partial struct NetcodePawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This system requires the PawnData component to be present in the world
        state.RequireForUpdate<PawnData>();
        // You can also require other components if needed, such as NetworkId for networking purposes
        // state.RequireForUpdate<NetworkId>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // This system could be used to manage pawns, their states, and interactions
        // it will manage the pawns' data, such as their faction, health, shield, and other properties.
        foreach ((RefRO<PawnData> pawnData, Entity entity)
        in SystemAPI.Query<RefRO<PawnData>>().WithEntityAccess())
        {
            // Here you can implement logic to handle pawn data, such as updating health, checking faction interactions, etc.
            // For example, you could log the faction of each pawn:
            // Debug.Log($"Pawn Entity: {entity.Index}, Faction: {pawnData.ValueRO.Faction}");
            
            // You can also add more logic here to manage pawns based on their data
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
