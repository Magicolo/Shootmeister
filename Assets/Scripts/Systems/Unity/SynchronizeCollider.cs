using Entia;
using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;
using UnityEngine;

namespace Game.Systems
{
    public struct SynchronizeCollider : IRun
    {
        public AllComponents Components;
        [All(typeof(Components.Collider)), None(typeof(Components.Unity<CircleCollider2D>))]
        public Group<Entity, Read<Components.Unity<GameObject>>> ToAdd;
        public Group<Read<Components.Collider>, Read<Components.Unity<CircleCollider2D>>> ToUpdate;

        public void Run()
        {
            foreach (ref readonly var item in ToAdd)
            {
                var entity = item.Value1;
                var @object = item.Value2.Value.Value;
                var circle = @object.AddComponent<CircleCollider2D>();
                circle.isTrigger = true;
                Components.Set(entity, new Components.Unity<CircleCollider2D> { Value = circle });
            }

            foreach (ref readonly var item in ToUpdate)
            {
                ref readonly var collider = ref item.Value1.Value;
                var circle = item.Value2.Value.Value;
                circle.radius = collider.Radius / Mathf.Max(circle.transform.localScale.x, circle.transform.localScale.y);
            }
        }
    }
}