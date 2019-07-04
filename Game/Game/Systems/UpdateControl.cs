using Entia;
using Entia.Injectables;
using Entia.Systems;
using Game.Components;

namespace Game.Systems
{
    public struct UpdateControl : IRunEach<Components.Controller, Motion, Position, Rotation, Velocity>
    {
        public AllEntities Entities;
        public AllComponents Components;
        public Resource<Resources.Time>.Read Time;
        public Emitter<Messages.OnShoot> OnShoot;

        public void Run(Entity entity, ref Components.Controller controller, ref Motion motion, ref Position position, ref Rotation rotation, ref Velocity velocity)
        {
            ref readonly var time = ref Time.Value;
            velocity.Angle -= controller.Direction * motion.RotateSpeed * time.Delta;
            if (controller.Shoot > 0.5f)
            {
                var bullet = Entities.Bullet(Components, position, rotation, 6 * controller.Shoot);
                OnShoot.Emit(new Messages.OnShoot { Entity = entity, Bullet = bullet });
            }
            controller = default;
        }
    }
}