---
description: |
  Weekly delivery report for engineering managers: where every story stands,
  what shipped, what is waiting on people, and what is at risk, built from the
  repository's issues, pull requests and CI runs.

on:
  schedule: weekly
  workflow_dispatch:
  stop-after: +30d

permissions:
  actions: read
  contents: read
  issues: read
  pull-requests: read
  copilot-requests: write

tools:
  github:
    toolsets: [default, actions]

safe-outputs:
  mentions: false
  create-issue:
    title-prefix: "[Delivery report] "
    labels: [report]
    close-older-issues: true
    max: 1

timeout-minutes: 15
---

# Weekly Delivery Report

Write the weekly delivery report for the engineering manager of ${{ github.repository }}. The reader
has five minutes and wants to know what moved, what is stuck and where to step in.

## Window

The last 7 full days ending when this run starts (UTC). State the window dates at the top.

## Gather

1. Stories: issues labelled `story`. For each one note its readiness label (`ready`,
   `needs-refinement` or none), whether an open pull request references it, who or what is working
   on it (a person, or Copilot when a pull request was opened by Copilot), and whether it was
   closed in the window.
2. Pull requests opened, merged or still open in the window: author, age in days, review state and
   check status.
3. CI: runs of the `CI` workflow in the window, with the number of failures and their causes. Use
   any `[CI failure]` issues and CI Doctor comments.
4. Open code scanning or `[CI failure]` issues that nobody has picked up.

## Report

Create one issue titled with the window end date (for example `Week ending 2026-11-13`). Use `###`
headings, never `#` or `##`:

```markdown
### Summary
<Three sentences at most: overall progress, the biggest win, the biggest risk.>

| Stories done | In progress | Ready, not started | Needs refinement | PRs merged | CI failures |
| --- | --- | --- | --- | --- | --- |

### Needs your attention
<Only items a manager must act on: stories blocked on refinement, pull requests waiting more than
2 days for review, repeated CI failures. One line each: what, how long, who can unblock it.>

### Story status
| Story | State | Working on it | Pull request | Note |
| --- | --- | --- | --- | --- |

<details><summary>Pull requests and CI detail</summary>

<lists of the pull requests and CI failures behind the numbers>

</details>

### Suggested next steps
<Two or three concrete actions for the coming week.>
```

## Rules

- Report facts from the repository. When something cannot be determined, say "unknown"; do not
  estimate.
- Use an "Unclassified" row for stories without a readiness label.
- If nothing happened in the window, call `noop` and state the window you checked.
