using Entia;
using Entia.Experimental;
using Entia.Injectables;
using Game.Components;

namespace Game
{
    public static partial class Systems
    {
        public static Node UpdateLifetime() =>
            Node.With((Resource<Resources.Time> time, Emitter<Messages.OnKill> onKill) =>
            Node.When<Phases.Run>.RunEach((Entity entity, ref Lifetime lifetime) =>
            {
                lifetime.Current += time.Value.Delta;
                if (lifetime.Current >= lifetime.Duration)
                    // BUG: destroy entity?
                    onKill.Emit(new Messages.OnKill { Entity = entity });
            }));
    }
}