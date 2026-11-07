# M0 — Project Stabilization

**Status: NOT STARTED** · Complexity: S–M · Tags: `[Editor]` needs Unity Editor · `[Build]` needs a build · `[Device]` needs a physical device · `[Approval]` needs owner approval first.

## 1. Objective
A safe, buildable, version-controlled baseline.

## 2. Current repository state (2026-11-06, STATIC)
No git repo (KI-002); binary scenes/prefabs (KI-003); Build Settings reference missing scene (KI-001); placeholder identity (KI-004); Project-wide Input Actions not persisted (KI-006); never built.

## 3. Scope
In: items below. Out: gameplay changes, folder migration, package changes (unless approved).

## 4. Prerequisites
Owner approvals for git init and Force Text. Unity 6000.3.6f1 installed. Android Build Support (SDK/NDK/JDK) — availability UNKNOWN.

## 5. Tasks
- [x] `[Approval]` Create Unity `.gitignore` — DONE 2026-11-07 (GitHub Unity template from remote + `.vscode/ .idea/` OS junk keystores). Original spec: (exclude `Library/ Temp/ Logs/ UserSettings/ obj/ Build*/ *.csproj *.sln *.slnx .vs/ .vscode/ (optional)` ; keep `Packages/manifest.json`, `packages-lock.json`, `ProjectSettings/`).
- [ ] `[Approval]` Decide: Git LFS for large binaries (fbx/exr/wav/png)? (PENDING)
- [ ] `[Editor]` Project Settings ▸ Editor: Asset Serialization = **Force Text**; Version Control = **Visible Meta Files**.
- [ ] `[Editor][Approval]` Reserialize all assets (Assets ▸ Reimport All / Force Reserialize). Verify scenes/prefabs now start with `%YAML`.
- [x] `[Approval]` `git init`; first baseline commit — DONE 2026-11-07, owner-approved; baseline `784095d` on `dev`/`xyz-backup` (pushed, verified via `git ls-remote`). Binary assets committed as-is; Force Text reserialize will be a separate large commit.
- [ ] `[Editor]` Fix Build Settings: add `Demo_Game_Jungle/Desert/Moon` scenes; choose index 0 (decision logged in ADR-011 / log).
- [ ] `[Editor]` Open each of the 3 scenes: record Console errors/warnings and any Missing references (do not fix unrelated ones; file issues).
- [ ] `[Editor]` Verify Project Settings ▸ Input System Package ▸ Project-wide Actions = `Tank_Actions`; if not, assign and save (KI-006) — explain impact.
- [ ] `[Editor]` Set development identity (company, package id placeholder → dev id) — final values deferred to M7 (KI-004).
- [ ] `[Build]` Windows x64 development build; launch; reach menu; play one match.
- [ ] `[Build][Device]` Android development build (IL2CPP, ARM64); install; reach menu.
- [ ] Record baseline facts in log/status; update KI statuses.

## 6. Technical implementation notes
- Do not edit binary files by hand. Use Editor.
- Keep reserialization as its own commit (no mixed changes).
- `ProjectWideActionsSetup.cs` only runs in Editor — it is not a runtime fix.
- Do not upgrade packages/Unity.

## 7. Files/systems likely affected
`.gitignore` (new), `ProjectSettings/EditorSettings.asset`, `ProjectSettings/EditorBuildSettings.asset`, all `.unity/.prefab/.asset` (reserialization), possibly `ProjectSettings/ProjectSettings.asset` (identity, preloaded assets).

## 8. Risks
Large reserialization diff; Android toolchain absence; silent behavior changes after reimport; scenes with latent missing refs surfacing.

## 9. Validation steps
Re-open scenes after reserialize (compare by playing a match); fresh clone test (B-05); Windows build played; Android install log (A-01); `git status` clean of generated folders.

## 10. Acceptance criteria
- Valid Build Settings; correct index 0.
- Force Text active; assets YAML; `.meta` visible.
- Repo exists with a baseline commit (owner-approved), generated folders ignored.
- All 3 scenes open with console results recorded.
- Windows dev build reaches menu and completes a match; Android dev build reaches menu (or milestone → REVIEW REQUIRED if no device).

## 11. Definition of Done
All acceptance criteria met with evidence in `DEVELOPMENT_LOG.md`; `KNOWN_ISSUES` KI-001/002/003 closed (VERIFIED FIXED); docs updated.

## 12. Completion summary
_(empty — fill when done)_
