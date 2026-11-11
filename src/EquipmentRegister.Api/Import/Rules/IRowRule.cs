namespace EquipmentRegister.Api.Import.Rules;

/// <summary>
/// A single import validation rule. Rules are stateless, registered in Program.cs and run
/// against every data row. Return no errors when the row passes.
/// </summary>
public interface IRowRule
{
    /// <summary>Stable identifier shown to users and documented in docs/validation-rules.md.</summary>
    string Id { get; }

    IEnumerable<RowError> Validate(ImportRow row, ImportContext context);
}
