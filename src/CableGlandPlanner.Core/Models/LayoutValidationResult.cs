namespace CableGlandPlanner.Core.Models;
public sealed class LayoutValidationResult {
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public double MinHoleDistance { get; set; }
    public double MaxUtilization { get; set; }
    public string FeasibilityScore { get; set; } = "HIGH";
}
