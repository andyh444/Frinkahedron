using Frinkahedron.Core.Colliders;
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
            Camera = new Camera(initialCameraPosition, initialCameraDirection);
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
            ConcurrentBag<(GameObject, GameObject)> collisionPairs = new ConcurrentBag<(GameObject, GameObject)>();
            List<(GameObject, AxisAlignedBoundingBox)> aabbs = new List<(GameObject, AxisAlignedBoundingBox)>(Objects.Count);
            foreach (var obj in Objects)
            {
                if (obj.Collider is not null
                    && obj.RigidBody is not null)
                {
                    aabbs.Add((obj, obj.Collider.CalculateAABB(obj.Position)));
                }
            }

            for (int i = 0; i < aabbs.Count; i++)
            { 
                for (int j = i + 1; j < aabbs.Count; j++)
                {
                    if (aabbs[i].Item2.IntersectsWith(aabbs[j].Item2))
                    {
                        collisionPairs.Add((aabbs[i].Item1, aabbs[j].Item1));
                    }
                }
            }

            TimeSpan aabbTime = Stopwatch.GetElapsedTime(start);

            foreach ((var object1, var object2) in collisionPairs)
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

            Console.WriteLine($"Integration: {20 * integrationTime.TotalMilliseconds:#0.000} ms, AABB: {20 * aabbTime.TotalMilliseconds:#0.000} ms, Collision: {20 * collisionTime.TotalMilliseconds:#0.000} ms, Resolution: {20 * resolutionTime.TotalMilliseconds:#0.000} ms, Inertia: {20 * inverseInertiaTime.TotalMilliseconds:#0.000} ms");
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