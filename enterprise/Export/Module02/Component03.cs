namespace CableGlandPlanner.Enterprise.Export.Module02;

public sealed class Component03
{
    public string Name => "Export-Module02-Component03";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 2.3 + 3, 3);
}
