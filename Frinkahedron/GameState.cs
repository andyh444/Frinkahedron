namespace Frinkahedron.Core
{
    public sealed class GameState
    {
        public HashSet<Key> KeysDown { get; }

        public float DeltaTime { get; set; }

        public Scene Scene { get; }

        public GameState(float deltaTime, Scene scene)
        {
            DeltaTime = deltaTime;
            KeysDown = [];
            Scene = scene;
        }
    }
}