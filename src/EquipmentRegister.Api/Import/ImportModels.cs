namespace EquipmentRegister.Api.Import;

/// <summary>
/// A data row from an import file. <see cref="RowNumber"/> matches the spreadsheet row
/// the engineer sees: the header is row 1, the first data row is row 2.
/// </summary>
public sealed record ImportRow(int RowNumber, IReadOnlyDictionary<string, string> Values)
{
    public string Get(string column) =>
        Values.TryGetValue(column, out var value) ? value.Trim() : string.Empty;
}

/// <summary>Everything a rule may need beyond the row itself, such as the other rows in the file.</summary>
public sealed record ImportContext(IReadOnlyList<ImportRow> Rows);

public sealed record RowError(int RowNumber, string RuleId, string Column, string Message);

public sealed record ImportResult(
    string FileName,
    int TotalRows,
    int Accepted,
    int Rejected,
    IReadOnlyList<RowError> Errors);

public static class ImportColumns
{
    public const string Tag = "Tag";
    public const string Description = "Description";
    public const string Discipline = "Discipline";
    public const string Area = "Area";
    public const string PowerKw = "PowerKw";
    public const string WeightKg = "WeightKg";
    public const string Status = "Status";

    public static readonly IReadOnlyList<string> Required = [Tag, Description, Discipline, Area];
}
