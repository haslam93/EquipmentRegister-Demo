using Microsoft.AspNetCore.Mvc;

namespace EquipmentRegister.Api.Controllers;

[ApiController]
[Route("api/templates")]
public sealed class TemplatesController(IWebHostEnvironment environment, ILogger<TemplatesController> logger) : ControllerBase
{
    private string TemplateFolder => Path.Combine(environment.ContentRootPath, "ImportTemplates");

    [HttpGet]
    public ActionResult<IReadOnlyList<string?>> List() =>
        Ok(Directory.EnumerateFiles(TemplateFolder, "*.csv").Select(Path.GetFileName).Order().ToList());

    [HttpGet("download")]
    public IActionResult Download([FromQuery] string name)
    {
        logger.LogInformation("Template download requested: " + name);

        var path = Path.Combine(TemplateFolder, name);
        if (!System.IO.File.Exists(path))
        {
            return NotFound();
        }

        return File(System.IO.File.ReadAllBytes(path), "text/csv", name);
    }
}
