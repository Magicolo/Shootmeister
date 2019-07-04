using System.Drawing;
using Entia;
using Entia.Injectables;

namespace Game
{
    public static class Templates
    {
        public static Entity Player(this AllEntities entities, AllComponents components)
        {
            var entity = entities.Create();
            components.Set(entity, new Entia.Components.Debug { Name = "Player" });
            components.Set(entity, new Components.Position());
            components.Set(entity, new Components.Velocity());
            components.Set(entity, new Components.Rotation());
            components.Set(entity, new Components.Scale { X = 2.0, Y = 2.5 });
            components.Set(entity, new Components.Motion { MoveSpeed = 0.0, RotateSpeed = 2.0 });
            components.Set(entity, new Components.Sprite { Path = "Shapes/Triangle", Color = Color.Cyan });
            components.Set(entity, new Components.Collider { Radius = 0.25 });
            components.Set(entity, new Components.Controller());
            components.Set(entity, new Components.Damageable { By = DamageTypes.Body });
            components.Set(entity, new Components.Health { Current = 1.0, Maximum = 1.0 });
            return entity;
        }

        public static Entity Enemy(this AllEntities entities, AllComponents components, in Components.Position position, in Components.Rotation rotation, double lifetime, double speed, double health)
        {
            var entity = entities.Create();
            components.Set(entity, new Entia.Components.Debug { Name = "Enemy" });
            components.Set(entity, position);
            components.Set(entity, rotation);
            components.Set(entity, new Components.Scale { X = 3.0, Y = 3.0 });
            components.Set(entity, new Components.Sprite { Path = "Shapes/Square", Color = Color.FromArgb(255, 255, (int)(255 - health * 75.0), 0) });
            components.Set(entity, new Components.Velocity());
            components.Set(entity, new Components.ForwardMotion { Speed = speed });
            components.Set(entity, new Components.Lifetime { Duration = lifetime });
            components.Set(entity, new Components.Collider { Radius = 1.5 });
            components.Set(entity, new Components.Damager { Type = DamageTypes.Body, Amount = 1.0 });
            components.Set(entity, new Components.Damageable { By = DamageTypes.Bullet });
            components.Set(entity, new Components.Health { Current = health, Maximum = health });
            components.Set(entity, new Components.IsObservable());
            return entity;
        }

        public static Entity Bullet(this AllEntities entities, AllComponents components, in Components.Position position, in Components.Rotation rotation, double speed)
        {
            var entity = entities.Create();
            components.Set(entity, new Entia.Components.Debug { Name = "Bullet" });
            components.Set(entity, position);
            components.Set(entity, rotation);
            components.Set(entity, new Components.Scale { X = 0.5, Y = 0.5 });
            components.Set(entity, new Components.Velocity());
            components.Set(entity, new Components.ForwardMotion { Speed = speed });
            components.Set(entity, new Components.Lifetime { Duration = 3.0 });
            components.Set(entity, new Components.Sprite { Path = "Shapes/Circle", Color = Color.Yellow });
            components.Set(entity, new Components.Collider { Radius = 0.1 });
            components.Set(entity, new Components.Damager { Type = DamageTypes.Bullet, Amount = 1.0 });
            components.Set(entity, new Components.Damageable { By = DamageTypes.Body });
            components.Set(entity, new Components.Health { Current = 1.0, Maximum = 1.0 });
            return entity;
        }
    }
}