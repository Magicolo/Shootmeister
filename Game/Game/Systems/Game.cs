using Entia.Experimental;
using Entia.Injectables;

namespace Game
{
    public static partial class Systems
    {
        public static Node InitializeGame() =>
            Node.Inject((AllEntities entities, AllComponents components) =>
            Node.System<Phases.Initialize>.Run(() => entities.Player(components)));

        public static Node UpdateGame() =>
            Node.Inject((Components<Components.Controller>.Read controllers, Emitter<Messages.DoQuit> doQuit) =>
            Node.System<Phases.Run>.Run(() =>
            {
                if (controllers.Has()) return;
                doQuit.Emit();
            }));

        public static Node UpdateTime() =>
            Node.System<Phases.Run>.Run((ref Resources.Time time) =>
            {
                time.Current += time.Delta = 1f / 10f;
                time.Frames++;
            });
    }
}