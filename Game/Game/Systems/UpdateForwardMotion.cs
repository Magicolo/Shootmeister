using System;
using Entia;
using Entia.Injectables;
using Entia.Systems;
using Game.Components;

namespace Game.Systems
{
    public struct UpdateForwardMotion : IRunEach<ForwardMotion, Rotation, Velocity>
    {
        public Resource<Resources.Time>.Read Time;

        public void Run(Entity entity, ref ForwardMotion motion, ref Rotation rotation, ref Velocity velocity)
        {
            ref readonly var time = ref Time.Value;
            var x = Math.Cos(rotation.Angle);
            var y = Math.Sin(rotation.Angle);
            velocity.X += x * motion.Speed * time.Delta;
            velocity.Y += y * motion.Speed * time.Delta;
        }
    }
}