namespace EquipmentRegister.Api.Import.Rules;

public sealed class RequiredFieldsRule : IRowRule
{
    public string Id => "R001";

    public IEnumerable<RowError> Validate(ImportRow row, ImportContext context)
    {
        foreach (var column in ImportColumns.Required)
        {
            if (row.Get(column).Length == 0)
            {
                yield return new RowError(row.RowNumber, Id, column, $"{column} is required.");
            }
        }
    }
}
