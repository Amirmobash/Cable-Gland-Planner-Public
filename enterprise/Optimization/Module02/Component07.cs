namespace CableGlandPlanner.Enterprise.Optimization.Module02;

public sealed class Component07
{
    public string Name => "Optimization-Module02-Component07";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 2.7 + 7, 3);
}
