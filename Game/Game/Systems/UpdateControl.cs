using Entia;
using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;

namespace Game.Systems
{
    public unsafe struct UpdateControl : IRun
    {
        public struct Query : IQueryable
        {
            public Entity Entity;
            public Components.Controller* Controller;
            public Components.Motion* Motion;
            public Components.Position* Position;
            public Components.Rotation* Rotation;
            public Components.Velocity* Velocity;
        }

        public AllEntities Entities;
        public AllComponents Components;
        public Resource<Resources.Time>.Read Time;
        public Group<Query> Group;
        public Emitter<Messages.OnShoot> OnShoot;

        public void Run()
        {
            ref readonly var time = ref Time.Value;
            foreach (ref readonly var item in Group)
            {
                item.Velocity->Angle -= item.Controller->Direction * item.Motion->RotateSpeed * time.Delta;
                if (item.Controller->Shoot > 0.5f)
                {
                    var bullet = Entities.Bullet(Components, *item.Position, *item.Rotation, 6 * item.Controller->Shoot);
                    OnShoot.Emit(new Messages.OnShoot { Entity = item.Entity, Bullet = bullet });
                }
            }
        }
    }
}