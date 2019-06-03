using System;
using System.Collections.Generic;
using Entia.Injectables;
using Entia.Systems;

namespace Game.Systems
{
    public struct InitializeGame : IInitialize
    {
        public AllEntities Entities;
        public AllComponents Components;

        public void Initialize() => Entities.Player(Components);
    }
}