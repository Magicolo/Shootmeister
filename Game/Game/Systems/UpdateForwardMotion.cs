using System;
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
                var x = Math.Cos(item.Rotation->Angle);
                var y = Math.Sin(item.Rotation->Angle);
                item.Velocity->X += x * item.Motion->Speed * time.Delta;
                item.Velocity->Y += y * item.Motion->Speed * time.Delta;
            }
        }
    }
}