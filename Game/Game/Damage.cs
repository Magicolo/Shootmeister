using System;

namespace Game
{
    [Flags]
    public enum DamageTypes : byte
    {
        Body = 1 << 0,
        Bullet = 1 << 1
    }

    public static class DamageExtensions
    {
        public static bool HasAny(this DamageTypes source, DamageTypes target) => (source & target) != 0;
        public static bool HasAll(this DamageTypes source, DamageTypes target) => (source & target) == target;
        public static bool HasNone(this DamageTypes source, DamageTypes target) => !source.HasAny(target);
    }
}