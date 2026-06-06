namespace CableGlandPlanner.Enterprise.Security.Module05;

public sealed class Component03
{
    public string Name => "Security-Module05-Component03";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 5.3 + 3, 3);
}
