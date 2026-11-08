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

## 2026-11-07 — M0 — Git repository setup
- Files modified: created `Documentation/Development/GIT_WORKFLOW.md`, `.gitignore` (remote template + additions); updated `CLAUDE.md` (§C VCS row, §K, new §N), `KNOWN_ISSUES.md` (KI-002), `Milestones/M0_PROJECT_STABILIZATION.md`, `MILESTONE_STATUS.md`, `NEXT_SESSION.md`. No Unity assets, scripts, scenes, prefabs, settings or packages modified. `PROJECT_AUDIT.md` untouched.
- Summary: local folder was not a repo. Remote `main` held GitHub's `0d9ef83` (.gitignore + README), not empty. Initialized locally, fetched, built `dev` on `0d9ef83`, committed baseline `784095d` (1047 files; audited: no generated folders, 0 missing `.meta`), created `xyz-backup` from it, pushed `dev` and `xyz-backup`. `main` left at `0d9ef83`.
- Incident: first push ran before the commit succeeded (PowerShell here-string misuse with `git commit -F -`), so `dev`/`xyz-backup` were briefly pushed at `0d9ef83`; corrected by a fast-forward push to `784095d`. No history rewritten, no force-push. Baseline subject carries an invisible BOM (cosmetic, left as is).
- Tests executed: none for Unity (no compile/build/run). Git state verified with `git ls-remote --heads origin`.
- Decisions: Git workflow and branch model approved by owner (recorded in `GIT_WORKFLOW.md`); `main` intentionally kept at remote initial commit so the project reaches it only via PR. Git LFS still PENDING.


## 2026-11-08 — Pre-M0 — Static Android release gap assessment
- Files modified: created `Documentation/Development/RELEASE_GAP_ASSESSMENT.md` (only; untracked). No other file changed.
- Summary: assessment-only, static inspection of docs and C# scripts against the Android-only direction; no Editor, compile, build or device use. Findings classified as IMPLEMENTED — UNVERIFIED / NEEDS FIX (static) / NEEDS ANDROID TESTING / DECISION REQUIRED. Nothing is runtime-verified.
- Tests executed: none. Decisions: none.

## 2026-11-08 — Planning — Android-first roadmap revision (M0–M8)
- Branch: `feature/android-release-roadmap` created from `dev` @ `6de4c4a`; uncommitted. `dev` was in sync with `origin/dev`; the only pre-existing change was the untracked assessment report (left untouched).
- Files created: `DECISION_REGISTER.md`, `SERVICE_INTEGRATION_PLAN.md`, `Milestones/M1…M8_*.md` (new Android-first definitions), `Archive/2026-11-06_PC-oriented-roadmap/` (old M1–M7 files + original roadmap, unchanged content with a superseded banner).
- Files updated: `CLAUDE.md` (scope, stack, platform, safety rules for services/licensing, milestone references), `MILESTONE_ROADMAP.md` (rewritten M0–M8), `MILESTONE_STATUS.md` (M0 IN PROGRESS, M1–M8 NOT STARTED), `NEXT_SESSION.md`, `TECHNICAL_DECISIONS.md` (ADR-002 superseded, ADR-008/009 amended, ADR-012…016 added), `KNOWN_ISSUES.md` (PC items deferred, KI-P08…P12, KI-A01…A06, mapping), `TESTING_CHECKLIST.md` (Android-first, service categories), `Milestones/M0_PROJECT_STABILIZATION.md` (status alignment, Windows build deferred, Android SDK/settings/fresh-clone/licensing-inventory tasks added; acceptance otherwise preserved), this log.
- Preserved unchanged: `PROJECT_AUDIT.md`, `RELEASE_GAP_ASSESSMENT.md`, `M0_BASELINE.md`, `GIT_WORKFLOW.md`.
- Decisions recorded from the owner: Android-only; single-player vs AI; touch mandatory; F2P Ads + IAP; AdMob, IAP/Play Billing, Firebase, GPGS mandatory; online and shared-screen multiplayer excluded; Desert/Jungle/Moon proposed; git roles. Pending: see `DECISION_REGISTER.md` §B.
- Conflict recorded: the referenced "approved Android development plan" and "Reusable Mobile Game Project Skeleton" were not available in the repo or conversation; the task message's milestone specification was used.
- Tests executed: none. Nothing compiled, built or run. No Unity project file, script, scene, prefab, setting or package was modified. No commit, push, merge, stage or stash was performed.

## 2026-11-08 — M1 (early) — Android AI failure: root cause, fix, device check (KI-P08)
- Branch: `feature/android-release-roadmap` (HEAD `6de4c4a` + uncommitted working tree). Nothing staged, committed, pushed or merged.
- Trigger: first Android Development Build on a OnePlus 12 (owner-reported): game ran smoothly, player tank worked, AI tank did not move or shoot, `NullReferenceException: ... Tanks.Complete.TankAI.SeekUpdate ()` (IL2CPP, no line number).
- Investigation (static, no files changed): `GameManager.m_SpawnPoints` has 4 entries in each of Desert/Jungle/Moon; `SpawnAllTanks` fills only `m_PlayerCount` (2 in 1v1); `TankAI.Setup` copied all 4 into `m_AllTanks`; `SeekUpdate` read `m_AllTanks[i].gameObject` before its null check. Bot never got a target/path, so no movement and no shooting. Believed platform-independent (not tried in the Editor). Line-level attribution inferred from code; the trace has no line number.
- Fix: only `Assets/_Tanks/Scripts/Tank/TankAI.cs`. (1) `Setup()` filters `m_Instance != null`. (2) `SeekUpdate()` checks `tank == null || !tank.activeInHierarchy` before the self-check and before any other use of the entry. No change to AI behavior, difficulty, scenes, prefabs, settings or packages. Secondary null paths (KI-P01) deliberately not changed.
- Validation performed: owner-reported on OnePlus 12, Android Development Build with the fix: AI opponent moves and shoots as expected; the `NullReferenceException` in `TankAI.SeekUpdate()` no longer reproduces in the tested build. The build succeeding is the only compile evidence (Unity MCP console read after the edit showed 0 errors but that does not prove a recompile).
- Validation NOT performed: other arenas, other player counts, multi-round/round-end cases, pause/background, AI soak, full `logcat` review, Editor run. Android version, build number and commit id of the tested build were not provided. No `logcat` excerpt was supplied.
- Documents updated: `KNOWN_ISSUES.md` (KI-P08), `MILESTONE_STATUS.md`, `Milestones/M0_PROJECT_STABILIZATION.md`, `Milestones/M1_ANDROID_GAMEPLAY_VERTICAL_SLICE.md`, `TESTING_CHECKLIST.md`, `NEXT_SESSION.md`, this log.
- Decisions: none new. Early M1 start flagged for owner confirmation (M0 exit gate only partly evidenced).

## 2026-11-08 — Planning/Scope — First release narrowed to one arena (Desert), DR-C14
- Owner decision (confirmed): the first commercial Android release launches with **Desert only**. Jungle and Moon are deferred to a future content update; they are deferred content, not release blockers.
- Documentation only. Files updated: `DECISION_REGISTER.md` (DR-C14 added, DR-C11 superseded, DR-P12 resolved), `MILESTONE_ROADMAP.md`, `Milestones/M0`, `M1`, `M2`, `M3`, `M4`, `M7`, `M8`, `TECHNICAL_DECISIONS.md`, `KNOWN_ISSUES.md`, `TESTING_CHECKLIST.md`, `MILESTONE_STATUS.md`, `NEXT_SESSION.md`, `CLAUDE.md` (§B scope lines only), this log. No Unity scene, prefab, asset, setting, package, Build Profile or gameplay code changed. Nothing staged, committed, pushed or merged.
- Build Profiles review (STATIC, read-only): `ProjectSettings/EditorBuildSettings.asset` — an earlier read this session showed Desert (enabled, first), Jungle (disabled), Moon (disabled); a later read, after the Editor modified the file (not by this task), lists **only Desert (enabled)**. The `Dev-Android` Build Profile has `m_OverrideGlobalSceneList: 0`, so it uses that global list. Only Desert needs to be in the first-release build. The only scene-loading calls are `GameManager` `LoadScene(0)` (match end) and `PauseMenu` reload-by-active-build-index, both of which resolve to Desert in this configuration; there are no name-based scene loads. This was read from the file, not confirmed in the Editor, and the file was not modified by this task. Whether Jungle/Moon-only assets are excluded from the build output was not analysed.
- Test evidence recorded: AI movement and shooting tested on a OnePlus 12 in the Desert arena (owner-reported; KI-P08 fix). Jungle and Moon are untested and deferred; not claimed broken or validated.
- Scope conflicts remaining: see the end-of-task report and `NEXT_SESSION.md`.

## 2026-11-08 — Documentation correction — M1 status
- Owner instruction: M1 is **NOT STARTED** until M0 formally passes its exit gate. Corrected in `MILESTONE_STATUS.md` (M1 row, header date, new correction entry; earlier entries marked superseded), `Milestones/M1_ANDROID_GAMEPLAY_VERTICAL_SLICE.md` (status line) and `NEXT_SESSION.md`. The earlier log entry "M1 (early)" above is kept as history; its "early M1 start" wording is superseded by this correction.
- Preserved unchanged: the KI-P08 fix task (ticked in the M1 file), its owner-reported OnePlus 12 Desert 1v1 evidence, `KNOWN_ISSUES.md` KI-P08, `TESTING_CHECKLIST.md` PARTIAL rows (B-04, A-01, A-02, AI-01, AI-07).
- Also: `CLAUDE.md` product pillar and proposed-folder wording aligned with Desert-only (DR-C14); new KI-A07 opened for the open question of Jungle/Moon-only asset inclusion in a Desert-only build (M8 investigation). Documentation only; no Unity code, asset, scene, prefab, setting or package change. Nothing staged, committed, pushed or merged.

## 2026-11-08 — M0 closure and M1 formal start (documentation only)
- Branch: `feature/android-release-roadmap`; working tree already contained many unrelated modifications (docs, `TankAI.cs`, fonts/materials/prefabs/settings), preserved untouched. Nothing staged, committed, pushed or merged.
- Owner report: all remaining M0 validation tasks completed and all M0 acceptance checks passed. **No per-task results, Android version, commit id, build identifier or `logcat` contents were supplied**; none are invented here. Claude did not execute or independently verify anything.
- Status changes: **M0 IN PROGRESS → COMPLETE** (owner-reported). **M1 NOT STARTED → IN PROGRESS** (formal). M1 scope: Android only, Desert only, 1 human vs AI, touch controls, core combat/match flow, AI movement/targeting/navigation/shooting; no online or shared-screen multiplayer.
- Preserved: the earlier OnePlus 12 Android dev build and Desert 1v1 AI validation (PARTIAL detail) and the KI-P08 fix evidence (1 of 9 M1 tasks; no M1 acceptance criterion met; slice not complete).
- Non-blocking items kept separate (see `Milestones/M0_*` §12): Android version / build id / startup `logcat` unrecorded; per-task evidence for owner-reported items absent; Git LFS pending; KI-001/002/003 not moved to VERIFIED FIXED.
- Files updated: `MILESTONE_STATUS.md`, `Milestones/M0_PROJECT_STABILIZATION.md`, `Milestones/M1_ANDROID_GAMEPLAY_VERTICAL_SLICE.md`, `NEXT_SESSION.md`, `TESTING_CHECKLIST.md`, this log. No Unity code, asset, scene, prefab, setting or package changed.
- Tests executed: none by Claude.


## 2026-11-08 — M1/M2 — Gameplay stabilization, initial tank balance, baseline freeze (documentation task; earlier steps recorded here retroactively)
- Branch: `feature/android-release-roadmap`; large pre-existing uncommitted tree preserved. Nothing staged, committed, pushed, merged or stashed.
- Earlier steps of this day, recorded now (they had no log entry): (1) **round-reset fixes** — Claude edited `PowerUpDetector.cs`, `PowerUpHUD.cs`, `TankShooting.cs`, `TankAI.cs`, `ShellExplosion.cs`, `PowerUp.cs` (GP-01…GP-04, GP-08, GP-10 guards); (2) **four-tank balance audit** — read-only static audit, no files changed; (3) **initial balance pass** — Claude edited `Demo_Tank - ATV Variant.prefab` (damage 50→60, cooldown 0.25→0.45, radius 2→3) and `Demo_Tank - Heavy Variant.prefab` (health 300→260, speed 6→7, explicit overrides added for max damage 80 and radius 4). Read-back of all four variants matched the approved table; the diff against a pre-edit snapshot showed only those lines.
- Validation by Claude: static inspection only; read-back of prefab values; no Unity compile, Editor run or device test. Validation by owner (reported, no build/commit/device-OS/logcat supplied): core gameplay stable, AI movement and shooting verified, GP-01…GP-04 and GP-08 verified, GP-10 guards implemented, all six power-ups verified, tanks perform much better after the balance pass.
- Status recorded in this task: gameplay baseline FROZEN (DR-C16); initial tank balance accepted as baseline (DR-C15; approved stats in `DECISION_REGISTER.md`); Android multi-device testing IN PROGRESS / RESULTS PENDING (OnePlus 12 only confirmed); next development focus Firebase + GPGS, not started (DR-C17); AdMob and IAP remain M6. Win-rate balance NOT established.
- Documents updated: `DECISION_REGISTER.md`, `MILESTONE_STATUS.md`, `MILESTONE_ROADMAP.md`, `NEXT_SESSION.md`, `KNOWN_ISSUES.md` (KI-P01/P02/P04/P11/P12 notes; new KI-P13, KI-P14, KI-A08), `TESTING_CHECKLIST.md` (G-10, P-01…P-07 PARTIAL; new A-10), `Milestones/M1_*` (GP-01 task ticked), `Milestones/M2_*` (note), `Milestones/M5_*` (status note), this log. Historical files untouched: `PROJECT_AUDIT.md`, `RELEASE_GAP_ASSESSMENT.md`, `M0_BASELINE.md`, `GIT_WORKFLOW.md`, `SERVICE_INTEGRATION_PLAN.md`, `TECHNICAL_DECISIONS.md`, `CLAUDE.md`, Archive. No Unity code, asset, scene, prefab, setting or package changed by the documentation task.
- Known issues / open owner questions: see `NEXT_SESSION.md` conflicts 8–13.

## 2026-11-08 — Documentation — Resolution of conflicts 8–13 (owner decisions)
- Branch `feature/android-release-roadmap`; pre-existing uncommitted tree preserved; nothing staged, committed, pushed, merged, stashed or discarded.
- Owner decisions applied: (8) M5 may overlap M2–M4, which stay in the roadmap and are not automatic blockers (DR-C18); (9) AND-01 input guards exempt from the gameplay freeze, unrelated gameplay changes not authorized (DR-C19); (10) Google Play Games checklist IDs GP-01…GP-06 renamed PGS-01…PGS-06, gameplay IDs preserved (DR-C22); (11) commit grouping deferred (DR-P21); (12) Force Text kept, no further reserialization, working-tree YAML vs `HEAD` binary documented (DR-C20); (13) owner-reported validation stays PARTIAL until evidence is recorded (DR-C21).
- Read-only observations used for (12): `ProjectSettings/EditorSettings.asset` `m_SerializationMode: 2`; Desert scene, `Demo_Tank.prefab` and `ProjectSettings.asset` start with `%YAML` in the working tree and are binary in `HEAD`; ~260 modified tracked asset-type files in `git status`.
- Files updated: `DECISION_REGISTER.md`, `MILESTONE_ROADMAP.md`, `MILESTONE_STATUS.md`, `NEXT_SESSION.md`, `TESTING_CHECKLIST.md`, `KNOWN_ISSUES.md` (KI-003, KI-A06 notes), `Milestones/M1_*`, `Milestones/M5_*`, this log. Not changed: `CLAUDE.md` (its 'binary' wording reflects `HEAD`), `TECHNICAL_DECISIONS.md`, historical reports, any Unity script/prefab/scene/setting/package.
- Tests executed: none.

## 2026-11-08 — Git — Development checkpoint committed and pushed (owner-authorized)
- Branch `feature/android-release-roadmap` (remote `origin` = `https://github.com/CodeDrift-Studio/Tanks.git`; the branch did not exist on the remote before this push). No branch switch, merge, force-push, stash or reset.
- Commits (in order): `36f685d` gameplay stabilization and AI fixes (6 scripts); `f6da0d5` initial four-tank balance (4 variant prefabs); `af171d0` Force Text reserialization, project settings, package manifest, In-Game Debug Console plugin, SampleScene removal (363 files); a final documentation commit (see `git log`).
- Safety review before staging: no keystore/credential files in the tree; no generated folders staged; content scan found no keys or passwords. `ps4Passcode` in `ProjectSettings.asset` is Unity's standard template default, not a secret.
- **Deliberately NOT committed (left in the working tree):** `ProjectSettings/ProjectSettings.asset` and `Assets/Settings/Build Profiles/Dev-Android™.asset(.meta)`. The Unity-saved versions embed a local absolute keystore path (`AndroidKeystoreName`) and alias; the committed `HEAD` version of `ProjectSettings.asset` did not contain it. No password or keystore file was involved (the keystore lives outside the repo). Owner decision needed: commit as is, or clear/relocate the keystore setting first. Until then `HEAD` keeps the older binary `ProjectSettings.asset`. The folder-level `Assets/Settings/Build Profiles.meta` was committed without its folder (harmless GUID stub).
- Flagged for review: `Packages/manifest.json` changed in the Editor (Rider/Visual Studio IDE packages replaced by `com.unity.feature.development`) — a package change not separately approved in the record; the In-Game Debug Console plugin (third-party, upstream MIT per its project page — UNVERIFIED here; no licence file shipped in the folder) needs provenance recorded under KI-007 and a decision on keeping it out of release builds.

## 2026-11-08 — Git — .gitignore / .gitattributes applied; line-ending normalization (owner-authorized)
- Inputs: owner-supplied `.gitignore` (modified, uncommitted) and `.gitattributes` (untracked). Review findings: the new `.gitignore` had dropped `/UserSettings/`, `.vscode/`, `.utmp/`, `*.slnx` (Unity 6 solution), `*.log`, OS-junk and several Unity-generated patterns (they showed up as untracked); the new `.gitattributes` sent `*.asset` (now YAML under Force Text) and `.scenetemplate` (YAML) to Git LFS and had no uppercase `*.FBX` (41 tracked files) or font rules.
- Applied: all owner-supplied ignore rules kept (incl. the new signing/Firebase-secret block) with the dropped project rules appended; `.gitattributes` keeps the owner's text rules, adds `.overrideController/.playable/.mask/.spriteatlas/.mixer/.lighting/.scenetemplate`, sets `.asset` to `text=auto eol=lf` with explicit `binary` for `*LightingData*.asset` and `NavMesh*.asset`, and marks real binaries `binary`. **The LFS filter lines are disabled** (kept as a commented block): LFS 3.8.1 is installed and configured, but no file is in LFS and ~140 tracked binaries are plain blobs in history, so enabling the filter would be an LFS migration needing explicit approval (DR-P22). No history was rewritten.
- Verified (`git check-ignore` / `git check-attr`): Library, Temp, Obj, Logs, Builds (1.1 GB local test builds incl. APK/symbols, untracked), UserSettings, MemoryCaptures, IDE/OS files, keystores, passwords files, service-account JSON, scratchpads ignored; Assets, Packages, ProjectSettings, `.meta`, scenes, prefabs, the plugin `.aar`, docs not ignored; no tracked file matches an ignore rule.
- Line endings: a throwaway-index dry run showed the explicit `text eol=lf` rules would modify 26 already-tracked CRLF files (18 `.cs`, 7 `.meta`, 1 `.spriteatlas`/`PauseMenu.prefab`; 25 are the In-Game Debug Console plugin). They were committed as one EOL-only commit. `text=auto` files (docs, license `.txt`, `.asmdef`) were deliberately not renormalized.
- Still not committed: `ProjectSettings/ProjectSettings.asset` and `Assets/Settings/Build Profiles/Dev-Android™.asset` (local keystore path; see the earlier checkpoint entry).
