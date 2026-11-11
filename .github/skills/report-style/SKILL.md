---
name: report-style
description: Create or fix HTML report templates so they follow the report style guide (approved fonts, sizes, colour palette, standard header and footer) and pass the report style check. Use when adding, editing, reviewing or fixing anything in Reports/Templates, when asked about report formatting or branding, or when the report style check or pre-commit hook fails.
---

# Report style

Reports go to clients. Every template in `src/EquipmentRegister.Api/Reports/Templates/` must follow
`docs/report-style-guide.md`. The machine-readable rules are in `tools/report-style.config.json`.

## Procedure

1. See where the template stands:
   `node tools/check-report-style.mjs --all`
2. Fix each problem using the table below. Copy structure and class names from
   `equipment-summary.html`, the reference template.
3. Keep every `{{placeholder}}` exactly as it is. `ReportBuilder.cs` fills them in; do not rename or
   remove any.
4. If the template is listed under `exempt` in `tools/report-style.config.json`, remove it from the
   list. That is what brings it under the check for good.
5. Run `node tools/check-report-style.mjs` until it prints `OK`, then
   `dotnet test EquipmentRegister.sln`.
6. Look at the result: `dotnet run --project src/EquipmentRegister.Api`, then open
   `http://localhost:5080/api/reports/<report-name>`.

## Fixing each rule

| Rule | Fix |
| --- | --- |
| RS001 | Use `Arial, Helvetica, sans-serif`. Use `Consolas, 'Courier New', monospace` only for tags and codes. |
| RS002 | Replace the colour with the palette value for its role, not simply the nearest shade: body text `#1F2933`, secondary text `#52606D`, section headings and the header rule `#D9480F`, table header `#1F2933` with `#FFFFFF` text, lines `#CBD2D9`, alternate rows `#F5F7FA`, problems and totals that need attention `#C53030`. |
| RS003 | Sizes in pt: report title 18pt, section heading 13pt, body and tables 10pt, footer 9pt. Nothing below 9pt. |
| RS004 | Move inline `style="..."` into the `<style>` block as a class. |
| RS005 | Add `<header class="report-header">` (the `h1` title, then project name and number) and `<footer class="report-footer">` containing `Generated {{generatedOn}}` and "Uncontrolled when printed". |

## Rules of the road

- Never loosen `tools/report-style.config.json`, and never add a template to `exempt`, to make the
  check pass.
- The pre-commit hook (`.githooks/pre-commit`) and CI run the same check, so a template that passes
  locally passes there.
