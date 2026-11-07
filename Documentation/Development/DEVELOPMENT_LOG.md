# DEVELOPMENT LOG

Append-only, chronological, newest at the bottom. Only real events. Never fabricate or backfill history.

Entry template:
```
## YYYY-MM-DD — Mx — <task>
- Files modified:
- Summary:
- Tests executed:
- Test results:
- Known issues:
- Decisions:
```

---

## 2026-11-06 — Pre-M0 — Static audit
- Files modified: created `PROJECT_AUDIT.md` (only).
- Summary: read-only static audit of the repository (Unity 6.3 URP; Unity "Tanks!" tutorial project; no git; broken Build Settings scene path; no tests).
- Tests executed: none. Nothing compiled, built or run.
- Test results: n/a.
- Known issues: see `KNOWN_ISSUES.md`.
- Decisions: none.

## 2026-11-06 — Pre-M0 — Workflow documentation created
- Files modified: created `CLAUDE.md` and everything under `Documentation/Development/`. No Unity project files, scripts, scenes, prefabs, settings or packages were modified.
- Summary: additional static inspection (remaining scripts, input action schemes, serialization format, ProjectSettings). Findings recorded in `KNOWN_ISSUES.md`; the audit was *extended* in places (see "Audit corrections" in `NEXT_SESSION.md`).
- Tests executed: none.
- Test results: n/a.
- Known issues: KI-001…KI-010, potential risks KI-P01…KI-P07.
- Decisions: none made (all ADRs PROPOSED/PENDING).

## 2026-11-06 — M0 — Stage 1: baseline inspection
- Files modified: created `M0_BASELINE.md`; updated `MILESTONE_STATUS.md`, `NEXT_SESSION.md`, `KNOWN_ISSUES.md` (notes only). No Unity project files, scripts, scenes, prefabs, settings or packages modified.
- Summary: inspected docs, settings, scene paths, scene-load calls, input path, power-up lifecycle, toolchain. One read-only Editor query via Unity MCP (serialization = Mixed, Visible Meta Files already on, project-wide actions = Tank_Actions, Build Settings = 1 scene Jungle, Win+Android modules installed). `adb devices`: none connected.
- Tests executed: none (no compile/build/run). Console read: 0 errors, 1 unrelated warning.
- Test results: n/a.
- Decisions recorded from user: git DEFERRED; Force Text and Visible Meta Files APPROVED; build index 0 = Desert.
- Stopped at Stage A: waiting for backup confirmation and user applying Force Text.
