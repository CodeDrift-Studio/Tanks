# NEXT SESSION HANDOFF

Updated: 2026-11-06 (end of workflow-documentation session). Update this file at the end of every meaningful session.

## Current project state
Unity 6.3 (6000.3.6f1) URP project containing Unity's Tanks! tutorial game. **No development work has begun.** Not a git repo. Never compiled, built or run by Claude. Binary-serialized scenes/prefabs. Build Settings list a missing scene.

## Active milestone
**M0 — Project Stabilization — IN PROGRESS (Stage A, waiting on user).** See `M0_BASELINE.md`. (`Milestones/M0_PROJECT_STABILIZATION.md`)

## Done in the previous session
- Created `PROJECT_AUDIT.md`, `CLAUDE.md`, and all of `Documentation/Development/` (roadmap, status, log, ADRs, testing checklist, known issues, 8 milestone files).
- No Unity project files were modified.

## Incomplete
Everything in M0–M7.

## Files changed (previous session)
Documentation only: `PROJECT_AUDIT.md`, `CLAUDE.md`, `Documentation/Development/**`.

## Audit corrections / extensions discovered after PROJECT_AUDIT.md
- Scenes, all 154 prefabs and several ProjectSettings assets are **binary**, not "partially" (KI-003).
- PC gamepad appears unwired in code (KI-010); touch controls absent from scenes by string search (KI-008).
- Project-wide Input Actions not persisted (`preloadedAssets: []`) (KI-006).
- Power-up effects may leak when the tank is deactivated mid-effect (KI-P02).
- Remaining scripts now read: `PowerUp*`, `CameraControl`, `PauseMenu`, `TankInputUser`, `TankHealth`, `MobileUIControl`.

## Known issues
See `KNOWN_ISSUES.md` (KI-001…KI-011, KI-P01…KI-P07).

## Pending approvals / decisions
1. Approve `git init` + Unity `.gitignore` (M0).
2. Approve Force Text re-serialization (M0).
3. Which scene is build index 0; boot/menu scene plan (M0/M3).
4. Folder migration (ADR-004), namespace (ADR-005), perf budgets (ADR-009), final name, pricing, business model.
5. Licensing review ownership (KI-007).
6. Available Android device/toolchain? Is a Unity MCP/CLI available to Claude for compile/test runs? (UNKNOWN)

## Recommended next step
Ask the owner for M0 approvals, then execute M0 tasks in order: (1) `.gitignore` + git init, (2) Force Text + Visible Meta Files + reserialize (in Editor), (3) fix Build Settings, (4) open all scenes & log console results, (5) Windows dev build, (6) Android dev build. Log everything in `DEVELOPMENT_LOG.md`.

## Git update (2026-11-07)
Git is now set up (supersedes "Not a git repo" / "git DEFERRED" above and below). Work on **`dev`**. `main` = `0d9ef83` (GitHub initial commit, intentionally untouched); `dev` = `xyz-backup` = `784095d` (project baseline, binary assets). Policy: `CLAUDE.md` §N, `GIT_WORKFLOW.md`. Start each session with `git status --short --branch`.
- The docs/policy commit made on top of the baseline is local until the owner authorizes a push.
- Force Text reserialize (still pending) must be its own commit on `fix/force-text`, after the owner's independent backup is confirmed.
- Open: Git LFS decision; fresh-clone test (B-05); first `dev` → `main` PR only after M0 acceptance + owner approval.

## M0 update (2026-11-06)
Exact next action: user confirms an independent backup (copy of the project folder outside it), saves all open scenes, sets Edit ▸ Project Settings ▸ Editor ▸ Asset Serialization = Force Text (Visible Meta Files is already set), then sets Build Settings order Desert(0)/Jungle(1)/Moon(2). Then Claude re-verifies via read-only Editor query and file headers (`%YAML`), checks for GUID/meta changes, and continues with Stage F. Git remains DEFERRED. Code fixes (power-up reset, input null guards) need explicit approval.
