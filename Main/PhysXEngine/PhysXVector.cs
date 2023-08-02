using System;
using UnityEngine;

namespace PhysX
{
    public readonly struct PhysXVector
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        public static readonly PhysXVector Zero = new PhysXVector(0f, 0f, 0f);

        public PhysXVector(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static PhysXVector operator +(PhysXVector a, PhysXVector b)
        {
            return new PhysXVector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static PhysXVector operator -(PhysXVector a, PhysXVector b)
        {
            return new PhysXVector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static PhysXVector operator -(PhysXVector v)
        {
            return new PhysXVector(-v.X, -v.Y, -v.Z);
        }

        public static PhysXVector operator *(PhysXVector v, float s)
        {
            return new PhysXVector(v.X * s, v.Y * s, v.Z * s);
        }

        public static PhysXVector operator *(float s, PhysXVector v)
        {
            return new PhysXVector(v.X * s, v.Y * s, v.Z * s);
        }

        public static PhysXVector operator /(PhysXVector v, float s)
        {
            return new PhysXVector(v.X / s, v.Y / s, v.Z / s);
        }

        internal static PhysXVector PhysXTransform(PhysXVector v, Transform transform)
        {
            return new PhysXVector(
                Mathf.Cos(transform.eulerAngles.x) * v.X - Mathf.Sin(transform.eulerAngles.y) * v.Y + Mathf.Sin(transform.eulerAngles.z) + transform.position.x,
                Mathf.Sin(transform.eulerAngles.x) * v.X + Mathf.Cos(transform.eulerAngles.y) * v.Y - Mathf.Sin(transform.eulerAngles.z) + transform.position.y,
                -Mathf.Sin(transform.eulerAngles.x) * v.X + Mathf.Cos(transform.eulerAngles.y) * v.Y + Mathf.Sin(transform.eulerAngles.z) + transform.position.z
                );
        }

        public bool Equals(PhysXVector other)
        {
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            if (obj is PhysXVector other)
            {
                return this.Equals(other);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return new { this.X, this.Y, this.Z }.GetHashCode();
        }

        public override string ToString()
        {
            return $"X: {this.X}, Y: {this.Y}, Z: {this.Z}";
        }
    }
}
