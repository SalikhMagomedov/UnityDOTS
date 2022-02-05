using Components;
using Unity.Entities;

namespace Systems
{
    public class LifetimeSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulation;

        protected override void OnCreate()
        {
            _endSimulation = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            var ecb = _endSimulation.CreateCommandBuffer().AsParallelWriter();

            Entities.ForEach((Entity entity, int entityInQueryIndex, ref Lifetime lifetime) =>
            {
                lifetime.value -= deltaTime;

                if (lifetime.value <= 0f) ecb.DestroyEntity(entityInQueryIndex, entity);
            }).ScheduleParallel();

            _endSimulation.AddJobHandleForProducer(Dependency);
        }
    }
}