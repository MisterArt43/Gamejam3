using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

public struct NetcodePlayerInputData : IInputComponentData
{
    public float2 inputVector; // Represents the input vector for player movement
    public float3 mousePosition; // Represents the mouse position in world space
    public InputEvent shoot; // Represents the input event for player actions
}
