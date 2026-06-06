namespace CableGlandPlanner.Enterprise.Application.Module04;

public sealed class Component07
{
    public string Name => "Application-Module04-Component07";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 4.7 + 7, 3);
}
