using Entia.Injectables;
using Entia.Systems;

namespace Game.Systems
{
    public struct UpdateTime : IRun
    {
        public Resource<Resources.Time> Time;

        public void Run()
        {
            ref var time = ref Time.Value;
            time.Current += time.Delta = 1f / 10f;
            time.Frames++;
        }
    }
}