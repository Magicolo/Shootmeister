using Entia;
using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;
using UnityEngine;

namespace Game.Systems
{
    public struct SynchronizeSprite : IRun
    {
        public AllComponents Components;
        [All(typeof(Components.Sprite)), None(typeof(Components.Unity<SpriteRenderer>))]
        public Group<Entity, Read<Components.Unity<GameObject>>> ToAdd;
        public Group<Read<Components.Sprite>, Read<Components.Unity<SpriteRenderer>>> ToUpdate;

        public void Run()
        {
            foreach (ref readonly var item in ToAdd)
            {
                var entity = item.Value1;
                var @object = item.Value2.Value.Value;
                var renderer = @object.AddComponent<SpriteRenderer>();
                Components.Set(entity, new Components.Unity<SpriteRenderer> { Value = renderer });
            }

            foreach (ref readonly var item in ToUpdate)
            {
                ref readonly var sprite = ref item.Value1.Value;
                var renderer = item.Value2.Value.Value;
                renderer.sprite = UnityEngine.Resources.Load<Sprite>(sprite.Path);
                renderer.color = new Color(sprite.Color.R, sprite.Color.G, sprite.Color.B, sprite.Color.A);
            }
        }
    }
}