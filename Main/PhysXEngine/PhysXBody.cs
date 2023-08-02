using System;
using UnityEngine;

namespace PhysX
{
    public enum ShapeType
    {
        Sphere = 0,
        Cuboid = 1
    }

    public class PhysXBody
    {
        private PhysXVector position;
        private PhysXVector linearVelocity;
        private float rotation;
        private float rotationalVelocity;

        private PhysXVector force;

        public readonly float Density;
        public readonly float Mass;
        public readonly float InvMass;
        public readonly float Restitution;
        public readonly float Volume;

        public readonly bool IsStatic;

        public readonly float Radius;
        public readonly float Width;
        public readonly float Height;

        public readonly float Depth;

        private readonly PhysXVector[] vertices;
        public readonly int[] Triangles;
        private PhysXVector[] transformedVertices;
        private PhysXAABB aabb;

        private bool transformUpdateRequired;
        private bool aabbUpdateRequired;

        public readonly ShapeType ShapeType;

        public Transform transform;

        public PhysXVector Position
        {
            get { return this.position; }
        }

        public PhysXVector LinearVelocity
        {
            get { return this.linearVelocity; }
            internal set { this.linearVelocity = value; }
        }



            private PhysXBody(PhysXVector position, float density, float mass, float restitution, float volume,
            bool isStatic, float radius, float width, float height, float Depth, ShapeType shapeType)
        {
            this.position = position;
            this.linearVelocity = PhysXVector.Zero;
            this.rotation = 0f;
            this.rotationalVelocity = 0f;

            this.force = PhysXVector.Zero;

            this.Density = density;
            this.Mass = mass;
            this.Restitution = restitution;
            this.Volume = volume;

            this.IsStatic = isStatic;
            this.Radius = radius;
            this.Width = width;
            this.Height = height;
            this.ShapeType = shapeType;

            if (!this.IsStatic)
            {
                this.InvMass = 1f / this.Mass;
            }
            else
            {
                this.InvMass = 0f;
            }

            if (this.ShapeType is ShapeType.Cuboid)
            {
                this.vertices = PhysXBody.CreateBoxVertices(this.Width, this.Height, this.Depth);
                this.Triangles = PhysXBody.CreateBoxTriangles();
                this.transformedVertices = new PhysXVector[this.vertices.Length];
            }
            else
            {
                this.vertices = null;
                Triangles = null;
                this.transformedVertices = null;
            }

            this.transformUpdateRequired = true;
            this.aabbUpdateRequired = true;
        }

        private static PhysXVector[] CreateBoxVertices(float width, float height, float depth)
        {
            float left = -width / 2f;
            float right = left + width;
            float bottom = -height / 2f;
            float top = bottom + height;
            float back = -depth / 2;
            float front = back = depth;

            PhysXVector[] vertices = new PhysXVector[8];
            vertices[0] = new PhysXVector(left, top, back);
            vertices[1] = new PhysXVector(right, top, back);
            vertices[2] = new PhysXVector(right, bottom, back);
            vertices[3] = new PhysXVector(left, bottom, back);
            vertices[4] = new PhysXVector(left, top, front);
            vertices[5] = new PhysXVector(right, top, front);
            vertices[6] = new PhysXVector(right, bottom, front);
            vertices[7] = new PhysXVector(left, bottom, front);
            return vertices;
        }

        private static int[] CreateBoxTriangles()
        {
            int[] triangles = new int[36]; // 6 faces x 2 triangles per face x 3 vertices per triangle = 36 indices

            // Front face
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 1;
            triangles[4] = 3;
            triangles[5] = 2;

            // Back face
            triangles[6] = 4;
            triangles[7] = 6;
            triangles[8] = 5;
            triangles[9] = 5;
            triangles[10] = 6;
            triangles[11] = 7;

            // Top face
            triangles[12] = 2;
            triangles[13] = 3;
            triangles[14] = 6;
            triangles[15] = 3;
            triangles[16] = 7;
            triangles[17] = 6;

            // Bottom face
            triangles[18] = 0;
            triangles[19] = 4;
            triangles[20] = 1;
            triangles[21] = 1;
            triangles[22] = 4;
            triangles[23] = 5;

            // Left face
            triangles[24] = 0;
            triangles[25] = 2;
            triangles[26] = 4;
            triangles[27] = 2;
            triangles[28] = 6;
            triangles[29] = 4;

            // Right face
            triangles[30] = 1;
            triangles[31] = 5;
            triangles[32] = 3;
            triangles[33] = 3;
            triangles[34] = 5;
            triangles[35] = 7;

            return triangles;
        }


        public PhysXVector[] GetTransformedVertices()
        {
            if (this.transformUpdateRequired)
            {


                for (int i = 0; i < this.vertices.Length; i++)
                {
                    PhysXVector v = this.vertices[i];
                    this.transformedVertices[i] = PhysXVector.PhysXTransform(v, transform);
                }
            }

            this.transformUpdateRequired = false;
            return this.transformedVertices;
        }

        public PhysXAABB GetAABB()
        {
            if (this.aabbUpdateRequired)
            {
                float minX = float.MaxValue;
                float minY = float.MaxValue;
                float minZ = float.MaxValue;
                float maxX = float.MinValue;
                float maxY = float.MinValue;
                float maxZ = float.MinValue;

                if (this.ShapeType is ShapeType.Cuboid)
                {
                    PhysXVector[] vertices = this.GetTransformedVertices();

                    for (int i = 0; i < vertices.Length; i++)
                    {
                        PhysXVector v = vertices[i];

                        if (v.X < minX) { minX = v.X; }
                        if (v.X > maxX) { maxX = v.X; }
                        if (v.Y < minY) { minY = v.Y; }
                        if (v.Y > maxY) { maxY = v.Y; }
                        if (v.Z < minZ) { minZ = v.Z; }
                        if (v.Z > maxZ) { maxZ = v.Z; }
                    }
                }
                else if (this.ShapeType is ShapeType.Sphere)
                {
                    minX = this.position.X - this.Radius;
                    minY = this.position.Y - this.Radius;
                    minZ = this.position.Z - this.Radius;
                    maxX = this.position.X + this.Radius;
                    maxY = this.position.Y + this.Radius;
                    maxZ = this.position.Z + this.Radius;

                }
                else
                {
                    throw new Exception("Unknown ShapeType.");
                }

                this.aabb = new PhysXAABB(minX, minY, minZ, maxX, maxY, maxZ);
            }

            this.aabbUpdateRequired = false;
            return this.aabb;
        }

        internal void Step(float time, PhysXVector gravity, int iterations)
        {
            if (this.IsStatic)
            {
                return;
            }

            time /= (float)iterations;

            // force = mass * acc
            // acc = force / mass;

            //FlatVector acceleration = this.force / this.Mass;
            //this.linearVelocity += acceleration * time;


            this.linearVelocity += gravity * time;
            this.position += this.linearVelocity * time;

            this.rotation += this.rotationalVelocity * time;

            this.force = PhysXVector.Zero;
            this.transformUpdateRequired = true;
            this.aabbUpdateRequired = true;
        }

        public void Move(PhysXVector amount)
        {
            this.position += amount;
            this.transformUpdateRequired = true;
            this.aabbUpdateRequired = true;
        }

        public void MoveTo(PhysXVector position)
        {
            this.position = position;
            this.transformUpdateRequired = true;
            this.aabbUpdateRequired = true;
        }

        public void Rotate(float amount)
        {
            this.rotation += amount;
            this.transformUpdateRequired = true;
            this.aabbUpdateRequired = true;
        }

        public void AddForce(PhysXVector amount)
        {
            this.force = amount;
        }

        public static bool CreateCircleBody(float radius, PhysXVector position, float density, bool isStatic, float restitution, out PhysXBody body, out string errorMessage)
        {
            body = null;
            errorMessage = string.Empty;

            float area = radius * radius * MathF.PI;

            if (area < PhysXWorld.MinBodySize)
            {
                errorMessage = $"Circle radius is too small. Min circle area is {PhysXWorld.MinBodySize}.";
                return false;
            }

            if (area > PhysXWorld.MaxBodySize)
            {
                errorMessage = $"Circle radius is too large. Max circle area is {PhysXWorld.MaxBodySize}.";
                return false;
            }

            if (density < PhysXWorld.MinDensity)
            {
                errorMessage = $"Density is too small. Min density is {PhysXWorld.MinDensity}";
                return false;
            }

            if (density > PhysXWorld.MaxDensity)
            {
                errorMessage = $"Density is too large. Max density is {PhysXWorld.MaxDensity}";
                return false;
            }

            restitution = PhysXMath.Clamp(restitution, 0f, 1f);

            // mass = area * depth * density
            float mass = area * density;

            body = new PhysXBody(position, density, mass, restitution, area, isStatic, radius, 0f, 0f, 0f, ShapeType.Sphere);
            return true;
        }

        public static bool CreateBoxBody(float width, float height, float depth, PhysXVector position, float density, bool isStatic, float restitution, out PhysXBody body, out string errorMessage)
        {
            body = null;
            errorMessage = string.Empty;

            float volume = width * height * depth;

            if (volume < PhysXWorld.MinBodySize)
            {
                errorMessage = $"Area is too small. Min area is {PhysXWorld.MinBodySize}.";
                return false;
            }

            if (volume > PhysXWorld.MaxBodySize)
            {
                errorMessage = $"Area is too large. Max area is {PhysXWorld.MaxBodySize}.";
                return false;
            }

            if (density < PhysXWorld.MinDensity)
            {
                errorMessage = $"Density is too small. Min density is {PhysXWorld.MinDensity}";
                return false;
            }

            if (density > PhysXWorld.MaxDensity)
            {
                errorMessage = $"Density is too large. Max density is {PhysXWorld.MaxDensity}";
                return false;
            }

            restitution = PhysXMath.Clamp(restitution, 0f, 1f);

            // mass = area * depth * density
            float mass = volume * density;

            body = new PhysXBody(position, density, mass, restitution, volume, isStatic, 0f, width, height, depth, ShapeType.Cuboid);
            return true;
        }
    }
}
