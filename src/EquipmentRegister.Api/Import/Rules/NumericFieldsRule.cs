using System.Globalization;

namespace EquipmentRegister.Api.Import.Rules;

public sealed class NumericFieldsRule : IRowRule
{
    private static readonly string[] NumericColumns = [ImportColumns.PowerKw, ImportColumns.WeightKg];

    public string Id => "R004";

    public IEnumerable<RowError> Validate(ImportRow row, ImportContext context)
    {
        foreach (var column in NumericColumns)
        {
            var raw = row.Get(column);
            if (raw.Length == 0)
            {
                continue;
            }

            if (!TryParse(raw, out var value))
            {
                yield return new RowError(
                    row.RowNumber,
                    Id,
                    column,
                    $"{column} '{raw}' is not a number. Use digits with '.' as the decimal separator and no thousands separator.");
            }
            else if (value < 0)
            {
                yield return new RowError(row.RowNumber, Id, column, $"{column} cannot be negative.");
            }
        }
    }

    /// <summary>
    /// Thousands separators are rejected on purpose: "1,5" exported from a comma-decimal
    /// locale would otherwise be read as 15.
    /// </summary>
    public static bool TryParse(string raw, out decimal value) =>
        decimal.TryParse(
            raw,
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture,
            out value);
}
