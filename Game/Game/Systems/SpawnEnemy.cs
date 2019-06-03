using System;
using System.Numerics;
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
                var position = Vector2.Normalize(new Vector2(_random.Next(-1f, 1f), _random.Next(-1f, 1f))) * distance;
                var angle = 3 * Math.PI / 2.0 - Math.Atan2(position.Y, position.X);
                var lifetime = distance * 2f;
                var speed = _random.Next(spawn.Speed);
                var health = _random.Next(spawn.Health);
                Entities.Enemy(Components,
                    new Components.Position { X = position.X, Y = position.Y },
                    new Components.Rotation { Angle = (float)angle },
                    lifetime,
                    speed,
                    health);
            }
        }
    }
}