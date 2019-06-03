using Entia;
using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;

namespace Game.Systems
{
    public unsafe struct UpdateCollision : IRun
    {
        public struct Query : IQueryable
        {
            public Entity Entity;
            public Components.Position* Position;
            public Components.Collider* Collider;
        }

        public Emitter<Messages.OnCollision> OnCollision;
        public Group<Query> Group;

        public void Run()
        {
            foreach (ref readonly var source in Group)
            {
                foreach (ref readonly var target in Group)
                {
                    if (source.Entity == target.Entity) continue;

                    var x = source.Position->X - target.Position->X;
                    var y = source.Position->Y - target.Position->Y;
                    var radius = source.Collider->Radius + target.Collider->Radius;
                    if (x * x + y * y < radius * radius)
                        OnCollision.Emit(new Messages.OnCollision { Source = source.Entity, Target = target.Entity });
                }
            }
        }
    }
}