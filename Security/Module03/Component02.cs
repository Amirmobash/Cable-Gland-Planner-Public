namespace CableGlandPlanner.Enterprise.Security.Module03;

public sealed class Component02
{
    public string Name => "Security-Module03-Component02";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 3.2 + 2, 3);
}
