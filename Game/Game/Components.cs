using System.Drawing;
using Entia;

namespace Game.Components
{
    public struct Position : IComponent { public float X, Y; }
    public struct Velocity : IComponent { public float X, Y, Angle; }
    public struct Rotation : IComponent { public float Angle; }
    public struct Scale : IComponent { public float X, Y; }
    public struct Sprite : IComponent { public string Path; public Color Color; }
    public struct Motion : IComponent { public float MoveSpeed, RotateSpeed; }
    public struct Controller : IComponent { public float Direction, Shoot; }
    public struct ForwardMotion : IComponent { public float Speed; }
    public struct Lifetime : IComponent { public float Duration, Current; }
    public struct Collider : IComponent { public float Radius; }
    public struct Damager : IComponent { public DamageTypes Type; public float Amount; }
    public struct Damageable : IComponent { public DamageTypes By; }
    public struct Health : IComponent { public float Maximum, Current; }
    public struct IsObservable : IComponent { }
}