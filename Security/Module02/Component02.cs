namespace CableGlandPlanner.Enterprise.Security.Module02;

public sealed class Component02
{
    public string Name => "Security-Module02-Component02";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 2.2 + 2, 3);
}
