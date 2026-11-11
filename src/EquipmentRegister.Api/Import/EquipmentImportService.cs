using EquipmentRegister.Api.Data;
using EquipmentRegister.Api.Domain;
using EquipmentRegister.Api.Import.Rules;

namespace EquipmentRegister.Api.Import;

/// <summary>
/// Imports an equipment list. Valid rows are added to the register (or update the item with
/// the same tag); invalid rows are rejected and reported with their spreadsheet row number.
/// </summary>
public sealed class EquipmentImportService(IEnumerable<IRowRule> rules, IEquipmentRepository repository)
{
    public const string FileRuleId = "FILE";

    private readonly IReadOnlyList<IRowRule> _rules = rules.ToList();

    public async Task<ImportResult> ImportAsync(string fileName, string csv, CancellationToken cancellationToken = default)
    {
        var lines = CsvParser.Parse(csv);
        if (lines.Count == 0)
        {
            return new ImportResult(fileName, 0, 0, 0, [new RowError(1, FileRuleId, string.Empty, "The file is empty.")]);
        }

        var header = lines[0].Fields.Select(field => field.Trim()).ToList();
        var dataRowCount = lines.Count - 1;
        var missing = ImportColumns.Required
            .Where(column => !header.Contains(column, StringComparer.OrdinalIgnoreCase))
            .ToList();
        if (missing.Count > 0)
        {
            var columns = string.Join(", ", missing);
            return new ImportResult(
                fileName,
                dataRowCount,
                0,
                dataRowCount,
                [new RowError(1, FileRuleId, columns, $"Missing required column(s): {columns}.")]);
        }

        var rows = lines.Skip(1).Select(line => ToRow(line, header)).ToList();
        var context = new ImportContext(rows);
        var errors = new List<RowError>();
        var accepted = 0;

        foreach (var row in rows)
        {
            var rowErrors = _rules.SelectMany(rule => rule.Validate(row, context)).ToList();
            if (rowErrors.Count > 0)
            {
                errors.AddRange(rowErrors);
                continue;
            }

            await repository.UpsertAsync(ToItem(row), cancellationToken);
            accepted++;
        }

        return new ImportResult(fileName, rows.Count, accepted, rows.Count - accepted, errors);
    }

    private static ImportRow ToRow(CsvLine line, IReadOnlyList<string> header)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < header.Count; i++)
        {
            values[header[i]] = i < line.Fields.Count ? line.Fields[i] : string.Empty;
        }

        return new ImportRow(line.LineNumber, values);
    }

    private static EquipmentItem ToItem(ImportRow row)
    {
        var discipline = row.Get(ImportColumns.Discipline);
        var status = row.Get(ImportColumns.Status);

        return new EquipmentItem(
            row.Get(ImportColumns.Tag),
            row.Get(ImportColumns.Description),
            Disciplines.All.First(known => known.Equals(discipline, StringComparison.OrdinalIgnoreCase)),
            row.Get(ImportColumns.Area),
            ToDecimal(row.Get(ImportColumns.PowerKw)),
            ToDecimal(row.Get(ImportColumns.WeightKg)),
            status.Length == 0
                ? EquipmentStatuses.Default
                : EquipmentStatuses.All.First(known => known.Equals(status, StringComparison.OrdinalIgnoreCase)));
    }

    private static decimal? ToDecimal(string raw) =>
        NumericFieldsRule.TryParse(raw, out var value) ? value : null;
}
