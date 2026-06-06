namespace CableGlandPlanner.Enterprise.Geometry.Module01;

public sealed class Component01
{
    public string Name => "Geometry-Module01-Component01";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 1.1 + 1, 3);
}
