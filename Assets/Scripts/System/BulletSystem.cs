using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
partial struct BulletSystem : ISystem
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
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        bool isServer = state.WorldUnmanaged.IsServer();

        //Note : IJobEntity detect PredictedSimulationSystemGroup and apply <Simulate> automatically
        BallJob job = new()
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            isServer = isServer,
            ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        };

        job.ScheduleParallel();
    }

    [BurstCompile]
    [WithAll(typeof(Simulate))]
    public partial struct BallJob : IJobEntity
    {
        public float DeltaTime;
        public bool isServer;
        public EntityCommandBuffer.ParallelWriter ECB;
        public readonly void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref BulletData bulletData, ref LocalTransform transform)
        {
            float movespeed = 10f;
            // Move the bullet forward in the direction it is facing

            transform.Position += DeltaTime * movespeed * math.forward(transform.Rotation);    

            if (isServer)
            {
                bulletData.timer -= DeltaTime;
                if (bulletData.timer <= 0f)
                {
                    ECB.DestroyEntity(chunkIndex, entity);
                }
            }
        }
    }

    [BurstCompile]
    public readonly void OnDestroy(ref SystemState state)
    {

    }
}
