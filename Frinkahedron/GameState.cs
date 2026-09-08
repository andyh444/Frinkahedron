namespace Frinkahedron.Core
{
    public sealed class GameState
    {
        public Input Input { get; }

        public float DeltaTime { get; set; }

        public Scene Scene { get; }

        public GameState(float deltaTime, Scene scene)
            :this(deltaTime, scene, new Input())
        {
        }

        public GameState(float deltaTime, Scene scene, Input input)
        {
            DeltaTime = deltaTime;
            Input = input;
            Scene = scene;
        }

        public GameState WithNewScene(Scene scene) => new GameState(DeltaTime, scene, Input);
    }
}