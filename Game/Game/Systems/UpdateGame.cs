using System;
using Entia;
using Entia.Core;
using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;

namespace Game.Systems
{
    public struct UpdateGame : IRun
    {
        [All(typeof(Components.Controller))]
        public Group<Entity> Group;
        public Emitter<Messages.DoQuit> DoQuit;

        public void Run()
        {
            if (Group.None()) DoQuit.Emit();
        }
    }
}