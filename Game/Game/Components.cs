using System.Drawing;
using Entia;

namespace Game.Components
{
    public struct Position : IComponent { public double X, Y; }
    public struct Velocity : IComponent { public double X, Y, Angle; }
    public struct Rotation : IComponent { public double Angle; }
    public struct Scale : IComponent { public double X, Y; }
    public struct Sprite : IComponent { public string Path; public Color Color; }
    public struct Motion : IComponent { public double MoveSpeed, RotateSpeed; }
    public struct Controller : IComponent { public double Direction, Shoot; }
    public struct ForwardMotion : IComponent { public double Speed; }
    public struct Lifetime : IComponent { public double Duration, Current; }
    public struct Collider : IComponent { public double Radius; }
    public struct Damager : IComponent { public DamageTypes Type; public double Amount; }
    public struct Damageable : IComponent { public DamageTypes By; }
    public struct Health : IComponent { public double Maximum, Current; }
    public struct IsObservable : IComponent { }
}