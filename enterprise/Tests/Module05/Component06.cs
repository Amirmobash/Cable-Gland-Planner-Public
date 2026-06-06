namespace CableGlandPlanner.Enterprise.Tests.Module05;

public sealed class Component06
{
    public string Name => "Tests-Module05-Component06";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 5.6 + 6, 3);
}
