# Equipment Register

A small, realistic application for showing how GitHub Copilot fits into the whole delivery
lifecycle, from a user story to a reviewed, tested and secure change, and not only into writing code.

Engineers on a capital project import equipment lists (CSV) into a register. Every row is validated,
problems are reported with the spreadsheet row number, and client-facing reports are generated from
the register.

> [!WARNING]
> This is a demonstration repository. It contains deliberately planted security flaws and code
> problems, used to show code scanning and code review. Do not deploy it.

## Stack

| Part | Technology |
| --- | --- |
| API | ASP.NET Core Web API on .NET 10 (`src/EquipmentRegister.Api`) |
| Web | Angular 22 with standalone components and signals (`src/web`) |
| Tests | xUnit and Vitest |
| Delivery | GitHub Actions, CodeQL, GitHub Agentic Workflows |

## Run it

You need the .NET 10 SDK and Node.js 22.

```powershell
./scripts/setup.ps1                                # restore, npm ci, enable the pre-commit hook
dotnet run --project src/EquipmentRegister.Api     # API on http://localhost:5080
npm start --prefix src/web                         # web app on http://localhost:4200
```

In VS Code, **Tasks: Run Build Task** starts both. Open the Import page and try the files in
`samples/`.

## One repository, every Copilot surface

Everything that shapes how Copilot works here is in the repository, so VS Code, Copilot CLI, the
GitHub Copilot app, Copilot cloud agent and Copilot code review all follow the same rules.

| Building block | Where | What it does |
| --- | --- | --- |
| Instructions | `.github/copilot-instructions.md`, `.github/instructions/` | Layout, commands and working agreements, plus path-specific rules for C#, Angular and report templates |
| Skills | `.github/skills/` | Repeatable procedures: `implement-story`, `add-validation-rule`, `report-style`, `code-review` |
| Custom agents | `.github/agents/` | Roles: `story-analyst`, `test-engineer`, `security-remediator` |
| MCP servers | `.mcp.json` | Tool connections shared by the team (Context7 for current library documentation). Personal ones, such as Azure DevOps, go in `~/.copilot/mcp-config.json` |
| Agentic workflows | `.github/workflows/*.md` | Automation written in Markdown: Story Readiness Check, CI Doctor, Weekly Delivery Report |
| Cloud agent environment | `.github/workflows/copilot-setup-steps.yml` | Installs .NET and Node before an Actions-based agent session starts |
| Guardrails | `.githooks/pre-commit`, `.github/workflows/ci.yml`, CodeQL | Checks that do not rely on anyone, human or agent, remembering |

## Stories

The backlog is in GitHub Issues (label `story`); the source is `scripts/stories.json`. Stories
follow `docs/definition-of-ready.md`.

Teams that keep their backlog in Azure DevOps can create the same stories as Azure Boards work
items with `scripts/seed-azure-boards.ps1`, then reference them as `AB#<id>` in issues and pull
request descriptions to link the work item to the change.

## Agentic workflows

The workflows are Markdown files compiled to GitHub Actions with the
[`gh aw`](https://github.github.com/gh-aw/) extension. After editing a `.md` file run
`gh aw compile` and commit the `.md` and the generated `.lock.yml` together.

| Workflow | Trigger | Result |
| --- | --- | --- |
| `story-readiness` | A story issue is opened or edited | Labels it `ready` or `needs-refinement` and comments with the gaps or a task breakdown |
| `ci-doctor` | The CI workflow fails | Explains the failure on the pull request, or opens an issue for failures on `main` |
| `delivery-report` | Weekly, or run manually | Opens an issue summarising story, pull request and CI status for the engineering manager |

## Documentation

- [Import validation rules](docs/validation-rules.md)
- [Report style guide](docs/report-style-guide.md)
- [Definition of Ready](docs/definition-of-ready.md)
