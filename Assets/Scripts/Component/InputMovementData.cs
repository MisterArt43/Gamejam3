using Unity.Entities;
using Unity.Mathematics;

public struct InputMovementData : IComponentData
{
    public float2 Movement; // Movement input in the X and Y axes
}
