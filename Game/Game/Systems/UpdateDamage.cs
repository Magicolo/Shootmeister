using Entia.Experimental;
using Entia.Injectables;
using Entia.Queryables;

namespace Game
{
    public static partial class Systems
    {
        public unsafe struct UpdateDamageQuery : IQueryable
        {
            public Components.Damageable* Damageable;
            public Components.Health* Health;
        }

        public static unsafe Node UpdateDamage() =>
            Node.With((
                AllEntities entities,
                Components<Components.Damager>.Read damagers,
                Receiver<Messages.OnCollision> onCollision,
                Emitter<Messages.OnDamage> onDamage,
                Group<UpdateDamageQuery> group) =>
            Node.When<Phases.Run>.Receive<Messages.OnCollision>.Run((in Messages.OnCollision message) =>
            {
                if (damagers.TryGet(message.Source, out var damager) &&
                    group.TryGet(message.Target, out var item) &&
                    item.Damageable->By.HasAny(damager.Type))
                {
                    item.Health->Current -= damager.Amount;
                    onDamage.Emit(new Messages.OnDamage { Source = message.Source, Target = message.Target, Amount = damager.Amount });
                }
            }));
    }
}