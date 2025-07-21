using Unity.Entities;
using Unity.Mathematics;

public struct BulletData : IComponentData
{
    // public float Speed; // Speed of the bullet
    // public float Damage; // Damage dealt by the bullet
    // public float Lifetime; // Time before the bullet is destroyed
    // public float TimeAlive; // Time the bullet has been alive
    // public float3 Direction; // Direction the bullet is traveling
    public float timer;
}
