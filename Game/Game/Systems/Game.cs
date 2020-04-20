using Entia.Experimental;
using Entia.Injectables;

namespace Game
{
    public static partial class Systems
    {
        public static Node InitializeGame() =>
            Node.With((AllEntities entities, AllComponents components) =>
            Node.When<Phases.Initialize>.Run(() => entities.Player(components)));

        public static Node UpdateGame() =>
            Node.With((Components<Components.Controller>.Read controllers, Emitter<Messages.DoQuit> doQuit) =>
            Node.When<Phases.Run>.Run(() =>
            {
                if (controllers.Has()) return;
                doQuit.Emit();
            }));

        public static Node UpdateTime() =>
            Node.When<Phases.Run>.Run((ref Resources.Time time) =>
            {
                time.Current += time.Delta = 1f / 10f;
                time.Frames++;
            });
    }
}