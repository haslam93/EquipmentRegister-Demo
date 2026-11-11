---
applyTo: "**/*.cs"
---

# C# conventions

- Target .NET 10 with nullable reference types enabled. Prefer `sealed` classes, records for data
  and primary constructors for dependency injection.
- Async all the way: never block on a task with `.Result`, `.Wait()` or
  `.GetAwaiter().GetResult()`. Accept a `CancellationToken` in async methods and pass it on.
- Parse and format numbers and dates with `CultureInfo.InvariantCulture`. For import values reuse
  `NumericFieldsRule.TryParse`; it rejects thousands separators on purpose.
- Controllers stay thin: validate the request, call a service, return the result. Client errors
  return `ProblemDetails` (`Problem(...)`), never a bare string.
- Request input is untrusted:
  - file names: `Path.GetFileName` plus an allow-list; never `Path.Combine` raw input
  - logging: message templates with placeholders (`logger.LogInformation("Imported {FileName}", name)`),
    never string concatenation or interpolation
  - HTML: `WebUtility.HtmlEncode`
- Tests are xUnit. One behaviour per test, named for the behaviour (`Rejects_negative_weight`).
  Rule tests build rows with the `Rows` helper. Endpoint tests use `WebApplicationFactory<Program>`.
