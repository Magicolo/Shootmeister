using System;
using System.Collections.Generic;
using System.Numerics;
using Entia;
using Entia.Core;
using Entia.Modules;
using Entia.Modules.Group;
using Entia.Modules.Message;
using Entia.Nodes;
using Entia.Phases;
using Entia.Queryables;
using static Entia.Nodes.Node;

namespace Game
{
    public unsafe sealed class Environment
    {
        public struct Player : Entia.Queryables.IQueryable
        {
            public Components.Rotation* Rotation;
            public Components.Controller* Controller;
        }

        [All(typeof(Components.IsObservable))]
        public struct Enemy : Entia.Queryables.IQueryable
        {
            public Components.Position* Position;
            public Components.Rotation* Rotation;
            public Components.Velocity* Velocity;
            public Components.ForwardMotion* Motion;
            public Components.Health* Health;
        }

        public struct Reward
        {
            public double Kill, Damage, Shoot, Step, Nothing, Death;
        }

        public static readonly Node Node = Sequence(
            System<Systems.InitializeGame>(),
            System<Systems.UpdateTime>(),

            System<Systems.SpawnEnemy>(),

            System<Systems.UpdateCollision>(),
            System<Systems.UpdateDamage>(),
            System<Systems.UpdateLifetime>(),
            System<Systems.UpdateDeath>(),
            System<Systems.UpdateForwardMotion>(),
            System<Systems.UpdateControl>(),
            System<Systems.UpdateVelocity>(),
            System<Systems.UpdateGame>()
        );

        public const int Observations = 5 * 1;
        public const int Actions = 3;

        public (int current, int maximum) Steps;
        public readonly Reward Rewards;

        readonly Node _node;
        readonly World _world;
        readonly Entia.Modules.Entities _entities;
        readonly Entia.Modules.Components _components;
        readonly Entia.Modules.Messages _messages;
        readonly Entia.Modules.Resources _resources;
        readonly Entia.Modules.Groups _groups;
        readonly Entia.Modules.Controllers _controllers;

        Emitter<Messages.OnDamage> _onDamage;
        Emitter<Messages.OnShoot> _onShoot;
        Emitter<Messages.OnKill> _onKill;
        Emitter<Messages.DoQuit> _doQuit;
        Group<Player> _players;
        Group<Enemy> _enemies;
        Controller _controller;

        public Environment(int steps, in Reward rewards, Node node = null)
        {
            Steps = (0, steps);
            Rewards = rewards;

            _node = node == null ? Node : Sequence(node, Node);
            _world = new World();
            _entities = _world.Entities();
            _components = _world.Components();
            _messages = _world.Messages();
            _resources = _world.Resources();
            _groups = _world.Groups();
            _controllers = _world.Controllers();
        }

        public void Initialize()
        {
            _onDamage = _messages.Emitter<Messages.OnDamage>();
            _onShoot = _messages.Emitter<Messages.OnShoot>();
            _onKill = _messages.Emitter<Messages.OnKill>();
            _doQuit = _messages.Emitter<Messages.DoQuit>();

            _players = _groups.Get<Player>();
            _enemies = _groups.Get<Enemy>();

            _controllers.Control(_node).TryValue(out _controller);
            _controller.Run<Initialize>();
            _controller.Run<React.Initialize>();
        }

        public double[] Observe()
        {
            var observations = new double[Observations];

            if (_players.TryFirst(out var player))
            {
                var direction = Vector2.Transform(Vector2.UnitY, Matrix3x2.CreateRotation(player.Rotation->Angle));
                var index = 1;
                foreach (ref readonly var enemy in _enemies)
                {
                    if (index >= observations.Length) break;

                    double angle = enemy.Rotation->Angle - player.Rotation->Angle + Math.PI;
                    while (angle > Math.PI) angle -= Math.PI * 2.0;
                    while (angle < -Math.PI) angle += Math.PI * 2.0;

                    var distance = Math.Sqrt(enemy.Position->X * enemy.Position->X + enemy.Position->Y * enemy.Position->Y);
                    observations[index++] = angle;
                    // observations[index++] = distance;
                    // observations[index++] = enemy.Motion->Speed;
                    // observations[index++] = enemy.Health->Current;
                }
            }

            return observations;
        }

        public (bool @continue, double[] observations, double reward) Step(double[] actions)
        {
            var reward = 0.0;

            if (actions[2] > 0.5) reward += Rewards.Nothing;
            else
            {
                foreach (ref readonly var player in _players)
                {
                    player.Controller->Direction = (float)(actions[0] * 2.0 - 1.0);
                    player.Controller->Shoot = (float)actions[1];
                }
            }

            using (var onShoot = _onShoot.Receive())
            using (var onDamage = _onDamage.Receive())
            using (var onKill = _onKill.Receive())
            using (var doQuit = _doQuit.Receive(1))
            {
                _controller.Run<Run>();

                reward += onShoot.Pop().Count(message => _players.Has(message.Entity)) * Rewards.Shoot;
                reward += onDamage.Pop().Count(message => _enemies.Has(message.Target)) * Rewards.Damage;
                reward += onKill.Pop().Count(message => _enemies.Has(message.Entity)) * Rewards.Kill;
                reward += doQuit.Count * Rewards.Death;

                var observations = Observe();
                var @continue = ++Steps.current < Steps.maximum && doQuit.Count == 0;
                return (@continue, observations, reward);
            }
        }

        public void Dispose()
        {
            _controller.Run<React.Dispose>();
            _controller.Run<Dispose>();
            _entities.Clear();
            _components.Clear();
            _messages.Clear();
            _resources.Clear();
            _groups.Clear();
            _controllers.Clear();
        }
    }
}