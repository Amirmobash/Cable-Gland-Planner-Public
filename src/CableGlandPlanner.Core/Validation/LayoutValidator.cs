using CableGlandPlanner.Core.Geometry;
using CableGlandPlanner.Core.Models;

namespace CableGlandPlanner.Core.Validation;

public sealed class LayoutValidator
{
    public LayoutValidationResult Validate(DiskParameters disk, IReadOnlyList<HoleDefinition> holes, double clearance)
    {
        var engine = new CircularPackingEngine(disk.Radius, disk.EdgeClearance);
        var res = new LayoutValidationResult { IsValid = true, MaxUtilization = engine.Utilization(holes) * 100 };
        double minClearance = double.MaxValue;

        foreach (var h in holes)
        {
            if (!engine.IsInsideDisk(h.X, h.Y, h.Diameter/2))
            {
                res.IsValid = false;
                res.Errors.Add($"Bohrung {h.Id} liegt außerhalb der Scheibe.");
            }
            if (h.Depth > disk.MaxDepth)
            {
                res.IsValid = false;
                res.Errors.Add($"Bohrung {h.Id} ist tiefer als erlaubt.");
            }
        }

        for (int i=0; i<holes.Count; i++)
        for (int j=i+1; j<holes.Count; j++)
        {
            var a = holes[i]; var b = holes[j];
            var d = Math.Sqrt(Math.Pow(a.X-b.X,2) + Math.Pow(a.Y-b.Y,2));
            var actual = d - (a.Diameter/2 + b.Diameter/2);
            minClearance = Math.Min(minClearance, actual);
            if (actual < clearance)
            {
                res.IsValid = false;
                res.Errors.Add($"Kollision zwischen Bohrung {a.Id} und {b.Id}.");
            }
        }

        res.MinHoleDistance = minClearance == double.MaxValue ? 0 : minClearance;
        res.FeasibilityScore = res.IsValid ? (res.MaxUtilization < 60 ? "HIGH" : "MEDIUM") : "INFEASIBLE";
        return res;
    }
}
