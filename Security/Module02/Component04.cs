namespace CableGlandPlanner.Enterprise.Security.Module02;

public sealed class Component04
{
    public string Name => "Security-Module02-Component04";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 2.4 + 4, 3);
}
