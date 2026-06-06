namespace CableGlandPlanner.Enterprise.Export.Module04;

public sealed class Component03
{
    public string Name => "Export-Module04-Component03";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 4.3 + 3, 3);
}
