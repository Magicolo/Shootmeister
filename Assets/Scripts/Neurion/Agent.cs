using System;
using System.Linq;
using Entia.Core;
using Entia.Modules.Group;

namespace Neurion
{
    public sealed class Agent
    {
        public readonly Network Brain;
        public readonly double Discount;
        public (double rate, double decay, double minimum) Exploration;
        public readonly Random Random;

        public Agent(Network brain, double discount = 0.95, (double rate, double decay, double minimum)? exploration = null, int? seed = null)
        {
            Brain = brain;
            Discount = discount;
            Exploration = exploration ?? (1.0, 0.001, 0.01);
            Random = seed is int value ? new Random(value) : new Random();
        }

        public (int action, double[] qualities) Act(double[] observations)
        {
            var (qualities, _) = Brain.Predict(observations);
            var action = Random.NextDouble() < Exploration.rate ? Random.Next(0, qualities.Length) : qualities.MaxIndex();
            return (action, qualities);
        }

        public void Train((int action, double[] qualities, double[] observations, double reward)[] replay)
        {
            Exploration.rate = Math.Max(Exploration.rate - Exploration.decay, Exploration.minimum);
            for (var i = replay.Length - 1; i >= 0; i--)
            {
                var current = replay[i];
                var reward = current.reward;
                if (i < replay.Length - 1)
                {
                    var next = replay[i + 1];
                    reward += Discount * next.qualities[next.action];
                }

                current.qualities[current.action] = reward;
                Brain.Train(current.observations, current.qualities);
            }
        }

        public void Mutate(double amount, int? seed = null)
        {
            double Mutation() => Random.NextDouble() * amount * 2.0 - amount;

            for (var l = 0u; l < Brain.Layers.Length; l++)
            {
                var layer = Brain.Layers[l];
                var weights = layer.Weights;
                var biases = layer.Biases;
                for (int o = 0; o < layer.Outputs; o++)
                {
                    for (int i = 0; i < layer.Inputs; i++) weights[i, o] += Mutation();
                    biases[o] += Mutation();
                }
            }
        }

        public Agent Clone() => new Agent(Brain.Clone(), Discount, Exploration);
    }
}