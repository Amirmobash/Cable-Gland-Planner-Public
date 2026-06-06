using CableGlandPlanner.Core.Models;

namespace CableGlandPlanner.Core.Optimization;

public sealed class LayoutOptimizer
{
    public IReadOnlyList<HoleDefinition> GenerateRadialLayout(
        DiskParameters disk,
        IReadOnlyList<HoleDefinition> holes,
        double clearance)
    {
        if (holes.Count == 0) return holes;
        var sorted = holes.OrderByDescending(h => h.Diameter).Select((h,i) => (h,i)).ToList();
        var result = holes.Select(h => new HoleDefinition {
            Id=h.Id, Diameter=h.Diameter, Depth=h.Depth, InsertType=h.InsertType,
            PosMapping=h.PosMapping, IsPocket=h.IsPocket
        }).ToList();

        for (var k = 0; k < sorted.Count; k++)
        {
            var (h, originalIndex) = sorted[k];
            var maxR = disk.Radius - disk.EdgeClearance - h.Diameter/2;
            var ring = holes.Count == 1 ? 0 : maxR * (0.35 + 0.55 * (k / Math.Max(1.0, holes.Count-1)));
            var angle = 2 * Math.PI * k / holes.Count;
            result[originalIndex].X = Math.Round(ring * Math.Cos(angle), 3);
            result[originalIndex].Y = Math.Round(ring * Math.Sin(angle), 3);
        }
        return result;
    }
}
