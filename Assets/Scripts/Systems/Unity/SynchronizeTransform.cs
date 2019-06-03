using Entia;
using Entia.Core;
using Entia.Injectables;
using Entia.Messages;
using Entia.Queryables;
using Entia.Systems;
using UnityEngine;

namespace Game.Systems
{
    public struct SynchronizeTransfom : IRun
    {
        public struct Query : IQueryable
        {
            public Read<Components.Position> Position;
            public Maybe<Read<Components.Rotation>> Rotation;
            public Maybe<Read<Components.Scale>> Scale;
            public Write<Components.Unity<Transform>> Transform;
        }

        public AllComponents Components;
        [All(typeof(Components.Position), typeof(Components.Rotation)), None(typeof(Components.Unity<Transform>))]
        public Group<Entity, Read<Components.Unity<GameObject>>> ToAdd;
        public Group<Query> ToUpdate;

        public void Run()
        {
            foreach (ref readonly var item in ToAdd)
            {
                var entity = item.Value1;
                var @object = item.Value2.Value.Value;
                Components.Set(entity, new Components.Unity<Transform> { Value = @object.transform });
            }

            foreach (ref readonly var item in ToUpdate)
            {
                ref readonly var position = ref item.Position.Value;
                ref readonly var rotation = ref item.Rotation.Get(out var hasRotation);
                ref readonly var scale = ref item.Scale.Get(out var hasScale);
                var transform = item.Transform.Value.Value;

                transform.localPosition = new Vector2((float)position.X, (float)position.Y);
                if (hasRotation) transform.localRotation = Quaternion.Euler(0f, 0f, (float)rotation.Angle * Mathf.Rad2Deg - 90f);
                if (hasScale) transform.localScale = new Vector2((float)scale.X, (float)scale.Y);
            }
        }
    }
}