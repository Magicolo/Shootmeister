using Entia;
using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;

namespace Game.Systems
{
    public unsafe struct UpdateDeath : IRun
    {
        public struct Query : IQueryable
        {
            public Entity Entity;
            public Components.Health* Health;
        }

        public AllEntities Entities;
        public Group<Query> Group;
        public Emitter<Messages.OnKill> OnKill;

        public void Run()
        {
            foreach (ref readonly var item in Group)
            {
                if (item.Health->Current <= 0)
                {
                    OnKill.Emit(new Messages.OnKill { Entity = item.Entity });
                    Entities.Destroy(item.Entity);
                }
            }
        }
    }
}