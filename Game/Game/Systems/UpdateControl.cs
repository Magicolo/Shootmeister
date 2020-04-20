using Entia;
using Entia.Experimental;
using Entia.Injectables;
using Game.Components;

namespace Game
{
    public static partial class Systems
    {
        public static Node UpdateControl() =>
            Node.With((AllEntities entities, AllComponents components, Resource<Resources.Time>.Read time, Emitter<Messages.OnShoot> onShoot) =>
            Node.When<Phases.Run>.RunEach((Entity entity, ref Components.Controller controller, ref Motion motion, ref Position position, ref Rotation rotation, ref Velocity velocity) =>
            {
                velocity.Angle -= controller.Direction * motion.RotateSpeed * time.Value.Delta;
                if (controller.Shoot > 0.5f)
                {
                    var bullet = entities.Bullet(components, position, rotation, 6 * controller.Shoot);
                    onShoot.Emit(new Messages.OnShoot { Entity = entity, Bullet = bullet });
                }
                controller = default;
            }));
    }
}