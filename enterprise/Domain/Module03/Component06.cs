namespace CableGlandPlanner.Enterprise.Domain.Module03;

public sealed class Component06
{
    public string Name => "Domain-Module03-Component06";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 3.6 + 6, 3);
}
