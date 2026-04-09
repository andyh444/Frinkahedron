using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using System;
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
            WorldRigidBody[] worldRigidBodies = new WorldRigidBody[Objects.Count];
            int index = 0;
            for (int i = 0; i < Objects.Count; i++)
            {
                GameObject obj = Objects[i];
                if (obj.Collider is not null
                    && obj.RigidBody is not null)
                {
                    worldRigidBodies[index++] = new WorldRigidBody(obj.Position, obj.RigidBody, obj.Collider);
                }
            }

            ConcurrentBag<(int, int)> collisionPairs = new ConcurrentBag<(int, int)>();
            //for (int i = 0; i < aabbs.Count; i++)
            Parallel.For(0, index, i =>
            {
                ref var bodyA = ref worldRigidBodies[i];
                for (int j = i + 1; j < worldRigidBodies.Length; j++)
                {
                    ref var bodyB = ref worldRigidBodies[j];
                    if (bodyA.BoundingBox.IntersectsWith(bodyB.BoundingBox))
                    {
                        //collisionPairs.Add((objA, inertiaA, objB, inertiaB));
                        collisionPairs.Add((i, j));
                    }
                }
            });

            foreach ((var indexA, var indexB) in collisionPairs)
            {
                var object1 = worldRigidBodies[indexA];
                var object2 = worldRigidBodies[indexB];

                RigidBody.ResolveCollision(
                    in object1,
                    in object2);
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