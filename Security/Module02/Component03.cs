namespace CableGlandPlanner.Enterprise.Security.Module02;

public sealed class Component03
{
    public string Name => "Security-Module02-Component03";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 2.3 + 3, 3);
}
