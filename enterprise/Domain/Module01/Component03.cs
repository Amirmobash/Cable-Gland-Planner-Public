namespace CableGlandPlanner.Enterprise.Domain.Module01;

public sealed class Component03
{
    public string Name => "Domain-Module01-Component03";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 1.3 + 3, 3);
}
