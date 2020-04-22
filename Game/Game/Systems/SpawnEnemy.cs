using System;
using Entia.Experimental;
using Entia.Injectables;

namespace Game
{
    public static partial class Systems
    {
        public static Node SpawnEnemy()
        {
            var random = new Random();
            return
                Node.Inject((AllEntities entities, AllComponents components) =>
                Node.System<Phases.Run>.Run((ref Resources.Spawn spawn, ref Resources.Time time) =>
                {
                    if (time.Current >= spawn.Next)
                    {
                        spawn.Next += 1f / random.Next(spawn.Rate);

                        var distance = random.Next(spawn.Distance);
                        var x = random.Next(-1.0, 1.0);
                        var y = random.Next(-1.0, 1.0);
                        var magnitude = Math.Sqrt(x * x + y * y);
                        var position = (x: x * distance / magnitude, y: y * distance / magnitude);
                        var angle = Math.Atan2(position.y, position.x) + Math.PI;
                        var lifetime = distance * 2.0;
                        var speed = random.Next(spawn.Speed);
                        var health = random.Next(spawn.Health);
                        entities.Enemy(components,
                            new Components.Position { X = position.x, Y = position.y },
                            new Components.Rotation { Angle = angle },
                            lifetime,
                            speed,
                            health);
                    }
                }));
        }
    }
}