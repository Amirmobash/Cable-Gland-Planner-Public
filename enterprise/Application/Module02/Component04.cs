namespace CableGlandPlanner.Enterprise.Application.Module02;

public sealed class Component04
{
    public string Name => "Application-Module02-Component04";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 2.4 + 4, 3);
}
