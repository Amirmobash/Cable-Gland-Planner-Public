namespace CableGlandPlanner.Enterprise.Tests.Module04;

public sealed class Component06
{
    public string Name => "Tests-Module04-Component06";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 4.6 + 6, 3);
}
