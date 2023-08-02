using System;

namespace PhysX
{
    public readonly struct PhysXAABB
    {
        public readonly PhysXVector Min;
        public readonly PhysXVector Max;

        public PhysXAABB(PhysXVector min, PhysXVector max)
        {
            this.Min = min;
            this.Max = max;
        }

        public PhysXAABB(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            this.Min = new PhysXVector(minX, minY, minZ);
            this.Max = new PhysXVector(maxX, maxY, maxZ);
        }
    }
}
