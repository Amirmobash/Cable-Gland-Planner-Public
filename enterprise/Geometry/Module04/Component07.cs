namespace CableGlandPlanner.Enterprise.Geometry.Module04;

public sealed class Component07
{
    public string Name => "Geometry-Module04-Component07";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 4.7 + 7, 3);
}
