using System;

namespace PhysX
{
    public static class Collisions
    {
        public static bool IntersectCirclePolygon(PhysXVector circleCenter, float circleRadius,
                                                    PhysXVector polygonCenter, PhysXVector[] vertices,
                                                    out PhysXVector normal, out float depth)
        {
            normal = PhysXVector.Zero;
            depth = float.MaxValue;

            PhysXVector axis = PhysXVector.Zero;
            float axisDepth = 0f;
            float minA, maxA, minB, maxB;

            for (int i = 0; i < vertices.Length; i++)
            {
                PhysXVector va = vertices[i];
                PhysXVector vb = vertices[(i + 1) % vertices.Length];

                PhysXVector edge = vb - va;
                axis = new PhysXVector(-edge.Y, edge.X, edge.Z);
                axis = PhysXMath.Normalize(axis);

                Collisions.ProjectVertices(vertices, axis, out minA, out maxA);
                Collisions.ProjectCircle(circleCenter, circleRadius, axis, out minB, out maxB);

                if (minA >= maxB || minB >= maxA)
                {
                    return false;
                }

                axisDepth = MathF.Min(maxB - minA, maxA - minB);

                if (axisDepth < depth)
                {
                    depth = axisDepth;
                    normal = axis;
                }
            }

            int cpIndex = Collisions.FindClosestPointOnPolygon(circleCenter, vertices);
            PhysXVector cp = vertices[cpIndex];

            axis = cp - circleCenter;
            axis = PhysXMath.Normalize(axis);

            Collisions.ProjectVertices(vertices, axis, out minA, out maxA);
            Collisions.ProjectCircle(circleCenter, circleRadius, axis, out minB, out maxB);

            if (minA >= maxB || minB >= maxA)
            {
                return false;
            }

            axisDepth = MathF.Min(maxB - minA, maxA - minB);

            if (axisDepth < depth)
            {
                depth = axisDepth;
                normal = axis;
            }

            PhysXVector direction = polygonCenter - circleCenter;

            if (PhysXMath.Dot(direction, normal) < 0f)
            {
                normal = -normal;
            }

            return true;
        }


        public static bool IntersectCirclePolygon(PhysXVector circleCenter, float circleRadius, 
            PhysXVector[] vertices, 
            out PhysXVector normal, out float depth)
        {
            normal = PhysXVector.Zero;
            depth = float.MaxValue;

            PhysXVector axis = PhysXVector.Zero;
            float axisDepth = 0f;
            float minA, maxA, minB, maxB;

            for (int i = 0; i < vertices.Length; i++)
            {
                PhysXVector va = vertices[i];
                PhysXVector vb = vertices[(i + 1) % vertices.Length];

                PhysXVector edge = vb - va;
                axis = new PhysXVector(-edge.Y, edge.X, edge.Z);
                axis = PhysXMath.Normalize(axis);

                Collisions.ProjectVertices(vertices, axis, out minA, out maxA);
                Collisions.ProjectCircle(circleCenter, circleRadius, axis, out minB, out maxB);

                if (minA >= maxB || minB >= maxA)
                {
                    return false;
                }

                axisDepth = MathF.Min(maxB - minA, maxA - minB);

                if (axisDepth < depth)
                {
                    depth = axisDepth;
                    normal = axis;
                }
            }

            int cpIndex = Collisions.FindClosestPointOnPolygon(circleCenter, vertices);
            PhysXVector cp = vertices[cpIndex];

            axis = cp - circleCenter;
            axis = PhysXMath.Normalize(axis);

            Collisions.ProjectVertices(vertices, axis, out minA, out maxA);
            Collisions.ProjectCircle(circleCenter, circleRadius, axis, out minB, out maxB);

            if (minA >= maxB || minB >= maxA)
            {
                return false;
            }

            axisDepth = MathF.Min(maxB - minA, maxA - minB);

            if (axisDepth < depth)
            {
                depth = axisDepth;
                normal = axis;
            }

            PhysXVector polygonCenter = Collisions.FindArithmeticMean(vertices);

            PhysXVector direction = polygonCenter - circleCenter;

            if (PhysXMath.Dot(direction, normal) < 0f)
            {
                normal = -normal;
            }

            return true;
        }

        private static int FindClosestPointOnPolygon(PhysXVector circleCenter, PhysXVector[] vertices)
        {
            int result = -1;
            float minDistance = float.MaxValue;

            for(int i = 0; i < vertices.Length; i++)
            {
                PhysXVector v = vertices[i];
                float distance = PhysXMath.Distance(v, circleCenter);

                if(distance < minDistance)
                {
                    minDistance = distance;
                    result = i;
                }
            }

            return result;
        }

        private static void ProjectCircle(PhysXVector center, float radius, PhysXVector axis, out float min, out float max)
        {
            PhysXVector direction = PhysXMath.Normalize(axis);
            PhysXVector directionAndRadius = direction * radius;

            PhysXVector p1 = center + directionAndRadius;
            PhysXVector p2 = center - directionAndRadius;

            min = PhysXMath.Dot(p1, axis);
            max = PhysXMath.Dot(p2, axis);

            if(min > max)
            {
                // swap the min and max values.
                float t = min;
                min = max;
                max = t;
            }
        }

        public static bool IntersectPolygons(PhysXVector centerA, PhysXVector[] verticesA, PhysXVector centerB, PhysXVector[] verticesB, out PhysXVector normal, out float depth)
        {
            normal = PhysXVector.Zero;
            depth = float.MaxValue;

            for (int i = 0; i < verticesA.Length; i++)
            {
                PhysXVector va = verticesA[i];
                PhysXVector vb = verticesA[(i + 1) % verticesA.Length];

                PhysXVector edge = vb - va;
                PhysXVector axis = new PhysXVector(-edge.Y, edge.X, edge.Z);
                axis = PhysXMath.Normalize(axis);

                Collisions.ProjectVertices(verticesA, axis, out float minA, out float maxA);
                Collisions.ProjectVertices(verticesB, axis, out float minB, out float maxB);

                if (minA >= maxB || minB >= maxA)
                {
                    return false;
                }

                float axisDepth = MathF.Min(maxB - minA, maxA - minB);

                if (axisDepth < depth)
                {
                    depth = axisDepth;
                    normal = axis;
                }
            }

            for (int i = 0; i < verticesB.Length; i++)
            {
                PhysXVector va = verticesB[i];
                PhysXVector vb = verticesB[(i + 1) % verticesB.Length];

                PhysXVector edge = vb - va;
                PhysXVector axis = new PhysXVector(-edge.Y, edge.X, edge.Z);
                axis = PhysXMath.Normalize(axis);

                Collisions.ProjectVertices(verticesA, axis, out float minA, out float maxA);
                Collisions.ProjectVertices(verticesB, axis, out float minB, out float maxB);

                if (minA >= maxB || minB >= maxA)
                {
                    return false;
                }

                float axisDepth = MathF.Min(maxB - minA, maxA - minB);

                if (axisDepth < depth)
                {
                    depth = axisDepth;
                    normal = axis;
                }
            }

            PhysXVector direction = centerB - centerA;

            if (PhysXMath.Dot(direction, normal) < 0f)
            {
                normal = -normal;
            }

            return true;
        }

        public static bool IntersectPolygons(PhysXVector[] verticesA, PhysXVector[] verticesB, out PhysXVector normal, out float depth)
        {
            normal = PhysXVector.Zero;
            depth = float.MaxValue;

            for(int i = 0; i < verticesA.Length; i++)
            {
                PhysXVector va = verticesA[i];
                PhysXVector vb = verticesA[(i + 1) % verticesA.Length];

                PhysXVector edge = vb - va;
                PhysXVector axis = new PhysXVector(-edge.Y, edge.X, edge.Z);
                axis = PhysXMath.Normalize(axis);

                Collisions.ProjectVertices(verticesA, axis, out float minA, out float maxA);
                Collisions.ProjectVertices(verticesB, axis, out float minB, out float maxB);

                if(minA >= maxB || minB >= maxA)
                {
                    return false;
                }

                float axisDepth = MathF.Min(maxB - minA, maxA - minB);

                if(axisDepth < depth)
                {
                    depth = axisDepth;
                    normal = axis;
                }
            }

            for (int i = 0; i < verticesB.Length; i++)
            {
                PhysXVector va = verticesB[i];
                PhysXVector vb = verticesB[(i + 1) % verticesB.Length];

                PhysXVector edge = vb - va;
                PhysXVector axis = new PhysXVector(-edge.Y, edge.X, edge.Z);
                axis = PhysXMath.Normalize(axis);

                Collisions.ProjectVertices(verticesA, axis, out float minA, out float maxA);
                Collisions.ProjectVertices(verticesB, axis, out float minB, out float maxB);

                if (minA >= maxB || minB >= maxA)
                {
                    return false;
                }

                float axisDepth = MathF.Min(maxB - minA, maxA - minB);

                if (axisDepth < depth)
                {
                    depth = axisDepth;
                    normal = axis;
                }
            }

            PhysXVector centerA = Collisions.FindArithmeticMean(verticesA);
            PhysXVector centerB = Collisions.FindArithmeticMean(verticesB);

            PhysXVector direction = centerB - centerA;

            if(PhysXMath.Dot(direction, normal) < 0f)
            {
                normal = -normal;
            }

            return true;
        }

        private static PhysXVector FindArithmeticMean(PhysXVector[] vertices)
        {
            float sumX = 0f;
            float sumY = 0f;
            float sumZ = 0f;

            for(int i = 0; i < vertices.Length; i++)
            {
                PhysXVector v = vertices[i];
                sumX += v.X;
                sumY += v.Y;
                sumZ += v.Z;
            }

            return new PhysXVector(sumX / (float)vertices.Length, sumY / (float)vertices.Length, sumZ/(float)vertices.Length);
        }

        private static void ProjectVertices(PhysXVector[] vertices, PhysXVector axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;

            for(int i = 0; i < vertices.Length; i++)
            {
                PhysXVector v = vertices[i];
                float proj = PhysXMath.Dot(v, axis);

                if(proj < min) { min = proj; }
                if(proj > max) { max = proj; }
            }
        }

        public static bool IntersectCircles(
            PhysXVector centerA, float radiusA, 
            PhysXVector centerB, float radiusB, 
            out PhysXVector normal, out float depth)
        {
            normal = PhysXVector.Zero;
            depth = 0f;

            float distance = PhysXMath.Distance(centerA, centerB);
            float radii = radiusA + radiusB;

            if(distance >= radii)
            {
                return false;
            }

            normal = PhysXMath.Normalize(centerB - centerA);
            depth = radii - distance;

            return true;
        }

    }
}
