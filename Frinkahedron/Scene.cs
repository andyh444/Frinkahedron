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
        public Camera Camera { get; }

        public IReadOnlyList<GameObject> Objects { get; }

        public Scene(Vector3 initialCameraPosition, Vector3 initialCameraDirection, IReadOnlyList<GameObject> objects)
        {
            Camera = Camera.CreatePerspectiveCamera(initialCameraPosition, initialCameraDirection);
            Objects = objects;
        }

        public void Update(GameState gameState)
        {
            var start = Stopwatch.GetTimestamp();
            foreach (var obj in Objects)
            {
                obj.Update(gameState);
            }
            TimeSpan integrationTime = Stopwatch.GetElapsedTime(start);

            TimeSpan collisionTime = TimeSpan.Zero;
            TimeSpan resolutionTime = TimeSpan.Zero;
            TimeSpan inverseInertiaTime = TimeSpan.Zero;

            start = Stopwatch.GetTimestamp();
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

            TimeSpan calculateAABBTime = Stopwatch.GetElapsedTime(start);
            start = Stopwatch.GetTimestamp();

            ConcurrentBag<(int, int)> collisionPairs = new ConcurrentBag<(int, int)>();
            //for (int i = 0; i < aabbs.Count; i++)
            Parallel.For(0, worldRigidBodies.Length, i =>
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

            TimeSpan aabbTime = Stopwatch.GetElapsedTime(start);

            foreach ((var indexA, var indexB) in collisionPairs)
            {
                var object1 = worldRigidBodies[indexA];
                var object2 = worldRigidBodies[indexB];

                RigidBody.ResolveCollision(
                    in object1,
                    in object2,
                    ref collisionTime,
                    ref inverseInertiaTime,
                    ref resolutionTime);
            }

            // resolve collisions
            /*for (int i = 0; i < Objects.Count; i++)
            {
                var object1 = Objects[i];
                if (object1.Collider is not null
                    && object1.RigidBody is not null)
                {
                    for (int j = i + 1; j < Objects.Count; j++)
                    {
                        var object2 = Objects[j];
                        if (object2.Collider is not null
                            && object2.RigidBody is not null)
                        {
                            RigidBody.ResolveCollision(
                                object1.Position,
                                object1.Collider,
                                object1.RigidBody,
                                object2.Position,
                                object2.Collider,
                                object2.RigidBody,
                                ref collisionTime,
                                ref resolutionTime,
                                ref inverseInertiaTime);
                        }
                    }
                }
            }*/

            Console.WriteLine($"Integration: {20 * integrationTime.TotalMilliseconds:#0.000} ms, Calculate AABB: {20 * calculateAABBTime.TotalMilliseconds:#0.000}, AABB Pairs: {20 * aabbTime.TotalMilliseconds:#0.000} ms, Collision: {20 * collisionTime.TotalMilliseconds:#0.000} ms, Resolution: {20 * resolutionTime.TotalMilliseconds:#0.000} ms, Inertia: {20 * inverseInertiaTime.TotalMilliseconds:#0.000} ms");
        }

        public void Draw(IRenderer renderer)
        {
            foreach (var obj in Objects)
            {
                obj.Draw(renderer);
            }
        }
    }
}