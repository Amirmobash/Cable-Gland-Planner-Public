namespace CableGlandPlanner.Enterprise.Tests.Module05;

public sealed class Component02
{
    public string Name => "Tests-Module05-Component02";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 5.2 + 2, 3);
}
