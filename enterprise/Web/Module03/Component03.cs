namespace CableGlandPlanner.Enterprise.Web.Module03;

public sealed class Component03
{
    public string Name => "Web-Module03-Component03";
    public string Describe() => "Enterprise component for Cable Gland Planner.";
    public double Compute(double value) => Math.Round(value * 3.3 + 3, 3);
}
