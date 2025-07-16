using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class BulletAuthoring : MonoBehaviour
{
    public float Speed = 10f; // Speed of the bullet
    public float Damage = 5f; // Damage dealt by the bullet
    public float Lifetime = 2f; // Time before the bullet is destroyed
    public float TimeAlive = 0f; // Time the bullet has been alive
    public float3 Direction = new float3(0, 0, 1); // Direction the bullet is traveling
}

class BulletAuthoringBaker : Baker<BulletAuthoring>
{
    public override void Bake(BulletAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new BulletData
        {
            Speed = authoring.Speed,
            Damage = authoring.Damage,
            Lifetime = authoring.Lifetime,
            TimeAlive = authoring.TimeAlive,
            Direction = authoring.Direction
        });
    }
}
