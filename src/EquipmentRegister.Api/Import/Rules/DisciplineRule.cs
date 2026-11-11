using EquipmentRegister.Api.Domain;

namespace EquipmentRegister.Api.Import.Rules;

public sealed class DisciplineRule : IRowRule
{
    public string Id => "R003";

    public IEnumerable<RowError> Validate(ImportRow row, ImportContext context)
    {
        var discipline = row.Get(ImportColumns.Discipline);
        if (discipline.Length > 0 && !Disciplines.All.Contains(discipline, StringComparer.OrdinalIgnoreCase))
        {
            yield return new RowError(
                row.RowNumber,
                Id,
                ImportColumns.Discipline,
                $"Discipline '{discipline}' is not recognised. Use one of: {string.Join(", ", Disciplines.All)}.");
        }
    }
}
