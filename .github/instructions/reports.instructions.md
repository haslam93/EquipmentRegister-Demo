---
applyTo: "src/EquipmentRegister.Api/Reports/Templates/**"
---

# Report templates

Reports are sent to clients. Use the `report-style` skill for any change to these files.

- Only the fonts, sizes and palette in `docs/report-style-guide.md`. No named colours, `rgb()` or
  inline `style` attributes.
- Keep the standard `report-header` and `report-footer` blocks.
- Keep every `{{placeholder}}` exactly as written; `ReportBuilder.cs` fills them in.
- Run `node tools/check-report-style.mjs` before committing. Never add a template to the `exempt`
  list or change the palette to make the check pass.
