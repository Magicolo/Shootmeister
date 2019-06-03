namespace Neurion
{
    public static class Losses
    {
        public readonly struct Context
        {
            public readonly double Value;
            public readonly double[] Inputs;
            public readonly double[] Outputs;
            public readonly uint Index;

            public double Output => Outputs[Index];

            public Context(double value, double[] inputs, double[] outputs, uint index)
            {
                Value = value;
                Inputs = inputs;
                Outputs = outputs;
                Index = index;
            }
        }

        public delegate double Function(in Context context);
    }
}