using CableGlandPlanner.Core.Models;

namespace CableGlandPlanner.Core.Geometry;

public sealed class CircularPackingEngine
{
    public double DiskRadius { get; }
    public double EdgeClearance { get; }
    public double EffectiveRadius => DiskRadius - EdgeClearance;

    public CircularPackingEngine(double diskRadius, double edgeClearance)
    {
        DiskRadius = diskRadius;
        EdgeClearance = edgeClearance;
    }

    public bool IsInsideDisk(double x, double y, double holeRadius)
        => Math.Sqrt(x*x + y*y) + holeRadius <= EffectiveRadius;

    public bool AreHolesColliding(HoleDefinition a, HoleDefinition b, double clearance)
    {
        var d = Math.Sqrt(Math.Pow(a.X-b.X,2) + Math.Pow(a.Y-b.Y,2));
        return d < a.Diameter/2 + b.Diameter/2 + clearance;
    }

    public double Utilization(IEnumerable<HoleDefinition> holes)
    {
        var holeArea = holes.Sum(h => Math.PI * Math.Pow(h.Diameter/2,2));
        var diskArea = Math.PI * EffectiveRadius * EffectiveRadius;
        return diskArea <= 0 ? 1 : holeArea / diskArea;
    }
}
