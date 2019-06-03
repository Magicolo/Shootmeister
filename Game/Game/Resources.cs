using System;
using System.Collections.Generic;
using Entia;
using Entia.Core;

namespace Game.Resources
{
    public struct Time : IResource { public double Current, Delta; public uint Frames; }
    public struct Spawn : IResource
    {
        [Default]
        public static Spawn Default => new Resources.Spawn
        {
            Rate = (0.15, 0.35),
            Distance = (8.0, 12.0),
            Speed = (0.75, 1.25),
            Health = (1.0, 10.0)
        };

        public (double minimum, double maximum) Rate;
        public (double minimum, double maximum) Distance;
        public (double minimum, double maximum) Speed;
        public (double minimum, double maximum) Health;
        public double Next;
    }
}