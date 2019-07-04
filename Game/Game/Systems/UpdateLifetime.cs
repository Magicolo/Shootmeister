using Entia;
using Entia.Injectables;
using Entia.Systems;
using Game.Components;

namespace Game.Systems
{
    public struct UpdateLifetime : IRunEach<Lifetime>
    {
        public Resource<Resources.Time>.Read Time;
        public Emitter<Messages.OnKill> DoKill;

        public void Run(Entity entity, ref Lifetime lifetime)
        {
            ref readonly var time = ref Time.Value;
            lifetime.Current += time.Delta;
            if (lifetime.Current >= lifetime.Duration)
                DoKill.Emit(new Messages.OnKill { Entity = entity });
        }
    }
}