namespace CableGlandPlanner.Enterprise.Export.Module05;

public sealed class Component06
{
    public string Name => "Export-Module05-Component06";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 5.6 + 6, 3);
}
