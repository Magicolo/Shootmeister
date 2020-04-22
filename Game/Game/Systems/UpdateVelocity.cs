using System;
using Entia.Experimental;
using Game.Components;

namespace Game
{
    public static partial class Systems
    {
        public static Node UpdateVelocity() =>
            Node.System<Phases.Run>.RunEach((ref Velocity velocity, ref Position position, ref Rotation rotation) =>
            {
                position.X += velocity.X;
                position.Y += velocity.Y;
                rotation.Angle += velocity.Angle;
                rotation.Angle %= 2f * (float)Math.PI;
                velocity = default;
            });
    }
}