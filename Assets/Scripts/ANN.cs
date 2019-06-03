using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entia.Core;
using Neurion;
using UnityEngine;

public sealed class ANN : MonoBehaviour
{
    [Serializable]
    public sealed class Data
    {
        public double[] Features;
        public double[] Labels;
        public double[] Prediction;

        public void Deconstruct(out double[] features, out double[] labels)
        {
            features = Features;
            labels = Labels;
        }
    }

    [Serializable]
    public sealed class DataSet
    {
        public string Name;
        public Data[] Data;
    }

    public bool Test;
    public int Seed = 1;
    public uint Inputs = 2;
    public uint[] Hidden = { 8 };
    public uint Outputs = 1;
    public uint Epochs = 100;
    public double Learn = 0.1;
    public DataSet[] Sets =
    {
        new DataSet
        {
            Name = "AND",
            Data = new []
            {
                new Data { Features = new []{ 1.0, 1.0}, Labels = new []{ 1.0 } },
                new Data { Features = new []{ 0.0, 1.0}, Labels = new []{ 0.0 } },
                new Data { Features = new []{ 1.0, 0.0}, Labels = new []{ 0.0 } },
                new Data { Features = new []{ 0.0, 0.0}, Labels = new []{ 0.0 } },
            }
        },
        new DataSet
        {
            Name = "NAND",
            Data = new []
            {
                new Data { Features = new []{ 1.0, 1.0}, Labels = new []{ 0.0 } },
                new Data { Features = new []{ 0.0, 1.0}, Labels = new []{ 1.0 } },
                new Data { Features = new []{ 1.0, 0.0}, Labels = new []{ 1.0 } },
                new Data { Features = new []{ 0.0, 0.0}, Labels = new []{ 1.0 } },
            }
        },
        new DataSet
        {
            Name = "OR",
            Data = new []
            {
                new Data { Features = new []{ 1.0, 1.0}, Labels = new []{ 1.0 } },
                new Data { Features = new []{ 0.0, 1.0}, Labels = new []{ 1.0 } },
                new Data { Features = new []{ 1.0, 0.0}, Labels = new []{ 1.0 } },
                new Data { Features = new []{ 0.0, 0.0}, Labels = new []{ 0.0 } },
            }
        },
        new DataSet
        {
            Name = "NOR",
            Data = new []
            {
                new Data { Features = new []{ 1.0, 1.0}, Labels = new []{ 0.0 } },
                new Data { Features = new []{ 0.0, 1.0}, Labels = new []{ 0.0 } },
                new Data { Features = new []{ 1.0, 0.0}, Labels = new []{ 0.0 } },
                new Data { Features = new []{ 0.0, 0.0}, Labels = new []{ 1.0 } },
            }
        },
        new DataSet
        {
            Name = "XOR",
            Data = new []
            {
                new Data { Features = new []{ 1.0, 1.0}, Labels = new []{ 0.0 } },
                new Data { Features = new []{ 0.0, 1.0}, Labels = new []{ 1.0 } },
                new Data { Features = new []{ 1.0, 0.0}, Labels = new []{ 1.0 } },
                new Data { Features = new []{ 0.0, 0.0}, Labels = new []{ 0.0 } },
            }
        },
        new DataSet
        {
            Name = "XNOR",
            Data = new []
            {
                new Data { Features = new []{ 1.0, 1.0}, Labels = new []{ 1.0 } },
                new Data { Features = new []{ 0.0, 1.0}, Labels = new []{ 0.0 } },
                new Data { Features = new []{ 1.0, 0.0}, Labels = new []{ 0.0 } },
                new Data { Features = new []{ 0.0, 0.0}, Labels = new []{ 1.0 } },
            }
        },
    };

    void OnValidate()
    {
        if (Test.Change(false))
        {
            Seed++;
            IEnumerable<(Data data, double[] prediction)> Train(DataSet set)
            {
                var layers = new Layer[Hidden.Length + 1];
                var inputs = Inputs;
                for (int i = 0; i < layers.Length; i++)
                {
                    layers[i] = i < layers.Length - 1 ?
                        new Layer(inputs, inputs = Hidden[i], Activation.Tanh) :
                        new Layer(inputs, Outputs, Activation.Sigmoid);
                }
                var network = new Neurion.Network(Learn, layers);
                network.Initialize(Seed);

                for (int e = 0; e < Epochs; e++)
                {
                    foreach (var (features, labels) in set.Data.Shuffle(Seed + e))
                        network.Train(features, labels);
                }

                foreach (var data in set.Data)
                {
                    var prediction = network.Predict(data.Features);
                    yield return (data, prediction.labels);
                }
            }

            var task = Task.WhenAll(Sets.Select(set => Task.Run(() => Train(set))));
            foreach (var (data, predictions) in task.Result.SelectMany(pairs => pairs)) data.Prediction = predictions; ;
        }
    }
}