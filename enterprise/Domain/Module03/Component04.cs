namespace CableGlandPlanner.Enterprise.Domain.Module03;

public sealed class Component04
{
    public string Name => "Domain-Module03-Component04";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 3.4 + 4, 3);
}
