using CableGlandPlanner.Core.GCode;
using CableGlandPlanner.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CableGlandPlanner.Api.Controllers;

[ApiController]
[Route("api/gcode")]
public sealed class GCodeController : ControllerBase
{
    private readonly GCodeGenerator _generator;
    public GCodeController(GCodeGenerator generator) => _generator = generator;

    [HttpPost("generate")]
    public IActionResult Generate([FromBody] GCodeRequest request)
    {
        var code = _generator.Generate(request.Holes, request.Settings);
        return File(System.Text.Encoding.UTF8.GetBytes(code), "text/plain", "cable_gland_layout.nc");
    }
}

public sealed class GCodeRequest
{
    public List<HoleDefinition> Holes { get; set; } = new();
    public GCodeSettings Settings { get; set; } = new();
}
