using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public class MoveToDestinationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;

            Entities.ForEach((ref Translation translation,
                ref Rotation rotation,
                in Destination destination,
                in MovementSpeed movementSpeed) =>
            {
                if (math.all(destination.value == translation.Value)) return;

                var toDestination = destination.value - translation.Value;
                rotation.Value = quaternion.LookRotation(toDestination, new float3(0, 1, 0));
                var movement = math.normalize(toDestination) * movementSpeed.value * deltaTime;
                if (math.length(movement) >= math.length(toDestination))
                    translation.Value = destination.value;
                else
                    translation.Value += movement;
            }).ScheduleParallel();
        }
    }
}