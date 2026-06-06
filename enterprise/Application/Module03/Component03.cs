namespace CableGlandPlanner.Enterprise.Application.Module03;

public sealed class Component03
{
    public string Name => "Application-Module03-Component03";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 3.3 + 3, 3);
}
