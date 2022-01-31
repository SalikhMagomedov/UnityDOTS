﻿using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public class NewDestinationSystem : SystemBase
    {
        private RandomSystem _randomSystem;

        protected override void OnCreate()
        {
            _randomSystem = World.GetExistingSystem<RandomSystem>();
        }

        protected override void OnUpdate()
        {
            var randomArray = _randomSystem.RandomArray;
            Entities
                .WithNativeDisableParallelForRestriction(randomArray)
                .ForEach((int nativeThreadIndex, ref Destination destination, in Translation translation) =>
                {
                    var distance = math.abs(math.length(destination.value - translation.Value));
                    if (!(distance <= .1f)) return;
                    var random = randomArray[nativeThreadIndex];
                    destination.value.x = random.NextFloat(0, 500);
                    destination.value.z = random.NextFloat(0, 500);
                    randomArray[nativeThreadIndex] = random;
                }).ScheduleParallel();
        }
    }
}