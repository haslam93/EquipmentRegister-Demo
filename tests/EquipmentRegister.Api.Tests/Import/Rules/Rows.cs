using EquipmentRegister.Api.Import;

namespace EquipmentRegister.Api.Tests.Import.Rules;

internal static class Rows
{
    public static ImportRow Row(
        int rowNumber = 2,
        string tag = "PU-1001A",
        string description = "Slurry pump",
        string discipline = "Process",
        string area = "100",
        string powerKw = "",
        string weightKg = "",
        string status = "") =>
        new(rowNumber, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [ImportColumns.Tag] = tag,
            [ImportColumns.Description] = description,
            [ImportColumns.Discipline] = discipline,
            [ImportColumns.Area] = area,
            [ImportColumns.PowerKw] = powerKw,
            [ImportColumns.WeightKg] = weightKg,
            [ImportColumns.Status] = status,
        });

    public static ImportContext Context(params ImportRow[] rows) => new(rows);
}
