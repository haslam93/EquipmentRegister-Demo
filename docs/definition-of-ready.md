# Definition of Ready

A story is ready for development when every item below is true. The `story-analyst` agent and the
Story Readiness workflow both check stories against this list.

1. **User and value.** The story says who needs it and why: "As a *role*, I want *capability*, so that
   *outcome*."
2. **Acceptance criteria.** At least two, each one testable (Given / When / Then, or a checklist of
   observable results). They cover the main path and at least one error or edge case.
3. **One slice.** The story fits in a single pull request. Anything deliberately left out is listed
   under "Out of scope".
4. **Specifics.** It names what is affected: columns, rule ids, endpoints, screens or reports. A new
   validation rule states its exact error message.
5. **No open questions.** Unknowns and dependencies are resolved, or listed with an owner.
6. **Traceability.** When the story started in Azure Boards it carries the work item reference
   (`AB#<id>`), so the pull request can link back to it.

## Outcome

| Result | Label | Meaning |
| --- | --- | --- |
| Ready | `ready` | A developer or an agent can start without asking questions. |
| Needs refinement | `needs-refinement` | One or more items fail; the gaps are listed as questions for the author. |
