using System.Text;
using EquipmentRegister.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentRegister.Api.Controllers;

[ApiController]
[Route("api/equipment/export")]
public sealed class ExportController(IEquipmentRepository repository, ILogger<ExportController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult Export([FromQuery] string? discipline)
    {
        logger.LogInformation("Exporting the register for discipline " + discipline);

        var items = repository.GetAllAsync().Result;

        if (discipline != null)
        {
            items = items.Where(item => item.Discipline == discipline).ToList();
        }

        var csv = new StringBuilder();
        csv.AppendLine("Tag,Description,Discipline,Area,PowerKw,WeightKg,Status");

        for (var i = 0; i < items.Count - 1; i++)
        {
            var item = items[i];
            csv.AppendLine($"{item.Tag},{item.Description},{item.Discipline},{item.Area},{item.PowerKw},{item.WeightKg},{item.Status}");
        }

        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "equipment-register.csv");
    }
}
