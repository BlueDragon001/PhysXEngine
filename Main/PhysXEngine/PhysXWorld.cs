using System;
using System.Collections.Generic;
using UnityEngine;

namespace PhysX
{
    public sealed class PhysXWorld 
    {
        public static readonly float MinBodySize = 0.01f * 0.01f;
        public static readonly float MaxBodySize = 64f * 64f;

        public static readonly float MinDensity = 0.5f;     // g/cm^3
        public static readonly float MaxDensity = 21.4f;

        public static readonly int MinIterations = 1;
        public static readonly int MaxIterations = 128;

        private PhysXVector gravity;
        private List<PhysXBody> bodyList;

        public int BodyCount
        {
            get { return this.bodyList.Count; }
        }

        public PhysXWorld()
        {
            this.gravity = new PhysXVector(0f, -9.81f, 0f);
            this.bodyList = new List<PhysXBody>();
        }

        public void AddBody(PhysXBody body)
        {
            this.bodyList.Add(body);
        }

        public bool RemoveBody(PhysXBody body)
        {
            return this.bodyList.Remove(body);
        }

        public bool GetBody(int index, out PhysXBody body)
        {
            body = null;

            if (index < 0 || index >= this.bodyList.Count)
            {
                return false;
            }

            body = this.bodyList[index];
            return true;
        }

        public void Step(float time, int iterations)
        {
            iterations = PhysXMath.Clamp(iterations, PhysXWorld.MinIterations, PhysXWorld.MaxIterations);

            for (int it = 0; it < iterations; it++)
            {
                // Movement step
                for (int i = 0; i < this.bodyList.Count; i++)
                {
                    this.bodyList[i].Step(time, this.gravity, iterations);
                }

                // collision step
                for (int i = 0; i < this.bodyList.Count - 1; i++)
                {
                    PhysXBody bodyA = this.bodyList[i];

                    for (int j = i + 1; j < this.bodyList.Count; j++)
                    {
                        PhysXBody bodyB = this.bodyList[j];

                        if (bodyA.IsStatic && bodyB.IsStatic)
                        {
                            continue;
                        }

                        if (this.Collide(bodyA, bodyB, out PhysXVector normal, out float depth))
                        {
                            if (bodyA.IsStatic)
                            {
                                bodyB.Move(normal * depth);
                            }
                            else if (bodyB.IsStatic)
                            {
                                bodyA.Move(-normal * depth);
                            }
                            else
                            {
                                bodyA.Move(-normal * depth / 2f);
                                bodyB.Move(normal * depth / 2f);
                            }

                            this.ResolveCollision(bodyA, bodyB, normal, depth);
                        }
                    }
                }
            }
        }

        public void ResolveCollision(PhysXBody bodyA, PhysXBody bodyB, PhysXVector normal, float depth)
        {
            PhysXVector relativeVelocity = bodyB.LinearVelocity - bodyA.LinearVelocity;

            if (PhysXMath.Dot(relativeVelocity, normal) > 0f)
            {
                return;
            }

            float e = MathF.Min(bodyA.Restitution, bodyB.Restitution);

            float j = -(1f + e) * PhysXMath.Dot(relativeVelocity, normal);
            j /= bodyA.InvMass + bodyB.InvMass;

            PhysXVector impulse = j * normal;

            bodyA.LinearVelocity -= impulse * bodyA.InvMass;
            bodyB.LinearVelocity += impulse * bodyB.InvMass;
        }

        public bool Collide(PhysXBody bodyA, PhysXBody bodyB, out PhysXVector normal, out float depth)
        {
            normal = PhysXVector.Zero;
            depth = 0f;

            ShapeType shapeTypeA = bodyA.ShapeType;
            ShapeType shapeTypeB = bodyB.ShapeType;

            if (shapeTypeA is ShapeType.Cuboid)
            {
                if (shapeTypeB is ShapeType.Cuboid)
                {
                    return Collisions.IntersectPolygons(
                        bodyA.Position, bodyA.GetTransformedVertices(),
                        bodyB.Position, bodyB.GetTransformedVertices(),
                        out normal, out depth);
                }
                else if (shapeTypeB is ShapeType.Sphere)
                {
                    bool result = Collisions.IntersectCirclePolygon(
                        bodyB.Position, bodyB.Radius,
                        bodyA.Position, bodyA.GetTransformedVertices(),
                        out normal, out depth);

                    normal = -normal;
                    return result;
                }
            }
            else if (shapeTypeA is ShapeType.Sphere)
            {
                if (shapeTypeB is ShapeType.Cuboid)
                {
                    return Collisions.IntersectCirclePolygon(
                        bodyA.Position, bodyA.Radius,
                        bodyB.Position, bodyB.GetTransformedVertices(),
                        out normal, out depth);
                }
                else if (shapeTypeB is ShapeType.Sphere)
                {
                    return Collisions.IntersectCircles(
                        bodyA.Position, bodyA.Radius,
                        bodyB.Position, bodyB.Radius,
                        out normal, out depth);
                }
            }

            return false;
        }
    }
}
