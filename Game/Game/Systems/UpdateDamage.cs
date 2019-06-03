using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;

namespace Game.Systems
{
    public unsafe struct UpdateDamage : IRun
    {
        public struct Query : IQueryable
        {
            public Components.Damageable* Damageable;
            public Components.Health* Health;
        }

        public AllEntities Entities;
        public Components<Components.Damager>.Read Damagers;
        public Receiver<Messages.OnCollision> OnCollision;
        public Emitter<Messages.OnDamage> OnDamage;
        public Group<Query> Group;

        public void Run()
        {
            while (OnCollision.TryPop(out var message))
            {
                if (Damagers.TryGet(message.Source, out var damager) &&
                    Group.TryGet(message.Target, out var item) &&
                    item.Damageable->By.HasAny(damager.Type))
                {
                    item.Health->Current -= damager.Amount;
                    OnDamage.Emit(new Messages.OnDamage { Source = message.Source, Target = message.Target, Amount = damager.Amount });
                }
            }
        }
    }
}