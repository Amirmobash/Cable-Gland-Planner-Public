namespace CableGlandPlanner.Core.Models;
public sealed class GCodeSettings {
    public double SafeZ { get; set; } = 10.0;
    public double FeedRateXY { get; set; } = 500;
    public double FeedRateZ { get; set; } = 250;
    public int SpindleSpeed { get; set; } = 12000;
    public bool UseCoolant { get; set; } = true;
    public string CoolantType { get; set; } = "MIST";
    public double ToolDiameter { get; set; } = 6.0;
    public double PeckDepth { get; set; } = 2.0;
    public bool UsePecking { get; set; }
}
