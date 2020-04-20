using System.Linq;
using Entia;
using Entia.Core;
using Entia.Experimental;
using Entia.Experimental.Scheduling;
using Entia.Modules;
using Entia.Modules.Group;
using Entia.Modules.Message;
using Entia.Queryables;

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
            public double Damage, Shoot, Step, Death, Nothing;
        }

        public static readonly Node Node = Node.Sequence(
            Systems.InitializeGame,
            Systems.UpdateTime,

            Systems.SpawnEnemy,

            Systems.UpdateCollision,
            Systems.UpdateDamage,
            Systems.UpdateLifetime,
            Systems.UpdateDeath,
            Systems.UpdateForwardMotion,
            Systems.UpdateControl,
            Systems.UpdateVelocity,
            Systems.UpdateGame
        );

        public const int Actions = 4;
        public const int Observations = 5 * 4 + 1;

        public (int current, int maximum) Steps;
        public Reward Rewards;

        readonly Node _node;
        readonly World _world;
        readonly Entia.Modules.Entities _entities;
        readonly Entia.Modules.Components _components;
        readonly Entia.Modules.Messages _messages;
        readonly Entia.Modules.Resources _resources;
        readonly Entia.Modules.Groups _groups;

        Emitter<Phases.Initialize> _initialize;
        Emitter<Phases.Run> _run;
        Emitter<Messages.OnDamage> _onDamage;
        Emitter<Messages.OnShoot> _onShoot;
        Emitter<Messages.OnKill> _onKill;
        Emitter<Messages.DoQuit> _doQuit;
        Group<Player> _players;
        Group<Enemy> _enemies;
        Disposable _systems;

        public Environment(int steps, in Reward rewards, Node node = null)
        {
            Steps = (0, steps);
            Rewards = rewards;

            _node = node == null ? Node : Node.Sequence(node, Node);
            _world = new World();
            _entities = _world.Entities();
            _components = _world.Components();
            _messages = _world.Messages();
            _resources = _world.Resources();
            _groups = _world.Groups();
        }

        public Disposable<Environment> Use()
        {
            Initialize();
            return new Disposable<Environment>(this, state => state.Dispose());
        }

        public void Initialize()
        {
            _initialize = _messages.Emitter<Phases.Initialize>();
            _run = _messages.Emitter<Phases.Run>();
            _onDamage = _messages.Emitter<Messages.OnDamage>();
            _onShoot = _messages.Emitter<Messages.OnShoot>();
            _onKill = _messages.Emitter<Messages.OnKill>();
            _doQuit = _messages.Emitter<Messages.DoQuit>();

            _players = _groups.Get<Player>();
            _enemies = _groups.Get<Enemy>();

            _world.Schedule(_node).TryValue(out _systems);
            _initialize.Emit();
        }

        public double[] Observe()
        {
            var observations = new double[Observations];
            if (_players.TryFirst(out var player)) observations[0] = player.Rotation->Angle;

            var index = 1;
            foreach (ref readonly var enemy in _enemies)
            {
                if (index >= observations.Length) break;
                observations[index++] = enemy.Position->X;
                observations[index++] = enemy.Position->Y;
                observations[index++] = enemy.Motion->Speed;
                observations[index++] = enemy.Health->Current;
            }

            return observations;
        }

        public (bool done, double[] observations, double reward) Step(int action)
        {
            var reward = 0.0;

            if (_players.TryFirst(out var player))
            {
                switch (action)
                {
                    case 0: player.Controller->Direction = -1.0; break;
                    case 1: player.Controller->Direction = 1.0; break;
                    case 2: player.Controller->Shoot = 1.0; break;
                    case 3: reward += Rewards.Nothing; break;
                }
            }

            using (var onShoot = _onShoot.Receive())
            using (var onDamage = _onDamage.Receive())
            using (var onKill = _onKill.Receive())
            using (var doQuit = _doQuit.Receive(1))
            {
                _run.Emit();
                reward += onShoot.Messages().Count(message => _players.Has(message.Entity)) * Rewards.Shoot;
                reward += onDamage.Messages().Count(message => _enemies.Has(message.Target)) * Rewards.Damage;
                reward += onKill.Messages().Count(message => _players.Has(message.Entity)) * Rewards.Death;

                var observations = Observe();
                var done = ++Steps.current >= Steps.maximum || doQuit.Count > 0;
                reward += done ? 0.0 : Rewards.Step;
                return (done, observations, reward);
            }
        }

        public void Dispose()
        {
            _entities.Clear();
            _resources.Clear();
            _world.Resolve();
            Steps.current = 0;
        }
    }
}