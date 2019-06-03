using System;
using System.Linq;
using Entia.Core;
using Entia.Modules.Group;

namespace Neurion
{
    public sealed class Agent
    {
        static int Maximum(double[] values)
        {
            if (values.Length == 0) return -1;

            var maximum = values[0];
            var index = 0;
            for (var i = 1; i < values.Length; i++)
            {
                var value = values[i];
                if (value > maximum)
                {
                    maximum = value;
                    index = i;
                }
            }
            return index;
        }

        public readonly Network Brain;
        public readonly double Discount;
        public (double rate, double decay, double minimum) Exploration;
        public readonly Random Random;

        public Agent(Network brain, double discount = 0.95, (double rate, double decay, double minimum)? exploration = null, int? seed = null)
        {
            Brain = brain;
            Discount = discount;
            Exploration = exploration ?? (1.0, 0.0001, 0.01);
            Random = seed is int value ? new Random(value) : new Random();
        }

        public double[] Act(double[] observations)
        {
            var (actions, _) = Brain.Predict(observations);
            for (int i = 0; i < actions.Length; i++) if (Random.NextDouble() < Exploration.rate) actions[i] = Random.NextDouble();
            Exploration.rate = Math.Max(Exploration.rate - Exploration.decay, Exploration.minimum);
            return actions;
        }

        public void Train((double[] actions, double[] observations, double reward)[] replay)
        {
            for (var i = replay.Length - 1; i >= 0; i--)
            {
                var current = replay[i];
                var feedback = current.reward;
                if (i < replay.Length - 1)
                {
                    var next = replay[i + 1];
                    var maximum = next.actions.Max();
                    feedback += Discount * maximum;
                }

                var labels = current.actions.ToArray();
                var index = Maximum(labels);
                labels[index] = feedback;
                Brain.Train(current.observations, labels);
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