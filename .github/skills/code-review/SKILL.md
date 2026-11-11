---
name: code-review
description: Review checklist for pull requests and local changes in this repository - correctness against the story's acceptance criteria, tests, security, culture-safe parsing, async usage and report style. Use when reviewing a pull request, a diff, or changes before committing.
---

# Code review checklist

Review like the person who will be called when this breaks. Report real problems only, most serious
first, each with the file and line and a concrete fix. Skip anything a formatter would catch.

## 1. Does it do what the story asked?

- Find the linked issue (`Closes #n`) or work item (`AB#n`) and read its acceptance criteria.
- Each criterion is implemented and has a test that would fail without the change. Name any
  criterion that has no test.
- Nothing outside the story's scope changed.

## 2. Correctness

- Loops and ranges cover every item, including the first and the last.
- Row numbers reported to users are spreadsheet rows (the header is row 1).
- CSV output quotes fields that contain commas, quotes or line breaks, and doubles embedded quotes.
- Empty input, blank optional values and repeated imports are handled.

## 3. Security

- No file path, log message or HTML is built from raw request input. Expect file-name validation
  (`Path.GetFileName` plus an allow-list), logging placeholders, and `WebUtility.HtmlEncode`.
- CSV exports neutralise cells that start with `=`, `+`, `-` or `@` (formula injection).
- No secrets, tokens or connection strings in code, tests or samples.

## 4. .NET

- Numbers and dates are parsed and formatted with `CultureInfo.InvariantCulture`.
- No `.Result`, `.Wait()` or `.GetAwaiter().GetResult()` on tasks; the `CancellationToken` is passed
  through.
- Client errors return `ProblemDetails`.

## 5. Angular

- Standalone components, signals, `@if` / `@for`; colours from the CSS variables in `styles.css`.

## 6. Reports and docs

- Report templates pass `node tools/check-report-style.mjs`.
- `docs/validation-rules.md` matches the rules in the code.

## Output

One finding per problem: severity (blocker, should fix, consider), location, why it matters, and the
suggested change. Finish with a table of the acceptance criteria: criterion, implemented, tested.
