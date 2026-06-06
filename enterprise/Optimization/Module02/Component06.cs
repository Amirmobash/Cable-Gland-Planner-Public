namespace CableGlandPlanner.Enterprise.Optimization.Module02;

public sealed class Component06
{
    public string Name => "Optimization-Module02-Component06";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 2.6 + 6, 3);
}
