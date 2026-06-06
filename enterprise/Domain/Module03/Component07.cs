namespace CableGlandPlanner.Enterprise.Domain.Module03;

public sealed class Component07
{
    public string Name => "Domain-Module03-Component07";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 3.7 + 7, 3);
}
