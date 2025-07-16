using Unity.Entities;
using Unity.NetCode;

//[GhostComponent] ou ça, ça fait la meme chose que [GhostField] mais pour toute la structure
[GhostComponent]
public struct PawnData : IComponentData
{
    public Faction Faction; // Faction of the pawn
    public float Health; // Health of the pawn
    public float Shield; // Shield of the pawn
    public float Speed; // Speed of the player

}
