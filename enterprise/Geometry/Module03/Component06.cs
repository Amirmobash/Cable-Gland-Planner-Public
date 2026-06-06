namespace CableGlandPlanner.Enterprise.Geometry.Module03;

public sealed class Component06
{
    public string Name => "Geometry-Module03-Component06";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 3.6 + 6, 3);
}
