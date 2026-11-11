# Report style guide

Reports leave the project team and are read by clients, so every report template must look like it
came from the same firm. These rules apply to the HTML templates in
`src/EquipmentRegister.Api/Reports/Templates/`.

The machine-readable version of this guide is `tools/report-style.config.json`. Change the two
together.

## Typography

| Use | Font stack | Size |
| --- | --- | --- |
| Body text, tables, headings | `Arial, Helvetica, sans-serif` | 10pt body, 13pt section heading, 18pt report title |
| Tags and codes | `Consolas, 'Courier New', monospace` | same as surrounding text |
| Footer | `Arial, Helvetica, sans-serif` | 9pt |

- Sizes are set in **pt** (reports are printed). Nothing smaller than **9pt**.
- No other fonts.

## Colour palette

Use the hex values exactly. Named colours (`red`, `purple`) and `rgb()` / `hsl()` are not allowed.

| Token | Hex | Use |
| --- | --- | --- |
| brand | `#D9480F` | Rule under the report header, section headings |
| ink | `#1F2933` | Body text, table header background |
| muted | `#52606D` | Secondary text, footer |
| rule | `#CBD2D9` | Table and footer lines |
| band | `#F5F7FA` | Alternate table rows |
| paper | `#FFFFFF` | Page background, table header text |
| ok | `#2F855A` | Positive status |
| warn | `#B7791F` | Caution status |
| alert | `#C53030` | Problem status, totals that need attention |

## Structure

- All styling lives in the `<style>` block. No inline `style="..."` attributes.
- Every report starts with `<header class="report-header">` (title, then project name and number)
  and ends with `<footer class="report-footer">` showing `Generated {{generatedOn}}` and
  "Uncontrolled when printed".
- Numeric columns use `class="number"` (right aligned).

`equipment-summary.html` is the reference implementation.

## Checking

```
node tools/check-report-style.mjs          # all templates except the exempt list
node tools/check-report-style.mjs --all    # include exempt (legacy) templates
```

| Rule | Checks |
| --- | --- |
| RS001 | `font-family` is one of the approved stacks |
| RS002 | Every colour is a palette hex value |
| RS003 | `font-size` is in pt and at least 9pt |
| RS004 | No inline `style` attributes |
| RS005 | The standard header and footer blocks are present |

The check runs in CI and in the pre-commit hook (`.githooks/pre-commit`) whenever a report template
is staged. Legacy templates that have not been brought into line are listed under `exempt` in the
config; remove a template from that list when you fix it.
