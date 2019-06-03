using System;
using Entia.Core;
using Entia.Injectables;
using Entia.Systems;

namespace Game.Systems
{
    public struct SpawnEnemy : IRun
    {
        [Default]
        public static SpawnEnemy Default => new SpawnEnemy { _random = new System.Random() };

        public AllEntities Entities;
        public AllComponents Components;
        public Resource<Resources.Spawn> Spawn;
        public Resource<Resources.Time>.Read Time;

        System.Random _random;

        public void Run()
        {
            ref var spawn = ref Spawn.Value;
            ref readonly var time = ref Time.Value;

            if (time.Current >= spawn.Next)
            {
                spawn.Next += 1f / _random.Next(spawn.Rate);

                var distance = _random.Next(spawn.Distance);
                var x = _random.Next(-1.0, 1.0);
                var y = _random.Next(-1.0, 1.0);
                var magnitude = Math.Sqrt(x * x + y * y);
                var position = (x: x * distance / magnitude, y: y * distance / magnitude);
                var angle = Math.Atan2(position.y, position.x) + Math.PI;
                var lifetime = distance * 2.0;
                var speed = _random.Next(spawn.Speed);
                var health = _random.Next(spawn.Health);
                Entities.Enemy(Components,
                    new Components.Position { X = position.x, Y = position.y },
                    new Components.Rotation { Angle = angle },
                    lifetime,
                    speed,
                    health);
            }
        }
    }
}