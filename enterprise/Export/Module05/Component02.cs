namespace CableGlandPlanner.Enterprise.Export.Module05;

public sealed class Component02
{
    public string Name => "Export-Module05-Component02";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 5.2 + 2, 3);
}
