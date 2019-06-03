using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;
using UnityEngine;

namespace Game.Systems
{
    public struct UpdateInput : IRun
    {
        public Group<Write<Components.Controller>> Group;

        public void Run()
        {
            var left = Input.GetKey(KeyCode.LeftArrow);
            var right = Input.GetKey(KeyCode.RightArrow);
            var shoot = Input.GetKey(KeyCode.Space);
            if (left || right || shoot)
            {
                foreach (ref readonly var item in Group)
                {
                    ref var controller = ref item.Value;
                    controller.Direction = (left ? -1f : 0f) + (right ? 1f : 0f);
                    controller.Shoot = shoot ? 1f : 0f;
                }
            }
        }
    }
}