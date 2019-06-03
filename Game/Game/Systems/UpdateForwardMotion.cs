using System;
using System.Numerics;
using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;

namespace Game.Systems
{
    public unsafe struct UpdateForwardMotion : IRun
    {
        public struct Query : IQueryable
        {
            public Components.ForwardMotion* Motion;
            public Components.Rotation* Rotation;
            public Components.Velocity* Velocity;
        }

        public Resource<Resources.Time>.Read Time;
        public Group<Query> Group;

        public void Run()
        {
            ref readonly var time = ref Time.Value;
            foreach (ref readonly var item in Group)
            {
                var direction = Vector2.Transform(Vector2.UnitY, Matrix3x2.CreateRotation(-item.Rotation->Angle));
                var move = direction * item.Motion->Speed * time.Delta;
                item.Velocity->X += move.X;
                item.Velocity->Y += move.Y;
            }
        }
    }
}