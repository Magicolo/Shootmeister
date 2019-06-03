using Entia;
using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;

namespace Game.Systems
{
    public unsafe struct UpdateLifetime : IRun
    {
        public struct Query : IQueryable
        {
            public Entity Entity;
            public Components.Lifetime* Lifetime;
        }

        public AllEntities Entities;
        public Resource<Resources.Time>.Read Time;
        public Group<Query> Group;
        public Emitter<Messages.OnKill> DoKill;

        public void Run()
        {
            ref readonly var time = ref Time.Value;
            foreach (ref readonly var item in Group)
            {
                item.Lifetime->Current += time.Delta;
                if (item.Lifetime->Current >= item.Lifetime->Duration)
                    DoKill.Emit(new Messages.OnKill { Entity = item.Entity });
            }
        }
    }
}