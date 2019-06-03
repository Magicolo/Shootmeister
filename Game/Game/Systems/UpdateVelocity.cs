using System;
using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;

namespace Game.Systems
{
    public unsafe struct UpdateVelocity : IRun
    {
        public struct Query : IQueryable
        {
            public Components.Velocity* Velocity;
            public Components.Position* Position;
            public Components.Rotation* Rotation;
        }

        public Group<Query> Group;

        public void Run()
        {
            foreach (ref readonly var item in Group)
            {
                item.Position->X += item.Velocity->X;
                item.Position->Y += item.Velocity->Y;
                item.Rotation->Angle += item.Velocity->Angle;
                item.Rotation->Angle %= 2f * (float)Math.PI;
                *item.Velocity = default;
            }
        }
    }
}