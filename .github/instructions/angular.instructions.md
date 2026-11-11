---
applyTo: "src/web/**"
---

# Angular conventions

- Angular 22: standalone components, signals for state (`signal`, `computed`), and built-in control
  flow (`@if`, `@for` with `track`). No NgModules, no `*ngIf` / `*ngFor`.
- Inject dependencies with `inject()`. All HTTP calls go through `EquipmentApi`; components never
  use `HttpClient` directly.
- File names follow the scaffold: `register-page.ts`, `register-page.html`, `register-page.spec.ts`.
- Colours come from the CSS variables in `src/styles.css`. Do not add hex values to component styles.
- Every interactive control has an accessible name (`aria-label` or a visible `<label>`).
- Tests are Vitest specs beside the component. Use `provideHttpClientTesting()` and assert on what
  the user sees in the DOM, not on private state.
- Check your work with `npm run build` and `npm run test:ci` in `src/web`.
