using System;

namespace Neurion
{
    public readonly struct Activation
    {
        public readonly struct Context
        {
            public readonly double[] Inputs;
            public readonly double[] Outputs;
            public readonly double Output;
            public readonly uint Index;

            public Context(double[] inputs, double[] outputs, uint index)
            {
                Inputs = inputs;
                Outputs = outputs;
                Output = outputs[index];
                Index = index;
            }
        }

        public delegate double ForwardFunction(double value, double[] values);
        public delegate double BackwardFunction(double value, double[] values, double forward, double[] forwards);

        public static readonly Activation Identity = new Activation(
            (value, _) => value,
            (_, __, ___, ____) => 1.0
        );
        public static readonly Activation Tanh = new Activation(
            (value, _) => Math.Tanh(value),
            (_, __, forward, ___) => 1.0 - forward * forward
        );
        public static readonly Activation Sigmoid = new Activation(
            (value, _) => 1.0 / (1.0 + Math.Exp(-value)),
            (_, __, forward, ___) => forward * (1.0 - forward)
        );
        public static readonly Activation Step = new Activation(
            (value, _) => value > 0 ? 1.0 : 0.0,
            (_, __, ___, ____) => 0.0
        );
        public static readonly Activation SoftMax = new Activation(
            (value, values) =>
            {
                var sum = 0.0;
                for (int i = 0; i < values.Length; i++) sum += Math.Exp(values[i]);
                return Math.Exp(value) / sum;
            },
            (_, __, forward, ___) => (1.0 - forward) * forward
        );
        public static readonly Activation Relu = new Activation(
            (value, _) => Math.Max(0.0, value),
            (value, _, __, ___) => value < 0.0 ? 0.0 : 1.0
        );
        public static Activation LeakyRelu(double leakage = 0.01) => new Activation(
            (value, _) => value < 0.0 ? value * leakage : value,
            (value, _, __, ___) => value < 0.0 ? leakage : 1.0
        );

        public readonly Activation.ForwardFunction Forward;
        public readonly Activation.BackwardFunction Backward;

        public Activation(Activation.ForwardFunction forward, Activation.BackwardFunction backward)
        {
            Forward = forward;
            Backward = backward;
        }
    }
}