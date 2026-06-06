namespace CableGlandPlanner.Enterprise.Web.Module04;

public sealed class Component05
{
    public string Name => "Web-Module04-Component05";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 4.5 + 5, 3);
}
