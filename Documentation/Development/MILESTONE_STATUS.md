# MILESTONE STATUS

Allowed statuses: NOT STARTED · IN PROGRESS · BLOCKED · REVIEW REQUIRED · COMPLETED.
"Completion" is stated as tasks done / mandatory tasks — a count, not an estimate of effort. A milestone is not COMPLETED because files changed; only when acceptance criteria are met (CLAUDE.md §M).

**Active milestone: M0 — Project Stabilization** (next to start; nothing begun).
Last updated: 2026-11-06 (documentation created; no development work performed).

| ID | Milestone | Status | Completion | Tasks done | Tasks pending | Blockers | Current focus | Next action |
|---|---|---|---|---|---|---|---|---|
| M0 | Project Stabilization | IN PROGRESS | baseline + investigations done; config changes pending | baseline, env assessment, static input/power-up/scene-load investigation | backup confirm, Force Text, Build Settings order, builds | REQUIRES USER ACTION: backup + Editor settings; git DEFERRED | Stage A | User: confirm backup, apply Force Text (see M0_BASELINE.md / session message) |
| M1 | Gameplay Validation | NOT STARTED | 0 | none | all | Needs M0 | — | — |
| M2 | Platform Input & Compatibility | NOT STARTED | 0 | none | all | Needs M0; physical Android device (UNKNOWN) | — | — |
| M3 | Game Foundation | NOT STARTED | 0 | none | all | Needs M0–M2; name/namespace/migration decisions | — | — |
| M4 | Gameplay & UX Polish | NOT STARTED | 0 | none | all | Needs M3 | — | — |
| M5 | Original Content | NOT STARTED | 0 | none | all | Needs M3; licensing findings | — | — |
| M6 | Optimization & QA | NOT STARTED | 0 | none | all | Needs M4/M5; perf budgets PENDING | — | — |
| M7 | Commercial Release | NOT STARTED | 0 | none | all | Needs M6; name, pricing, business model, licensing | — | — |

## Cross-cutting pending approvals
- Initialize git and add a Unity `.gitignore` (M0).
- Force Text serialization (re-saves all binary scenes/prefabs) (M0).
- Which scene is build index 0 (M0).
- Folder migration target (M3, ADR-004).

## Status change log
- 2026-11-06: M0 NOT STARTED → IN PROGRESS (baseline written; awaiting user Editor actions).
