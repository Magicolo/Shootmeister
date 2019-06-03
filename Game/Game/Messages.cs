using Entia;

namespace Messages
{
    public struct OnShoot : IMessage { public Entity Entity; public Entity Bullet; }
    public struct OnKill : IMessage { public Entity Entity; }
    public struct OnCollision : IMessage { public Entity Source, Target; }
    public struct OnDamage : IMessage { public Entity Source, Target; public float Amount; }
    public struct DoQuit : IMessage { }
}