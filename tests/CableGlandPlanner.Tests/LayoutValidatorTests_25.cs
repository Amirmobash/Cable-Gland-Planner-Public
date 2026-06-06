using CableGlandPlanner.Core.Models;
using CableGlandPlanner.Core.Validation;
using Xunit;

public class LayoutValidatorTests_25
{
    [Fact]
    public void Layout_25_Should_Be_Validated()
    {
        var validator = new LayoutValidator();
        var disk = new DiskParameters { Radius = 50, EdgeClearance = 5 };
        var holes = new List<HoleDefinition> { new() { Id=1, Diameter=10, Depth=5, X=0, Y=0 } };
        var result = validator.Validate(disk, holes, 3);
        Assert.True(result.IsValid);
    }
}
