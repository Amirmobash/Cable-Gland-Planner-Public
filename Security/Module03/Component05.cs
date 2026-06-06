namespace CableGlandPlanner.Enterprise.Security.Module03;

public sealed class Component05
{
    public string Name => "Security-Module03-Component05";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 3.5 + 5, 3);
}
