using System;

namespace PhysX
{
    public static class PhysXMath
    {
        public static float Clamp(float value, float min, float max)
        {
            if (min == max)
            {
                return min;
            }

            if (min > max)
            {
                throw new ArgumentOutOfRangeException("min is greater than the max.");
            }

            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (min == max)
            {
                return min;
            }

            if (min > max)
            {
                throw new ArgumentOutOfRangeException("min is greater than the max.");
            }

            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }


        public static float Length(PhysXVector v)
        {
            return MathF.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        }

        public static float Distance(PhysXVector a, PhysXVector b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            float dz = a.Z - b.Z;
            return MathF.Sqrt(dx * dx + dy * dy + dz + dz);
        }

        public static PhysXVector Normalize(PhysXVector v)
        {
            float len = PhysXMath.Length(v);
            return new PhysXVector(v.X / len, v.Y / len, v.Z / len);
        }

        public static float Dot(PhysXVector a, PhysXVector b)
        {
            // a · b = ax * bx + ay * by
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static PhysXVector Cross(PhysXVector a, PhysXVector b)
        {
            // cz = ax * by − ay * bx
            return new PhysXVector(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z,  a.X * b.Y - a.Y * b.X);
        }

    }
}
