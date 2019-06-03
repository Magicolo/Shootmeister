using System;
using System.Collections.Generic;
using Entia.Injectables;
using Entia.Systems;

namespace Game.Systems
{
    public struct InitializeGame : IInitialize
    {
        public AllResources Resources;
        public AllEntities Entities;
        public AllComponents Components;

        public void Initialize()
        {
            Resources.Set(new Resources.Spawn
            {
                Rate = (0.2f, 0.4f),
                Distance = (8f, 12f),
                Speed = (0.75f, 1.25f),
                Health = (1, 10)
            });
            Entities.Player(Components);
        }
    }
}