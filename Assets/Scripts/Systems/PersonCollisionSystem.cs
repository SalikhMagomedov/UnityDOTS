using System;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Random = Unity.Mathematics.Random;

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
            [ReadOnly] public ComponentDataFromEntity<PersonTag> personGroup;
            public ComponentDataFromEntity<URPMaterialPropertyBaseColor> colorGroup;
            public float seed;

            public void Execute(TriggerEvent triggerEvent)
            {
                var isEntityAPerson = personGroup.HasComponent(triggerEvent.EntityA);
                var isEntityBPerson = personGroup.HasComponent(triggerEvent.EntityB);

                if (!isEntityAPerson || !isEntityBPerson) return;

                var random = new Random((uint)(1 + seed + triggerEvent.BodyIndexA * triggerEvent.BodyIndexB));
                random = ChangeMaterialColor(random, triggerEvent.EntityA);
                ChangeMaterialColor(random, triggerEvent.EntityB);
            }

            private Random ChangeMaterialColor(Random random, Entity entity)
            {
                if (!colorGroup.HasComponent(entity)) return random;
                
                var colorComponent = colorGroup[entity];
                colorComponent.Value.xyz = random.NextFloat3();
                colorGroup[entity] = colorComponent;

                return random;
            }
        }

        protected override void OnUpdate()
        {
            Dependency = new PersonCollisionJob
            {
                personGroup = GetComponentDataFromEntity<PersonTag>(true),
                colorGroup = GetComponentDataFromEntity<URPMaterialPropertyBaseColor>(),
                seed = DateTimeOffset.Now.Millisecond
            }.Schedule(_step.Simulation, ref _build.PhysicsWorld, Dependency);
        }
    }
}