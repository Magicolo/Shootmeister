using Entia;
using Entia.Experimental;
using Entia.Injectables;
using Entia.Queryables;

namespace Game
{
    public static partial class Systems
    {
        public unsafe struct UpdateCollisionQuery : IQueryable
        {
            public Entity Entity;
            public Components.Position* Position;
            public Components.Collider* Collider;
        }

        public static unsafe Node UpdateCollision() =>
            Node.Inject((Emitter<Messages.OnCollision> onCollision, Group<UpdateCollisionQuery> group) =>
            Node.System<Phases.Run>.Run(() =>
            {
                foreach (ref readonly var source in group)
                {
                    foreach (ref readonly var target in group)
                    {
                        if (source.Entity == target.Entity) continue;

                        var x = source.Position->X - target.Position->X;
                        var y = source.Position->Y - target.Position->Y;
                        var radius = source.Collider->Radius + target.Collider->Radius;
                        if (x * x + y * y < radius * radius)
                            onCollision.Emit(new Messages.OnCollision { Source = source.Entity, Target = target.Entity });
                    }
                }
            }));
    }
}