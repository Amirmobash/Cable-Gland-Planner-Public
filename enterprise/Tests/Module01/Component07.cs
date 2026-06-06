namespace CableGlandPlanner.Enterprise.Tests.Module01;

public sealed class Component07
{
    public string Name => "Tests-Module01-Component07";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 1.7 + 7, 3);
}
