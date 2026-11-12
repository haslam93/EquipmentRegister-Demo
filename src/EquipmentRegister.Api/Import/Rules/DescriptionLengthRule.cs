using System.Globalization;

namespace EquipmentRegister.Api.Import.Rules;

public sealed class DescriptionLengthRule : IRowRule
{
    public string Id => "R008";

    public IEnumerable<RowError> Validate(ImportRow row, ImportContext context)
    {
        var description = row.Get(ImportColumns.Description);
        if (description.Length > 120)
        {
            yield return new RowError(
                row.RowNumber,
                Id,
                ImportColumns.Description,
                $"Description is {description.Length.ToString(CultureInfo.InvariantCulture)} characters long. Shorten it to 120 characters or fewer.");
        }
    }
}
