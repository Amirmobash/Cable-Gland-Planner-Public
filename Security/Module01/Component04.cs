namespace CableGlandPlanner.Enterprise.Security.Module01;

public sealed class Component04
{
    public string Name => "Security-Module01-Component04";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 1.4 + 4, 3);
}
