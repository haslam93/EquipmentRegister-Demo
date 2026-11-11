using System.Text.RegularExpressions;

namespace EquipmentRegister.Api.Import.Rules;

public sealed partial class TagFormatRule : IRowRule
{
    public string Id => "R002";

    public IEnumerable<RowError> Validate(ImportRow row, ImportContext context)
    {
        var tag = row.Get(ImportColumns.Tag);
        if (tag.Length > 0 && !TagPattern().IsMatch(tag))
        {
            yield return new RowError(
                row.RowNumber,
                Id,
                ImportColumns.Tag,
                $"Tag '{tag}' must be 2-3 capital letters, a dash, 4 digits and an optional suffix letter (for example PU-2010A).");
        }
    }

    [GeneratedRegex(@"^[A-Z]{2,3}-\d{4}[A-Z]?$", RegexOptions.CultureInvariant)]
    private static partial Regex TagPattern();
}
