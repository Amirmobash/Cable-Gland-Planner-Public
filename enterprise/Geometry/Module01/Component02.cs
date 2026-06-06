namespace CableGlandPlanner.Enterprise.Geometry.Module01;

public sealed class Component02
{
    public string Name => "Geometry-Module01-Component02";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 1.2 + 2, 3);
}
