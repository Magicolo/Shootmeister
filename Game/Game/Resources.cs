using Entia;
using Entia.Core;

namespace Game.Resources
{
    public struct Time : IResource { public double Current, Delta; public uint Frames; }
    public struct Spawn : IResource
    {
        [Default]
        public static Spawn Default => new Spawn
        {
            Rate = (0.1, 0.2),
            Distance = (8.0, 12.0),
            Speed = (0.25, 0.5),
            Health = (1.0, 3.0)
        };

        public (double minimum, double maximum) Rate;
        public (double minimum, double maximum) Distance;
        public (double minimum, double maximum) Speed;
        public (double minimum, double maximum) Health;
        public double Next;
    }
}