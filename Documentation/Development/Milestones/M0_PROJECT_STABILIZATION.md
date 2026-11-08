# M0 — Project Stabilization

**Status: COMPLETE (owner-reported acceptance, 2026-11-08; see §12)** (aligned with `MILESTONE_STATUS.md`) · Complexity: S–M · Effort: 3–7 days + owner/device availability · Tags: `[Editor]` needs Unity Editor · `[Build]` needs a build · `[Device]` needs a physical device · `[Approval]` needs owner approval first.

## 1. Objective
A safe, buildable, version-controlled baseline.

## 2. Current repository state (2026-11-06, STATIC)
No git repo (KI-002); binary scenes/prefabs (KI-003); Build Settings reference missing scene (KI-001); placeholder identity (KI-004); Project-wide Input Actions not persisted (KI-006); never built.

## 2a. Android-first alignment (2026-11-08)
The first release is **Android only** (`DECISION_REGISTER.md` DR-C1…C13; roadmap `MILESTONE_ROADMAP.md`). Therefore: the **Windows x64 development build task is DEFERRED** (out of first-release scope; may still be done as an Editor convenience, not an acceptance requirement); the **Android development build on a physical device is the primary acceptance item**. Original M0 acceptance criteria and outstanding work are otherwise preserved. Added: Android SDK/NDK/JDK path verification, recording of Android Player Settings (min/target SDK, orientation, architectures, graphics API, identifier — currently UNKNOWN/second-hand because `ProjectSettings.asset` is binary), and starting the licensing evidence inventory (KI-007) which continues into M1. Execution of items below still requires the owner (backup, Editor settings) and a device.

## 3. Scope
In: items below. Out: gameplay changes, folder migration, package changes (unless approved); service SDKs (M5/M6); Windows release work.

## 4. Prerequisites
Owner approvals for git init and Force Text. Unity 6000.3.6f1 installed. Android Build Support (SDK/NDK/JDK) — availability UNKNOWN.

## 5. Tasks
- [x] `[Approval]` Create Unity `.gitignore` — DONE 2026-11-07 (GitHub Unity template from remote + `.vscode/ .idea/` OS junk keystores). Original spec: (exclude `Library/ Temp/ Logs/ UserSettings/ obj/ Build*/ *.csproj *.sln *.slnx .vs/ .vscode/ (optional)` ; keep `Packages/manifest.json`, `packages-lock.json`, `ProjectSettings/`).
- [ ] `[Approval]` Decide: Git LFS for large binaries (fbx/exr/wav/png)? (PENDING)
- [ ] `[Editor]` Project Settings ▸ Editor: Asset Serialization = **Force Text**; Version Control = **Visible Meta Files**.
- [ ] `[Editor][Approval]` Reserialize all assets (Assets ▸ Reimport All / Force Reserialize). Verify scenes/prefabs now start with `%YAML`.
- [x] `[Approval]` `git init`; first baseline commit — DONE 2026-11-07, owner-approved; baseline `784095d` on `dev`/`xyz-backup` (pushed, verified via `git ls-remote`). Binary assets committed as-is; Force Text reserialize will be a separate large commit.
- [ ] `[Editor]` Fix Build Settings (**scope revised 2026-11-08, DR-C14**): the first-release build needs **only `Demo_Game_Desert` at index 0, enabled**. Jungle and Moon may stay in the list **disabled** (or be removed) — they are deferred content and must not be in the release build. STATIC observation 2026-11-08 (file read, not Editor-verified, not changed by this task; the file changed while this documentation task ran): an earlier read showed Desert (enabled, first), Jungle (disabled), Moon (disabled); the latest read shows `ProjectSettings/EditorBuildSettings.asset` listing **only `Demo_Game_Desert` (enabled)**, and the `Dev-Android` Build Profile has `m_OverrideGlobalSceneList: 0` (uses that global list). This matches the revised requirement. Confirm in File ▸ Build Profiles before ticking.
- [ ] `[Editor]` Open the **Desert** scene: record Console errors/warnings and any Missing references (do not fix unrelated ones; file issues). Jungle and Moon are deferred (DR-C14); open them only as optional, non-blocking information.
- [ ] `[Editor]` Verify Project Settings ▸ Input System Package ▸ Project-wide Actions = `Tank_Actions`; if not, assign and save (KI-006) — explain impact.
- [ ] `[Editor]` Set development identity (company, package id placeholder → dev id) — final values deferred to M7 (KI-004).
- [ ] `[Editor]` Verify Android SDK/NDK/OpenJDK paths in Preferences ▸ External Tools; record Android Player Settings (min/target SDK, orientation, architectures, graphics API, application identifier, version, safe-area setting). (Added 2026-11-08.)
- [ ] `[Approval]` Fresh-clone validation (TESTING_CHECKLIST B-05): clone repo, open in Unity, no missing refs — after Force Text commit.
- [ ] ~~`[Build]` Windows x64 development build; launch; reach menu; play one match.~~ **DEFERRED 2026-11-08** (Windows is out of first-release scope; optional Editor convenience only).
- [~] `[Build][Device]` Android development build (IL2CPP, ARM64); install on a **physical device**; reach menu; capture `adb logcat` of startup. (**Primary acceptance item.**) **PARTIAL, owner-reported 2026-11-08:** Development Build built, installed and ran on a OnePlus 12; the game ran smoothly and the player tank worked. Not recorded: Android version, build/commit id, startup `logcat` capture, IL2CPP/ARM64 confirmation. Keep open until those are recorded (TESTING_CHECKLIST A-01, B-04).
- [ ] (Start) Licensing evidence inventory for KI-007 — facts only; continues in M1. (Added 2026-11-08.)
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
Re-open scenes after reserialize (compare by playing a match); fresh clone test (B-05); Android install + startup `logcat` (A-01) on a physical device (Windows build no longer required); `git status` clean of generated folders.

## 10. Acceptance criteria
- Valid Build Settings; correct index 0.
- Force Text active; assets YAML; `.meta` visible.
- Repo exists with a baseline commit (owner-approved), generated folders ignored.
- Desert scene opens with console results recorded (Jungle/Moon deferred, not required).
- ~~Windows dev build reaches menu and completes a match;~~ (deferred, out of scope) Android dev build installs on a physical device and reaches the menu, with startup `logcat` recorded (or milestone → REVIEW REQUIRED if no device).
- Android Player Settings recorded from the Editor; SDK/NDK/JDK verified.
- Fresh-clone open validated.

## 11. Definition of Done
All acceptance criteria met with evidence in `DEVELOPMENT_LOG.md`; `KNOWN_ISSUES` KI-001/002/003 closed (VERIFIED FIXED); docs updated.

## 12. Completion summary
**Closed 2026-11-08 — COMPLETE on owner-reported acceptance.**

**Basis:** the owner reported that all remaining M0 validation tasks were completed and that all M0 acceptance checks passed. **Evidence quality: owner-reported only.** The owner supplied no per-task results, no Android version, no build/commit id, no build identifier and no `logcat` contents. Claude did not run, build, compile or independently verify any M0 check, and did not re-inspect the Editor, Build Profiles, serialization mode or Player Settings in this closure task.

**Individual task results (as recorded; the open `[ ]` boxes in §5 were not individually re-ticked because no per-task detail was supplied):**
| Item | Recorded result | Evidence quality |
|---|---|---|
| Android development build, install and run on a physical device | Built, installed and ran on a OnePlus 12; game ran smoothly; player tank worked (recorded earlier, 2026-11-08) | Owner-reported, PARTIAL detail (unchanged) |
| Desert 1 human vs 1 AI | Match ran; after the KI-P08 fix the bot moves and shoots, NRE gone (recorded earlier) | Owner-reported, PARTIAL (1v1 only, unchanged) |
| Git setup, baseline commit, `.gitignore` | Done 2026-11-07, verified via `git ls-remote` | Verified in that session |
| All other §5 items (Force Text/reserialize, Build Profiles, Desert scene console check, Project-wide Actions, dev identity, SDK/NDK/JDK + Player Settings recording, fresh-clone, licensing inventory start) and §10 acceptance criteria | Reported completed / passed by the owner | Owner-reported, no details; **not independently verified** |
| Windows x64 build | DEFERRED (out of scope), unchanged | — |

**Non-blocking items remaining (tracked separately, not claimed as verified):**
- Android OS version of the OnePlus 12, build/commit id, IL2CPP/ARM64 confirmation and startup `logcat` for the tested build are still **not recorded** (TESTING_CHECKLIST A-01, B-04 stay PARTIAL).
- Per-task evidence for the owner-reported items above (e.g. Force Text commit, Build Profiles Editor check, Player Settings values, fresh-clone result) is not in the repo docs; B-02, B-03, B-05, B-06 remain NOT TESTED in the checklist until the owner supplies details.
- Git LFS decision (PENDING). Git working tree still holds many uncommitted changes (see `NEXT_SESSION.md`).
- KI-001/002/003 "VERIFIED FIXED" status was not updated here (no evidence supplied); update when details are provided.
