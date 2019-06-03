using System;
using System.Collections.Generic;

namespace Game
{
    public static class Extensions
    {
        public static T[] DequeueAll<T>(this Queue<T> queue) => queue.Dequeue(queue.Count);

        public static T[] Dequeue<T>(this Queue<T> queue, int count)
        {
            count = Math.Min(count, queue.Count);
            var values = new T[count];
            for (int i = 0; i < count; i++) values[i] = queue.Dequeue();
            return values;
        }

        public static double Next(this Random random, double minimum, double maximum) => random.NextDouble() * (maximum - minimum) + minimum;
        public static float Next(this Random random, float minimum, float maximum) => (float)random.NextDouble() * (maximum - minimum) + minimum;
        public static double Next(this Random random, in (double minimum, double maximum) range) => random.Next(range.minimum, range.maximum);
        public static float Next(this Random random, in (float minimum, float maximum) range) => random.Next(range.minimum, range.maximum);
    }
}