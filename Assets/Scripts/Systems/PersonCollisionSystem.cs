using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace Systems
{
    public class PersonCollisionSystem : SystemBase
    {
        private BuildPhysicsWorld _build;
        private StepPhysicsWorld _step;

        protected override void OnCreate()
        {
            _build = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _step = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        private struct PersonCollisionJob : ITriggerEventsJob
        {
            public void Execute(TriggerEvent triggerEvent)
            {
                
            }
        }
        
        protected override void OnUpdate()
        {
            Dependency = new PersonCollisionJob().Schedule(_step.Simulation, ref _build.PhysicsWorld, Dependency);
        }
    }
}