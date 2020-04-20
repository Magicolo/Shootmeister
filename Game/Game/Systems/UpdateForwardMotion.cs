using System;
using Entia;
using Entia.Experimental;
using Entia.Injectables;
using Game.Components;

namespace Game
{
    public static partial class Systems
    {
        public static Node UpdateForwardMotion() =>
            Node.With((Resource<Resources.Time>.Read time) =>
            Node.When<Phases.Run>.RunEach((ref ForwardMotion motion, ref Rotation rotation, ref Velocity velocity) =>
            {
                var x = Math.Cos(rotation.Angle);
                var y = Math.Sin(rotation.Angle);
                velocity.X += x * motion.Speed * time.Value.Delta;
                velocity.Y += y * motion.Speed * time.Value.Delta;
            }));
    }
}