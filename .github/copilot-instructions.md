# Equipment Register - repository instructions

Equipment Register imports equipment lists (CSV) for a capital project into a register, validates
every row, and produces client-facing reports.

## Layout

| Path | What lives there |
| --- | --- |
| `src/EquipmentRegister.Api` | ASP.NET Core Web API (.NET 10): controllers, import pipeline, report builder |
| `src/EquipmentRegister.Api/Import/Rules` | One class per import validation rule (`IRowRule`), registered in `Program.cs` |
| `src/EquipmentRegister.Api/Reports/Templates` | HTML report templates; must follow `docs/report-style-guide.md` |
| `src/web` | Angular 22 app (standalone components, signals); calls the API through `/api` |
| `tests/EquipmentRegister.Api.Tests` | xUnit tests for the API |
| `docs` | `validation-rules.md`, `report-style-guide.md`, `definition-of-ready.md` |
| `samples` | CSV files for trying imports |

## Commands

Run from the repository root unless a folder is given.

| Purpose | Command |
| --- | --- |
| Build and test the API | `dotnet test EquipmentRegister.sln` |
| Install web dependencies | `npm ci` in `src/web` |
| Build and test the web app | `npm run build` and `npm run test:ci` in `src/web` |
| Check report templates | `node tools/check-report-style.mjs` |
| Run the API | `dotnet watch --no-hot-reload --project src/EquipmentRegister.Api run -c Release` (http://localhost:5080). Release keeps it clear of test builds, which are Debug |
| Run the web app | `npm start` in `src/web` (http://localhost:4200, proxies `/api` to the API) |

## Working agreements

- Start from the story. Read the issue or Azure Boards work item and its acceptance criteria before
  changing code. If the story fails `docs/definition-of-ready.md`, say what is missing; do not guess.
- Make the smallest change that satisfies the acceptance criteria. No drive-by refactoring.
- Every behaviour change comes with tests: at least one per acceptance criterion, named for the
  behaviour it proves.
- Keep `docs/validation-rules.md` in step with the rules, in the same change.
- Before finishing, run the commands that cover what you touched. Report what you ran and the
  result, and say plainly what you did not run.
- Verify with builds, tests and checks. Do not start the API or the web server unless you are asked
  to; someone may already be running them on ports 5080 and 4200.

## Conventions that matter here

- Import files come from many locales: parse and format numbers and dates with
  `CultureInfo.InvariantCulture`.
- Row numbers shown to users are spreadsheet rows (the header is row 1).
- Validation messages are plain English, name the column, and quote the offending value.
- Never build file paths, log messages or HTML from raw request input.
- Report templates use only the fonts, sizes and colours in `docs/report-style-guide.md`.

Language-specific rules are in `.github/instructions/`.

## Pull requests

- Title: imperative mood, under 70 characters.
- Fill in `.github/pull_request_template.md`: the work item (`Closes #<issue>`, plus `AB#<id>` when
  the story has an Azure Boards item), what changed, each acceptance criterion with the test that
  covers it, and the commands you ran.
