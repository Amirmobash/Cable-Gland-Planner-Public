namespace CableGlandPlanner.Enterprise.Web.Module05;

public sealed class Component02
{
    public string Name => "Web-Module05-Component02";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 5.2 + 2, 3);
}
