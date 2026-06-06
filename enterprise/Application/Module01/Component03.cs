namespace CableGlandPlanner.Enterprise.Application.Module01;

public sealed class Component03
{
    public string Name => "Application-Module01-Component03";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 1.3 + 3, 3);
}
