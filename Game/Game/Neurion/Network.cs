using System;
using System.Linq;

namespace Neurion
{
    public sealed class Network
    {
        public uint Inputs => Layers[0].Inputs;
        public uint Outputs => Layers[Layers.Length - 1].Outputs;
        public readonly double Learn;
        public readonly Layer[] Layers;

        public Network(double learn, params Layer[] layers)
        {
            Learn = learn;
            Layers = layers;
        }

        public void Initialize(int? seed = null) => Initialize(seed is int value ? new Random(value) : new Random());
        public void Initialize(Random random) => Initialize((_, __, ___) => random.NextDouble() * 2.0 - 1.0, (_, __) => random.NextDouble() * 2.0 - 1.0);
        public void Initialize(Func<uint, uint, uint, double> weight, Func<uint, uint, double> bias)
        {
            for (var l = 0u; l < Layers.Length; l++)
            {
                var layer = Layers[l];
                var weights = layer.Weights;
                var biases = layer.Biases;
                for (var i = 0u; i < layer.Inputs; i++) for (var o = 0u; o < layer.Outputs; o++) weights[i, o] = weight(l, i, o);
                for (var o = 0u; o < layer.Outputs; o++) biases[o] = bias(l, o);
            }
        }

        public (double[] labels, (double[] inputs, double[] outputs, double[] gradients)[] results) Train(double[] features, double[] labels)
        {
            var results = new (double[] inputs, double[] outputs, double[] gradients)[Layers.Length];
            var predictions = Predict(features);
            var first = true;

            for (var l = Layers.Length - 1; l >= 0; l--)
            {
                var layer = Layers[l];
                var (inputs, outputs, forwards) = predictions.results[l];
                var gradients = new double[forwards.Length];

                if (first)
                {
                    first = false;
                    for (var o = 0u; o < forwards.Length; o++)
                    {
                        var forward = forwards[o];
                        var gradient = labels[o] - forward;
                        gradient *= layer.Activation.Backward(outputs[o], outputs, forward, forwards);
                        gradients[o] = gradient;
                        for (var i = 0u; i < inputs.Length; i++) layer.Weights[i, o] += gradient * inputs[i] * Learn;
                        layer.Biases[o] += gradient * Learn;
                    }
                }
                else
                {
                    var nextLayer = Layers[l + 1];
                    var nextWeights = nextLayer.Weights;
                    var nextGradients = results[l + 1].gradients;
                    for (var o1 = 0u; o1 < forwards.Length; o1++)
                    {
                        var forward = forwards[o1];
                        var gradient = 0.0;
                        for (var o2 = 0u; o2 < nextGradients.Length; o2++) gradient += nextGradients[o2] * nextWeights[o1, o2];
                        gradient *= layer.Activation.Backward(outputs[o1], outputs, forward, forwards);
                        gradients[o1] = gradient;
                        for (var i = 0u; i < inputs.Length; i++) layer.Weights[i, o1] += gradient * inputs[i] * Learn;
                        layer.Biases[o1] += gradient * Learn;
                    }
                }

                results[l] = (inputs, forwards, gradients);
            }

            return (predictions.labels, results);
        }

        public (double[] labels, (double[] inputs, double[] outputs, double[] forwards)[] results) Predict(double[] features)
        {
            var results = new (double[], double[], double[])[Layers.Length];
            var inputs = features;
            for (var l = 0u; l < Layers.Length; l++)
            {
                var layer = Layers[l];
                var activation = layer.Activation;
                var weights = layer.Weights;
                var biases = layer.Biases;
                var outputs = new double[layer.Outputs];
                var forwards = new double[layer.Outputs];
                for (var o = 0u; o < outputs.Length; o++)
                {
                    var sum = biases[o];
                    for (var i = 0u; i < inputs.Length; i++) sum += inputs[i] * weights[i, o];
                    outputs[o] = sum;
                }

                for (var o = 0u; o < outputs.Length; o++) forwards[o] = activation.Forward(outputs[o], outputs);
                results[l] = (inputs, outputs, forwards);
                inputs = forwards;
            }

            return (inputs, results);
        }

        public Network Clone() => new Network(Learn, Layers.Select(layer => layer.Clone()).ToArray());
    }
}