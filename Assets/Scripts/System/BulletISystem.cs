using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct BulletISystem : ISystem
{
    [BurstCompile]
    public readonly void OnCreate(ref SystemState state)
    {
        // Initialize any required components or systems here
        state.RequireForUpdate<BulletData>();
        // This ensures that the system will only run when BulletData is present
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BallJob job = new()
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        job.ScheduleParallel();
    }

    // [BurstCompile]
    // public void OnFixedUpdate(ref SystemState state)
    // {

    // }

    public partial struct BallJob : IJobEntity
    {
        public float DeltaTime;
        public readonly void Execute(ref BulletData bulletData, ref LocalTransform transform)
        {
            transform = transform.Translate(bulletData.Speed * DeltaTime * bulletData.Direction);
        }
    }

    [BurstCompile]
    public readonly void OnDestroy(ref SystemState state)
    {
        
    }
}
