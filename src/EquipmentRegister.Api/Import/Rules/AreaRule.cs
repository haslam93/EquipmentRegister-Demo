using EquipmentRegister.Api.Reports;
using Microsoft.Extensions.Options;

namespace EquipmentRegister.Api.Import.Rules;

public sealed class AreaRule(IOptions<ProjectOptions> project) : IRowRule
{
    public string Id => "R007";

    public IEnumerable<RowError> Validate(ImportRow row, ImportContext context)
    {
        var area = row.Get(ImportColumns.Area);
        var configuredAreas = project.Value.Areas;

        if (area.Length == 0 || configuredAreas.Count == 0 ||
            configuredAreas.Any(configured => string.Equals(configured.Code, area, StringComparison.Ordinal)))
        {
            yield break;
        }

        yield return new RowError(
            row.RowNumber,
            Id,
            ImportColumns.Area,
            $"Area '{area}' is not an area of this project. Use one of: {string.Join(", ", configuredAreas.Select(configured => configured.Code))}.");
    }
}
