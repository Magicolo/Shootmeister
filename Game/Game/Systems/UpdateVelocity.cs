using System;
using Entia;
using Entia.Systems;
using Game.Components;

namespace Game.Systems
{
    public struct UpdateVelocity : IRunEach<Velocity, Position, Rotation>
    {
        public void Run(Entity entity, ref Velocity velocity, ref Position position, ref Rotation rotation)
        {
            position.X += velocity.X;
            position.Y += velocity.Y;
            rotation.Angle += velocity.Angle;
            rotation.Angle %= 2f * (float)Math.PI;
            velocity = default;
        }
    }
}