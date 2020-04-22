using Entia;
using Entia.Experimental;
using Entia.Injectables;
using Game.Components;

namespace Game
{
    public static partial class Systems
    {
        public static Node UpdateDeath() =>
            Node.Inject((AllEntities entities, Emitter<Messages.OnKill> onKill) =>
            Node.System<Phases.Run>.RunEach((Entity entity, ref Health health) =>
            {
                if (health.Current <= 0)
                {
                    onKill.Emit(new Messages.OnKill { Entity = entity });
                    entities.Destroy(entity);
                }
            }));
    }
}