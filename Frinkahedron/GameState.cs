namespace Frinkahedron.Core
{
    public sealed class GameState
    {
        public Input Input { get; }

        public float DeltaTime { get; set; }

        public Scene Scene { get; }

        public GameState(float deltaTime, Scene scene)
        {
            DeltaTime = deltaTime;
            Input = new Input();
            Scene = scene;
        }
    }
}