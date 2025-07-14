using System.Data;
using Unity.Entities;
using UnityEngine;

public partial class InputMovementSystem : SystemBase
{
    private InputSystem_Actions inputs = null;
    
    protected override void OnCreate()
    {
        inputs = new InputSystem_Actions();
        inputs.Enable();
    }

    protected override void OnUpdate()
    {
        foreach (RefRW<InputMovementData> data in SystemAPI.Query<RefRW<InputMovementData>>())
        {
            data.ValueRW.Movement = inputs.Player.Move.ReadValue<Vector2>();
        }
    }
}
