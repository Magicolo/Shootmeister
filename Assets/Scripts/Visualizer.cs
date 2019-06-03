using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entia;
using Entia.Core;
using Entia.Modules;
using Entia.Nodes;
using static Entia.Nodes.Node;
using Entia.Phases;
using UnityEngine;

namespace Game
{
    public class Visualizer : MonoBehaviour
    {
        delegate bool TryParse<T>(string text, out T value);

        public static readonly Node Node = Sequence(
            System<Systems.SynchronizeGameObject>(),
            System<Systems.SynchronizeTransfom>(),
            System<Systems.SynchronizeSprite>(),
            System<Systems.SynchronizeCollider>()
        );

        public const int Agents = 8;

        public bool Paused;

        int _speed = 1;
        bool _fade = true;
        double _mutation = 10.0;
        int _trainInterval = 13;
        int _maximumSteps = 1000;
        int _memory = 100;
        int[] _epochs;
        int[] _steps;
        int[] _bestSteps;
        double[] _rewards;
        double[] _bestRewards;
        Neurion.Agent[] _agents;
        Environment.Reward _reward = new Environment.Reward
        {
            Shoot = -0.05,
            Damage = 0.05,
            Kill = 0.2,
            Nothing = 0.01,
            Step = 0.1,
            Death = -5.0,
        };

        readonly Neurion.Network _brain = new Neurion.Network(0.0001,
            new Neurion.Layer(Environment.Observations, 128, Neurion.Activation.Tanh),
            new Neurion.Layer(128, 64, Neurion.Activation.Sigmoid),
            new Neurion.Layer(64, Environment.Actions, Neurion.Activation.SoftMax)
        );

        void OnGUI()
        {
            var style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = new Color(1f, 1f, 1f, _fade ? 0.75f : 0.1f);

            void Label(string label) => GUILayout.Label(label, style);
            void TextField<T>(string label, ref T value, TryParse<T> parse)
            {
                GUILayout.BeginHorizontal();
                Label(label);
                if (parse(GUILayout.TextField(value.ToString()), out var parsed)) value = parsed;
                GUILayout.EndHorizontal();
            }

            Label($"Epochs: {_epochs.Sum()}");
            Label($"Best Reward: {_bestRewards.Max()}");
            Label($"Best Steps: {_bestSteps.Max()}");
            Label($"Speed: {_speed}");
            Label($"Paused: {Paused}");
            Label("P = Pause | E = Explore | I = Initialize | F = Fade | Numpad = Speed");

            GUILayout.Space(20f);
            TextField("Train Interval: ", ref _trainInterval, int.TryParse);
            TextField("Maximum Steps: ", ref _maximumSteps, int.TryParse);
            TextField("Memory: ", ref _memory, int.TryParse);
            TextField("Mutation: ", ref _mutation, double.TryParse);
            Label("Rewards: ");
            GUILayout.BeginHorizontal();
            GUILayout.Space(20f);
            TextField("Shoot: ", ref _reward.Shoot, double.TryParse);
            Label(" | ");
            TextField("Damage: ", ref _reward.Damage, double.TryParse);
            Label(" | ");
            TextField("Kill: ", ref _reward.Kill, double.TryParse);
            Label(" | ");
            TextField("Nothing: ", ref _reward.Nothing, double.TryParse);
            Label(" | ");
            TextField("Step: ", ref _reward.Step, double.TryParse);
            Label(" | ");
            TextField("Death: ", ref _reward.Death, double.TryParse);
            GUILayout.EndHorizontal();

            GUILayout.Space(20f);
            Label("Agents");
            for (int i = 0; i < _agents.Length; i++)
            {
                var agent = _agents[i];
                var steps = _steps[i].ToString("0000");
                var bestSteps = _bestSteps[i].ToString("0000");
                var exploration = Math.Round(agent.Exploration.rate, 3).ToString("0.000");
                var reward = Math.Round(_rewards[i], 3).ToString("00.00");
                var bestReward = Math.Round(_bestRewards[i], 3).ToString("00.00");
                GUILayout.BeginHorizontal();
                GUILayout.Space(20f);
                Label($"Epoch: {_epochs[i]} | Exploration: {exploration} | Reward: {reward} | Best Reward: {bestReward} | Steps: {steps} | Best Steps: {bestSteps}");
                GUILayout.EndHorizontal();
            }
        }

        void Initialize()
        {
            _epochs = new int[Agents];
            _steps = new int[Agents];
            _bestSteps = new int[Agents];
            _rewards = new double[Agents];
            _bestRewards = new double[Agents];
            _bestRewards.Fill(_reward.Death);
            _agents = new Neurion.Agent[Agents];
            for (int i = 0; i < _agents.Length; i++)
            {
                var brain = _brain.Clone();
                brain.Initialize();
                _agents[i] = new Neurion.Agent(brain);
            }
        }

        IEnumerator Start()
        {
            var quit = false;
            Application.quitting += () => quit = true;

            Initialize();

            for (int i = 1; i < Agents; i++)
            {
                var index = i;
                Task.Run(() => Asynchronous(index, index * 10));
            }
            foreach (var _ in Synchronous(0, 1)) yield return _;

            void UpdateStep(int index, int steps, double reward)
            {
                _steps[index] = steps;
                _bestSteps[index] = Math.Max(_bestSteps[index], steps);
                _rewards[index] += reward;
            }

            Neurion.Agent Next(int index, int evolve)
            {
                ref var bestReward = ref _bestRewards[index];
                ref var reward = ref _rewards[index];
                bestReward = Math.Max(bestReward, reward);
                reward = 0.0;

                if (++_epochs[index] % evolve == 0)
                {
                    var best = (index, reward: bestReward);
                    for (int i = 0; i < _bestRewards.Length; i++)
                    {
                        var current = _bestRewards[i];
                        if (current > best.reward) best = (i, current);
                    }
                    _bestSteps[index] = 0;
                    bestReward = _reward.Death;

                    var agent = _agents[best.index];
                    lock (agent)
                    {
                        var clone = agent.Clone();
                        clone.Mutate(_mutation);
                        return _agents[index] = clone;
                    }
                }
                return _agents[index];
            }

            void Asynchronous(int index, int evolve)
            {
                var agent = _agents[index];
                while (true)
                {
                    foreach (var (steps, reward) in Run(agent, false))
                    {
                        UpdateStep(index, steps, reward);
                        while (Paused || quit) if (quit) throw new TaskCanceledException(); else Thread.Sleep(100);
                    }
                    agent = Next(index, evolve);
                }
            }

            IEnumerable Synchronous(int index, int evolve)
            {
                var agent = _agents[index];
                while (true)
                {
                    foreach (var (steps, reward) in Run(agent, true))
                    {
                        UpdateStep(index, steps, reward);
                        if (steps % _speed == 0) yield return null;
                    }
                    agent = Next(index, evolve);
                }
            }

            IEnumerable<(int steps, double reward)> Run(Neurion.Agent agent, bool visualize)
            {
                var replay = new Queue<(double[] actions, double[] observations, double reward)>();
                var environment = new Environment(_maximumSteps, _reward, visualize ? Node : default);
                environment.Initialize();

                var observations = new double[_brain.Inputs];
                var @continue = true;

                while (@continue)
                {
                    var actions = agent.Act(observations);
                    var result = environment.Step(actions);
                    @continue = result.@continue;
                    observations = result.observations;

                    replay.Enqueue((actions, result.observations, result.reward));
                    if (replay.Count > _memory) replay.Dequeue();

                    if (environment.Steps.current % _trainInterval == 0) Train();
                    yield return (environment.Steps.current, result.reward);
                }

                Train();
                environment.Dispose();

                void Train()
                {
                    lock (agent) agent.Train(replay.ToArray());
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P)) Paused = !Paused;
            if (Input.GetKeyDown(KeyCode.E)) _agents.Iterate(agent => agent.Exploration.rate = 1.0);
            if (Input.GetKeyDown(KeyCode.I)) Initialize();
            if (Input.GetKeyDown(KeyCode.F)) _fade = !_fade;
            UpdateTime();
        }

        bool UpdateTime()
        {
            bool TrySet(KeyCode key, int iterations)
            {
                if (Input.GetKeyDown(key))
                {
                    _speed = iterations;
                    return true;
                }
                return false;
            }

            Application.targetFrameRate = 30;
            return
                TrySet(KeyCode.Keypad0, 0) ||
                TrySet(KeyCode.Keypad1, 1) ||
                TrySet(KeyCode.Keypad2, 2) ||
                TrySet(KeyCode.Keypad3, 5) ||
                TrySet(KeyCode.Keypad4, 10) ||
                TrySet(KeyCode.Keypad5, 20) ||
                TrySet(KeyCode.Keypad6, 50) ||
                TrySet(KeyCode.Keypad7, 100) ||
                TrySet(KeyCode.Keypad8, 200) ||
                TrySet(KeyCode.Keypad9, 500);
        }
    }
}