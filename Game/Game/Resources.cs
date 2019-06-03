using System;
using System.Collections.Generic;
using Entia;
using Entia.Core;

namespace Game.Resources
{
    public struct Time : IResource { public float Current; public float Delta; public uint Frames; }
    public struct Spawn : IResource
    {
        [Default]
        public static Spawn Default => new Resources.Spawn
        {
            Rate = (0.15f, 0.35f),
            Distance = (8f, 12f),
            Speed = (0.75f, 1.25f),
            Health = (1, 10)
        };

        public (float minimum, float maximum) Rate;
        public (float minimum, float maximum) Distance;
        public (float minimum, float maximum) Speed;
        public (float minimum, float maximum) Health;
        public float Next;
    }
}