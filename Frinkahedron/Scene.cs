using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core
{

    public sealed class Scene
    {
        private readonly List<GameObject> objects;
        private readonly List<GameObject> toAdd;
        private readonly HashSet<GameObject> toRemove;

        public Camera Camera { get; }

        public IReadOnlyList<GameObject> Objects => objects;

        public SceneLights SceneLights { get; }

        public int TicksPerUpdate { get; set; } = 20;

        public bool CollisionsEnabled { get; set; } = true;

        public Scene(Vector3 initialCameraPosition, Vector3 initialCameraDirection, float cameraAspectRatio, IReadOnlyList<GameObject> objects)
        {
            Camera = Camera.CreatePerspectiveCamera(initialCameraPosition, initialCameraDirection, cameraAspectRatio);
            this.objects = objects.ToList();
            toAdd = new List<GameObject>();
            toRemove = new HashSet<GameObject>();
            SceneLights = new SceneLights();
        }

        public void AddObject(GameObject obj)
        {
            toAdd.Add(obj);
        }

        public void RemoveObject(GameObject obj)
        {
            toRemove.Add(obj);
        }

        public void Update(GameState gameState)
        {
            float originalDeltaTime = gameState.DeltaTime;
            gameState.DeltaTime /= TicksPerUpdate;
            for (int i = 0; i < TicksPerUpdate; i++)
            {
                if (toRemove.Count > 0)
                {
                    objects.RemoveAll(toRemove.Contains);
                    toRemove.Clear();
                }

                objects.AddRange(toAdd);
                toAdd.Clear();


                foreach (var obj in Objects)
                {
                    obj.Update(gameState);
                }
                if (CollisionsEnabled)
                {
                    ResolveAllCollisions();
                }
                gameState.Input.Clear();
            }
            gameState.DeltaTime = originalDeltaTime;
        }

        private void ResolveAllCollisions()
        {
            // we need to check for collisions with all dynamic body pairs
            // and collisions with all static-dynamic body pairs
            // but don't need to check static-static body pairs as they never move
            WorldRigidBody[] worldDynamicBodies = ArrayPool<WorldRigidBody>.Shared.Rent(Objects.Count);
            WorldRigidBody[] worldStaticBodies = ArrayPool<WorldRigidBody>.Shared.Rent(Objects.Count);
            try
            {
                int dynamicIndex = 0;
                int staticIndex = 0;
                for (int i = 0; i < Objects.Count; i++)
                {
                    GameObject obj = Objects[i];
                    if (obj.Collider is not null
                        && obj.RigidBody is not null)
                    {
                        if (obj.RigidBody.RigidBodyType == RigidBodyType.Dynamic)
                        {
                            worldDynamicBodies[dynamicIndex++] = new WorldRigidBody(obj.Position, obj.RigidBody, obj.Collider);
                        }
                        else
                        {
                            worldStaticBodies[staticIndex++] = new WorldRigidBody(obj.Position, obj.RigidBody, obj.Collider);
                        }
                    }
                }

                ConcurrentBag<(int, int)> dynamicCollisionPairs = new ConcurrentBag<(int, int)>();
                ConcurrentBag<(int staticIndex, int dynamicIndex)> staticDynamicCollisionPairs = new ConcurrentBag<(int, int)>();

                Parallel.For(0, dynamicIndex, i =>
                {
                    ref var bodyA = ref worldDynamicBodies[i];
                    for (int j = i + 1; j < dynamicIndex; j++)
                    {
                        ref var bodyB = ref worldDynamicBodies[j];
                        if (bodyA.BoundingBox.IntersectsWith(bodyB.BoundingBox))
                        {
                            dynamicCollisionPairs.Add((i, j));
                        }
                    }

                    for (int j = 0; j < staticIndex; j++)
                    {
                        ref var bodyB = ref worldStaticBodies[j];
                        if (bodyA.BoundingBox.IntersectsWith(bodyB.BoundingBox))
                        {
                            staticDynamicCollisionPairs.Add((j, i));
                        }
                    }
                });

                foreach ((var indexA, var indexB) in dynamicCollisionPairs)
                {
                    var object1 = worldDynamicBodies[indexA];
                    var object2 = worldDynamicBodies[indexB];

                    WorldRigidBody.ResolveCollision(
                        in object1,
                        in object2);
                }

                foreach ((var indexA, var indexB) in staticDynamicCollisionPairs)
                {
                    var object1 = worldStaticBodies[indexA];
                    var object2 = worldDynamicBodies[indexB];

                    WorldRigidBody.ResolveCollision(
                        in object1,
                        in object2);
                }
            }
            finally
            {
                ArrayPool<WorldRigidBody>.Shared.Return(worldDynamicBodies);
                ArrayPool<WorldRigidBody>.Shared.Return(worldStaticBodies);
            }
        }

        public void Draw(IRenderContext renderer)
        {
            foreach (var obj in Objects)
            {
                obj.Draw(renderer);
            }
        }
    }
}