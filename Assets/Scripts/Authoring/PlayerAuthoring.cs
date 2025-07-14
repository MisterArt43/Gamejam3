using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class PlayerAuthoring : MonoBehaviour
{
    public Faction Faction = Faction.Player; // Faction of the pawn
    public float Health = 10f; // Health of the pawn
    public float Shield = 0f; // Shield of the pawn
    public float Speed = 3f; // Speed of the player
}

class PlayerAuthoringBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new PawnData
        {
            Faction = authoring.Faction,
            Health = authoring.Health,
            Shield = authoring.Shield,
            Speed = authoring.Speed
        });

        AddComponent(entity, new InputMovementData
        { });
        
        
    }
}
