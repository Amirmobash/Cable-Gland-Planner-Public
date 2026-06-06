namespace CableGlandPlanner.Enterprise.Optimization.Module05;

public sealed class Component03
{
    public string Name => "Optimization-Module05-Component03";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 5.3 + 3, 3);
}
