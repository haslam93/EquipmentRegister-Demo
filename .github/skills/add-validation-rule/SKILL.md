---
name: add-validation-rule
description: Add or change an import validation rule (R0xx) for equipment list CSV imports - rule class, registration, tests, sample file and documentation. Use for any work on import validation, rejected rows, row errors, duplicate checks or rule ids.
---

# Add an import validation rule

A rule is one class that implements `IRowRule`. `TagFormatRule` and `TagFormatRuleTests` are the
pattern to copy.

## Steps

1. **Pick the id.** Use the next free `R0xx` in `docs/validation-rules.md`. Ids are never reused or
   renumbered.
2. **Write the tests first** in
   `tests/EquipmentRegister.Api.Tests/Import/Rules/<Name>RuleTests.cs`.
   - Build rows with `Rows.Row(...)` and the context with `Rows.Context(...)`.
   - Cover a row that passes, each way a row can fail, and a blank value when the column is
     optional.
   - Assert `RuleId`, `Column`, `RowNumber` and the exact message the story specifies.
3. **Create the rule** in `src/EquipmentRegister.Api/Import/Rules/<Name>Rule.cs`.
   - `sealed` and stateless. Read values with `row.Get(ImportColumns.X)`; they are already trimmed.
   - Checks across rows (duplicates, totals) read `context.Rows`. Never keep state in fields: the
     rule is a singleton shared by every import.
   - A blank value is `RequiredFieldsRule`'s job. Return no error for it unless the story says
     otherwise.
   - Message: plain English, names the column, quotes the offending value, says how to fix it.
4. **Register it** in `src/EquipmentRegister.Api/Program.cs` after the existing rules, and add it to
   the rule list in `EquipmentImportServiceTests`.
5. **Show it in a sample.** Make sure a file in `samples/` triggers the rule so people can see it in
   the Import page. Add a row to `equipment-with-errors.csv` if no sample does; keep existing rows.
6. **Document it.** Add a row to the table in `docs/validation-rules.md` in the same wording style.
7. **Verify** with `dotnet test EquipmentRegister.sln`.

## Do not

- Change the behaviour of an existing rule, or renumber rules.
- Reject a whole file for a row-level problem. Row rules reject rows.
- Parse numbers yourself. Reuse `NumericFieldsRule.TryParse` (invariant culture, no thousands
  separators).
