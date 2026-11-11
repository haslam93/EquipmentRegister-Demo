# Import validation rules

Every data row in an import file is checked against the rules below. A row that breaks any rule is
rejected and reported with its spreadsheet row number (the header is row 1). Rows that pass are added
to the register, or update the item that already has the same tag.

| Rule | Column(s) | A row is rejected when | Example message |
| --- | --- | --- | --- |
| R001 | Tag, Description, Discipline, Area | a required value is blank | `Description is required.` |
| R002 | Tag | the tag is not 2-3 capital letters, a dash, 4 digits and an optional suffix letter | `Tag 'pu-3030' must be 2-3 capital letters, a dash, 4 digits and an optional suffix letter (for example PU-2010A).` |
| R003 | Discipline | the discipline is not Mechanical, Electrical, Process, Piping, Instrumentation or Structural (case is ignored) | `Discipline 'Chemical' is not recognised. Use one of: ...` |
| R004 | PowerKw, WeightKg | a value is present but is not a plain decimal number (`.` as the decimal separator, no thousands separator), or is negative | `PowerKw '1,5' is not a number. ...` |
| R005 | Status | a value is present but is not Planned, Ordered, Delivered or Installed (blank means Planned) | `Status 'On hold' is not recognised. ...` |
| R007 | Area | the area does not match a configured project area code | `Area '900' is not an area of this project. Use one of: 100, 200, 300, 400, 500.` |

## File-level check

Before any row is validated the header must contain `Tag`, `Description`, `Discipline` and `Area`.
If one is missing the whole file is rejected with rule id `FILE`. `PowerKw`, `WeightKg` and `Status`
are optional columns.

## Where the rules live

| What | Where |
| --- | --- |
| Rule classes (one per rule) | `src/EquipmentRegister.Api/Import/Rules/` |
| Registration and run order | `src/EquipmentRegister.Api/Program.cs` |
| Rule tests (one class per rule) | `tests/EquipmentRegister.Api.Tests/Import/Rules/` |
| Files to try in the UI | `samples/` |

To add a rule, follow the `add-validation-rule` skill in `.github/skills/add-validation-rule/`.
