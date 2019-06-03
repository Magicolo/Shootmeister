// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Numerics;
// using Entia;
// using Entia.Core;
// using Entia.Modules;
// using Entia.Phases;
// using Entia.Queryables;

// namespace Neurion
// {
//     public unsafe sealed class Environment
//     {
//         public struct Player : Entia.Queryables.IQueryable
//         {
//             public Components.Rotation* Rotation;
//             public Components.Controller* Controller;
//         }

//         [All(typeof(Components.IsObservable))]
//         public struct Enemy : Entia.Queryables.IQueryable
//         {
//             public Components.Position* Position;
//             public Components.Rotation* Rotation;
//             public Components.Velocity* Velocity;
//             public Components.ForwardMotion* Motion;
//             public Components.Health* Health;
//         }

//         public struct Reward
//         {
//             public double Kill, Damage, Shoot, Step, Nothing, Death;
//         }

//         public readonly Controller Controller;
//         public readonly Reward Rewards;
//         public readonly Queue<(double[] actions, double[] observations, double reward)> Replay;
//         public readonly int Memory;
//         public readonly int Maximum;
//         public int Steps { get; private set; }

//         public Environment(Controller controller, in Reward rewards, int memory, int maximum)
//         {
//             Controller = controller;
//             Rewards = rewards;
//             Replay = new Queue<(double[], double[], double)>(memory);
//             Memory = memory;
//             Maximum = maximum;
//         }

//         public void Initialize()
//         {
//             Controller.Run<Initialize>();
//             Controller.Run<React.Initialize>();
//         }

//         public void Dispose()
//         {
//             Controller.World.Entities().Clear();
//             Controller.World.Resources().Clear();
//             Controller.Run<React.Dispose>();
//             Controller.Run<Dispose>();
//         }

//         public double[] Observe()
//         {
//             var observations = new double[5 * 1];
//             var players = Controller.World.Groups().Get<Player>();
//             var enemies = Controller.World.Groups().Get<Enemy>();

//             if (players.TryFirst(out var player))
//             {
//                 var direction = Vector2.Transform(Vector2.UnitY, Matrix3x2.CreateRotation(player.Rotation->Angle));
//                 var index = 1;
//                 foreach (ref readonly var enemy in enemies)
//                 {
//                     if (index >= observations.Length) break;

//                     double angle = enemy.Rotation->Angle - player.Rotation->Angle + Math.PI;
//                     while (angle > Math.PI) angle -= Math.PI * 2.0;
//                     while (angle < -Math.PI) angle += Math.PI * 2.0;

//                     var distance = Math.Sqrt(enemy.Position->X * enemy.Position->X + enemy.Position->Y * enemy.Position->Y);
//                     observations[index++] = angle;
//                     // observations[index++] = distance;
//                     // observations[index++] = enemy.Motion->Speed;
//                     // observations[index++] = enemy.Health->Current;
//                 }
//             }

//             return observations;
//         }

//         public (bool @continue, double[] observations, double reward) Step(double[] actions)
//         {
//             var players = Controller.World.Groups().Get<Player>();
//             var enemies = Controller.World.Groups().Get<Enemy>();
//             var reward = 0.0;

//             if (actions[2] > 0.5) reward += Rewards.Nothing;
//             else
//             {
//                 foreach (ref readonly var player in players)
//                 {
//                     player.Controller->Direction = (float)(actions[0] * 2.0 - 1.0);
//                     player.Controller->Shoot = (float)actions[1];
//                 }
//             }

//             var messages = Controller.World.Messages();
//             using (var onShoot = messages.Receive<Messages.OnShoot>())
//             using (var onDamage = messages.Receive<Messages.OnDamage>())
//             using (var onKill = messages.Receive<Messages.OnKill>())
//             using (var doQuit = messages.Receive<Messages.DoQuit>(1))
//             {
//                 Controller.Run<Run>();

//                 reward += onShoot.Pop().Count(message => players.Has(message.Entity)) * Rewards.Shoot;
//                 reward += onDamage.Pop().Count(message => enemies.Has(message.Target)) * Rewards.Damage;
//                 reward += onKill.Pop().Count(message => enemies.Has(message.Entity)) * Rewards.Kill;
//                 reward += doQuit.Count * Rewards.Death;

//                 var observations = Observe();
//                 var @continue = ++Steps < Maximum && doQuit.Count == 0;
//                 Replay.Enqueue((actions, observations, reward));
//                 while (Replay.Count > Memory) Replay.Dequeue();
//                 return (@continue, observations, reward);
//             }
//         }
//     }
// }