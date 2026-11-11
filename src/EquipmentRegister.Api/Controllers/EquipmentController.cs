using EquipmentRegister.Api.Data;
using EquipmentRegister.Api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentRegister.Api.Controllers;

[ApiController]
[Route("api/equipment")]
public sealed class EquipmentController(IEquipmentRepository repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EquipmentItem>>> List(
        [FromQuery] string? q,
        [FromQuery] string? discipline,
        CancellationToken cancellationToken)
    {
        IEnumerable<EquipmentItem> items = await repository.GetAllAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(discipline))
        {
            items = items.Where(item => item.Discipline.Equals(discipline.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            items = items.Where(item =>
                item.Tag.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                item.Description.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        return Ok(items.ToList());
    }

    [HttpGet("{tag}")]
    public async Task<ActionResult<EquipmentItem>> Get(string tag, CancellationToken cancellationToken)
    {
        var item = await repository.FindByTagAsync(tag, cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("disciplines")]
    public ActionResult<IReadOnlyList<string>> GetDisciplines() => Ok(Disciplines.All);
}
