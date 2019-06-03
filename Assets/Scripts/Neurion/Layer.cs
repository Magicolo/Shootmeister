namespace Neurion
{
    public sealed class Layer
    {
        public readonly uint Inputs;
        public readonly uint Outputs;
        public readonly Activation Activation;
        public readonly double[,] Weights;
        public readonly double[] Biases;

        public Layer(uint inputs, uint outputs, in Activation activation)
        {
            Inputs = inputs;
            Outputs = outputs;
            Weights = new double[inputs, outputs];
            Biases = new double[outputs];
            Activation = activation;
        }

        Layer(uint inputs, uint outputs, in Activation activation, double[,] weights, double[] biases)
        {
            Inputs = inputs;
            Outputs = outputs;
            Weights = weights;
            Biases = biases;
            Activation = activation;
        }

        public Layer Clone() => new Layer(Inputs, Outputs, Activation, Weights.Clone() as double[,], Biases.Clone() as double[]);
    }
}