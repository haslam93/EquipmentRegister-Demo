using EquipmentRegister.Api.Domain;

namespace EquipmentRegister.Api.Import.Rules;

public sealed class StatusRule : IRowRule
{
    public string Id => "R005";

    public IEnumerable<RowError> Validate(ImportRow row, ImportContext context)
    {
        var status = row.Get(ImportColumns.Status);
        if (status.Length > 0 && !EquipmentStatuses.All.Contains(status, StringComparer.OrdinalIgnoreCase))
        {
            yield return new RowError(
                row.RowNumber,
                Id,
                ImportColumns.Status,
                $"Status '{status}' is not recognised. Use one of: {string.Join(", ", EquipmentStatuses.All)}, or leave it blank for {EquipmentStatuses.Default}.");
        }
    }
}
