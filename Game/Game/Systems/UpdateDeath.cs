using Entia;
using Entia.Injectables;
using Entia.Systems;
using Game.Components;

namespace Game.Systems
{
    public struct UpdateDeath : IRunEach<Health>
    {
        public AllEntities Entities;
        public Emitter<Messages.OnKill> OnKill;

        public void Run(Entity entity, ref Health health)
        {
            if (health.Current <= 0)
            {
                OnKill.Emit(new Messages.OnKill { Entity = entity });
                Entities.Destroy(entity);
            }
        }
    }
}