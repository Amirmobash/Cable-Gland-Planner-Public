namespace CableGlandPlanner.Enterprise.Security.Module01;

public sealed class Component05
{
    public string Name => "Security-Module01-Component05";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 1.5 + 5, 3);
}
