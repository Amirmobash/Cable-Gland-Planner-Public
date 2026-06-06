namespace CableGlandPlanner.Core.Models;
public sealed class DiskParameters {
    public double Radius { get; set; }
    public double Diameter => Radius * 2.0;
    public double EdgeClearance { get; set; }
    public string Material { get; set; } = "Aluminium";
    public double MaxDepth { get; set; } = 35.0;
}
