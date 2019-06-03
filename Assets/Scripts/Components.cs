using Entia;
using UnityEngine;

namespace Game.Components
{
    public struct Unity<T> : IComponent where T : UnityEngine.Object { public T Value; }
}