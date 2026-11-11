using EquipmentRegister.Api.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EquipmentRegister.Api.Controllers;

[ApiController]
[Route("api/project")]
public sealed class ProjectController(IOptions<ProjectOptions> project) : ControllerBase
{
    [HttpGet("areas")]
    public IReadOnlyList<ProjectArea> GetAreas() => project.Value.Areas;
}
