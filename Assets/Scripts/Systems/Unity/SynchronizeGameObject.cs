using Entia;
using Entia.Core;
using Entia.Injectables;
using Entia.Messages;
using Entia.Queryables;
using Entia.Systems;
using UnityEngine;

namespace Game.Systems
{
    public struct SynchronizeGameObject : IRun, IDispose, IReact<OnPreDestroy>
    {
        public Components<Components.Unity<GameObject>> Components;
        [None(typeof(Components.Unity<GameObject>))]
        public Group<Entity, Maybe<Read<Entia.Components.Debug>>> Group;

        public void React(in OnPreDestroy message)
        {
            if (Components.TryGet(message.Entity, out var item))
                UnityEngine.Object.Destroy(@item.Value);
        }

        public void Run()
        {
            foreach (ref readonly var item in Group)
            {
                var entity = item.Value1;
                ref readonly var debug = ref item.Value2.Has ? ref item.Value2.Value.Value : ref Dummy<Entia.Components.Debug>.Value;
                var @object = new GameObject($"{debug.Name}: {entity}");
                Components.Set(entity, new Components.Unity<GameObject> { Value = @object });
            }
        }

        public void Dispose()
        {
            foreach (var component in Components) if (component.Value) UnityEngine.Object.Destroy(component.Value);
        }
    }
}