---
name: implement-story
description: Implement a user story from a GitHub issue or Azure Boards work item end to end - check it is ready, plan the smallest slice, write tests from the acceptance criteria, implement, verify and prepare the pull request. Use when asked to implement, build, pick up or work on a story, issue or work item.
---

# Implement a story

Follow these steps in order. Do not skip the readiness check or the evidence.

## 1. Read the story

- GitHub issue: read the title, body, labels and comments (GitHub tools, or
  `gh issue view <number> --comments`).
- Azure Boards work item (`AB#<id>` or a dev.azure.com link): read it with the Azure DevOps tools
  when they are available; otherwise ask for the text.
- Write the acceptance criteria as a numbered list. Keep the numbering: tests and the pull request
  refer to it.

## 2. Check readiness

- The story has the `ready` label: the Story Readiness check has already passed it. Continue.
- The story has the `needs-refinement` label: stop and point to the open questions on the story.
- No readiness label: compare the story with `docs/definition-of-ready.md` yourself. If it is not
  ready, stop and reply with the gaps as questions for the author. Do not invent requirements.

## 3. Plan the smallest slice

Name the files you will add or change and the checks you will run. Use the more specific skill when
one fits:

| Work | Skill |
| --- | --- |
| A new or changed import validation rule | `add-validation-rule` |
| A report template | `report-style` |

When working interactively, share the plan before editing.

## 4. Tests first

Write at least one test per acceptance criterion, named for the behaviour, and watch it fail for
the right reason. Where an existing check already proves a criterion (the report style check, an
existing test), name that check instead of writing a new test.

- API: xUnit in `tests/EquipmentRegister.Api.Tests` (rules in `Import/Rules`, endpoints in `Api`).
- Web: Vitest specs beside the component (`*.spec.ts`).

## 5. Implement

Change only what the acceptance criteria need. Follow `.github/copilot-instructions.md` and the
files in `.github/instructions/`.

## 6. Verify

Run what covers the change and keep the output for the pull request.

| Changed | Run |
| --- | --- |
| API | `dotnet test EquipmentRegister.sln` |
| Web | `npm run build` and `npm run test:ci` in `src/web` (`npm ci` first in a fresh checkout) |
| Report templates | `node tools/check-report-style.mjs` |

## 7. Hand over

- Branch: `story/<issue-number>-<short-slug>`.
- Pull request body: fill in `.github/pull_request_template.md`.
  - `Closes #<issue>`, and `AB#<id>` on its own line when the story has an Azure Boards item.
  - Each acceptance criterion with the test that proves it.
  - The commands you ran and their results, and anything you did not run.
