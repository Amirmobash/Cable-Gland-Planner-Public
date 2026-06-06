namespace CableGlandPlanner.Enterprise.Optimization.Module05;

public sealed class Component06
{
    public string Name => "Optimization-Module05-Component06";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 5.6 + 6, 3);
}
