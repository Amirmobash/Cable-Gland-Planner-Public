namespace CableGlandPlanner.Enterprise.Domain.Module05;

public sealed class Component05
{
    public string Name => "Domain-Module05-Component05";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 5.5 + 5, 3);
}
