namespace CableGlandPlanner.Core.Models;
public sealed class HoleDefinition {
    public int Id { get; set; }
    public double Diameter { get; set; }
    public double Depth { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public string InsertType { get; set; } = "M20";
    public string PosMapping { get; set; } = "POS_01";
    public bool IsPocket { get; set; }
}
