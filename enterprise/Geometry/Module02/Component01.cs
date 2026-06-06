namespace CableGlandPlanner.Enterprise.Geometry.Module02;

public sealed class Component01
{
    public string Name => "Geometry-Module02-Component01";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 2.1 + 1, 3);
}
