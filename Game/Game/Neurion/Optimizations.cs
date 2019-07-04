namespace Neurion
{
    public static class Optimizations
    {
        public readonly struct Context
        {
            public readonly double Value;
            public readonly double Error;
            public readonly double[,] Weigths;
            public readonly double[] Inputs;
            public readonly double[] Outputs;
            public readonly double[] Biases;
            public readonly uint Index;

            public double Weight(uint input) => Weigths[Index, input];
            public double Output => Outputs[Index];
            public double Bias => Biases[Index];

            public Context(double value, double error, double[,] weights, double[] inputs, double[] outputs, double[] biases, uint index)
            {
                Value = value;
                Error = error;
                Weigths = weights;
                Inputs = inputs;
                Outputs = outputs;
                Biases = biases;
                Index = index;
            }
        }

        public delegate double Function(in Context context);
    }
}