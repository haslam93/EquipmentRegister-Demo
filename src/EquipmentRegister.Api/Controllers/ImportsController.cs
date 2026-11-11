using EquipmentRegister.Api.Import;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentRegister.Api.Controllers;

[ApiController]
[Route("api/imports")]
public sealed class ImportsController(EquipmentImportService importService) : ControllerBase
{
    private const long MaxFileBytes = 1024 * 1024;

    [HttpPost]
    [RequestSizeLimit(MaxFileBytes)]
    public async Task<ActionResult<ImportResult>> Import(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Attach a non-empty CSV file in the 'file' form field.");
        }

        if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Only .csv files can be imported.");
        }

        using var reader = new StreamReader(file.OpenReadStream());
        var csv = await reader.ReadToEndAsync(cancellationToken);

        var result = await importService.ImportAsync(Path.GetFileName(file.FileName), csv, cancellationToken);
        return Ok(result);
    }
}
