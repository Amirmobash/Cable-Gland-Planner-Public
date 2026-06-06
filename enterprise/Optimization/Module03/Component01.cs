namespace CableGlandPlanner.Enterprise.Optimization.Module03;

public sealed class Component01
{
    public string Name => "Optimization-Module03-Component01";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 3.1 + 1, 3);
}
