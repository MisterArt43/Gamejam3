using Unity.Entities;

public struct PawnData : IComponentData
{
    public Faction Faction; // Faction of the pawn
    public float Health; // Health of the pawn
    public float Shield; // Shield of the pawn
    public float Speed; // Speed of the player

}
