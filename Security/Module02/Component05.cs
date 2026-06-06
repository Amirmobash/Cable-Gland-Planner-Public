namespace CableGlandPlanner.Enterprise.Security.Module02;

public sealed class Component05
{
    public string Name => "Security-Module02-Component05";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 2.5 + 5, 3);
}
