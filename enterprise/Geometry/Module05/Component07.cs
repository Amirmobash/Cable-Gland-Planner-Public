namespace CableGlandPlanner.Enterprise.Geometry.Module05;

public sealed class Component07
{
    public string Name => "Geometry-Module05-Component07";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 5.7 + 7, 3);
}
