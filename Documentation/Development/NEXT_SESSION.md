# NEXT SESSION HANDOFF

Updated: 2026-11-08 (gameplay baseline freeze documentation session; earlier: M0 closure / M1 start session; earlier: KI-P08 AI-fix session, roadmap-revision session). Update this file at the end of every meaningful session.

## Current status (2026-11-08, gameplay baseline freeze) — READ FIRST
- **Gameplay baseline FROZEN (temporary)** — rules in `DECISION_REGISTER.md` DR-C16: no tank stat changes, no mechanic changes, no refactors of stable gameplay systems, no new gameplay features, no reopening verified fixes without a reproducible issue; a real critical defect is documented and severity-assessed before the baseline is touched. Not a project-wide freeze. Lifts only when the additional-device Android report arrives and the owner says so. **Exempt (DR-C19):** AND-01 Android input guards; this does not authorize unrelated gameplay changes, and the normal per-fix approval step still applies.
- **Stabilized (owner-reported verified; Claude did static inspection only, never compiled or ran anything):** core gameplay stable; AI movement and shooting; GP-01…GP-04 (power-up effects reverted in `PowerUpDetector.OnDisable`; `TankShooting.OnEnable` clears charging/fired/cooldown; `TankAI.OnEnable` clears target/path/state/timers; `PowerUpHUD` cleanup); GP-08 (AI no longer left flagged as charging after auto-fire at max charge); GP-10 null guards (`ShellExplosion`, `PowerUp`); all six power-ups. Files (all **uncommitted**): `PowerUpDetector.cs`, `PowerUpHUD.cs`, `TankShooting.cs`, `TankAI.cs`, `ShellExplosion.cs`, `PowerUp.cs`. Remaining GP-10 concern (shell trigger detonating on any trigger, e.g. power-up pickups) was not changed: KI-P11, runtime check outstanding.
- **Tank balance baseline (DR-C15):** initial pass accepted. Edited prefabs: `Demo_Tank - ATV Variant.prefab` and `Demo_Tank - Heavy Variant.prefab` (Heavy damage and radius are now explicit overrides); Shark and Medium already matched. Stats:

| Stat | ATV (Tank 1) | Shark (Tank 2) | Medium (Tank 3) | Heavy (Tank 4) |
|---|---:|---:|---:|---:|
| Health | 150 | 225 | 200 | 260 |
| Movement speed | 14 | 12 | 10 | 7 |
| Turn speed | 180 | 180 | 150 | 120 |
| Shot cooldown (s) | 0.45 | 1.0 | 0.85 | 1.25 |
| Maximum damage | 60 | 60 | 80 | 80 |
| Explosion radius | 3 | 6 | 3 | 4 |
| Explosion knockback | 10 | 40 | 15 | 25 |

  Win-rate balance across the six matchups is **not** established (no playtest data). Note: `TankAI` fires on its own fixed 2 s timer and ignores `m_ShotCooldown`, so cooldown differences do not apply to AI tanks; not changed (frozen, out of scope).
- **Android testing:** OnePlus 12 works (owner-reported). Additional devices: **IN PROGRESS / RESULTS PENDING** — report not received, device models/Android versions not supplied. Do not mark multi-device testing complete.
- **Next development focus: Firebase + GPGS (M5)** — not started, may proceed while testing continues (DR-C17). M5 may overlap M2–M4, which stay in the roadmap but are not automatic blockers (DR-C18). First step is the plan's verification task and the gating decisions (DR-P2 package ID, DR-P5/P6/P7 scope, DR-P13, DR-P15, package-install approval); do not invent the feature scope. AdMob and Unity IAP stay in M6.
- M1 task list: item (5) 'power-up lifecycle fix GP-01' below is **done**; other M1 items remain open (M1 is not complete).
- **Git:** branch `feature/android-release-roadmap`; large uncommitted working tree (scripts, tank prefabs, docs, many unrelated assets). Nothing staged, committed, pushed or merged by this task.
- **Git checkpoint (2026-11-08, owner-authorized):** the working tree was committed and pushed to `origin/feature/android-release-roadmap` (gameplay fixes, tank balance, Force Text reserialization + project config, docs). **Held back, still uncommitted:** `ProjectSettings/ProjectSettings.asset` and `Assets/Settings/Build Profiles/Dev-Android™.asset` — they embed a local keystore path; owner to decide how to commit them. Also review the `Packages/manifest.json` change and record provenance for `Assets/Plugins/IngameDebugConsole`. The earlier bullet above ('large uncommitted working tree') is historical.
- **Git config (2026-11-08):** `.gitignore` and `.gitattributes` are now committed (owner-supplied rules plus restored project rules; LFS lines disabled pending DR-P22; `.asset` stays diffable text). A separate EOL-only commit normalized 26 CRLF files to LF. `Builds/` (1.1 GB of local test builds) is ignored and stays local.

## Earlier status (2026-11-08, M0 closure / M1 start) — superseded in part by the block above
- **M0 — COMPLETE** (owner-reported: all remaining M0 validation tasks done, all acceptance checks passed). Evidence quality: owner-reported only; no Android version, build/commit id, `logcat` or per-task details supplied; Claude verified nothing. The earlier OnePlus 12 Android build and Desert 1v1 AI validation are preserved. Non-blocking leftovers are in `Milestones/M0_*` §12.
- **M1 — IN PROGRESS.** Scope: Android only, Desert only, one human vs AI, touch controls, core combat and match flow, AI movement/targeting/navigation/shooting; no online or shared-screen multiplayer. KI-P08 fix (1v1 Desert, OnePlus 12, owner-reported) is preserved as 1 of 9 tasks; the vertical slice is NOT complete.
- Sections below that say "M0 IN PROGRESS" or "M1 formally NOT STARTED", and the "Exact next actions (M0)" list, are **historical** and superseded by this block.
- **Next M1 actions (in order):** (1) baseline `logcat` of the build through menu → start → a few seconds of play, recording Android version and commit id this time; (2) Desert scene check of `MobileControlCanvas`/`MobileUIControl`, joystick, fire, pause, `m_SpawnPoints`, EventSystem module (KI-008); (3) Android input guards AND-01 (needs approval); (4) joystick + touch fire multi-touch on device; (5) power-up lifecycle fix GP-01 (needs approval); (6) full first-to-5 match vs one AI; (7) `logcat` review; (8) checklist rows; plus remaining KI-P08 regression cases (Desert).
- **Git:** still on `feature/android-release-roadmap`; large uncommitted working tree (docs, `TankAI.cs` fix, many modified assets). Nothing staged/committed/pushed/merged by this task. Keep the `TankAI.cs` fix in its own commit.

## Current project state
Unity 6.3 (6000.3.6f1) URP project containing Unity's Tanks! tutorial game. **No development work has begun.** Never compiled, built or run on Android (or anywhere) by Claude. Scenes/prefabs are binary-serialized (Force Text still pending). Build Settings: latest STATIC read (2026-11-08) lists only Desert (enabled); first release is Desert only (DR-C14); Editor confirmation pending. Git is set up (`main`/`dev`/`xyz-backup`; remote `https://github.com/CodeDrift-Studio/Tanks.git`).

## Scope decision 2026-11-08 — first release = Desert arena only (DR-C14)
Owner confirmed one arena for the first commercial release: **Desert**. **Jungle and Moon are deferred to a future content update** (code, scenes, assets kept; not release blockers; not tested, not known broken). Documentation updated: `DECISION_REGISTER.md` (DR-C14; DR-C11 superseded; DR-P12 resolved), `MILESTONE_ROADMAP.md`, M0/M1/M2/M3/M4/M7/M8 milestone files, `TECHNICAL_DECISIONS.md` (ADR-011/012 notes), `KNOWN_ISSUES.md`, `TESTING_CHECKLIST.md`, `MILESTONE_STATUS.md`, `CLAUDE.md` (§B scope lines). No arena-selection UI is required for the first release; keep the arena as data in the match config (M3) so more arenas can be added later. No Unity scene, Build Profile, prefab, setting or package was changed by this documentation task. Historical documents (`PROJECT_AUDIT.md`, `RELEASE_GAP_ASSESSMENT.md`, `M0_BASELINE.md`) still mention three arenas and were left unchanged.

## Earlier session — KI-P08 Android AI fix (code change, owner-reported device check)
First Android Development Build ran on a OnePlus 12; the bot did not move/shoot and threw `NullReferenceException` in `TankAI.SeekUpdate()`. Cause: `TankAI.m_AllTanks` contained null entries for unused spawn slots (4 spawn points, 2 tanks in 1v1). Fix in `Assets/_Tanks/Scripts/Tank/TankAI.cs` only (**uncommitted**, not staged): null-filter in `Setup()`, null check before dereference in `SeekUpdate()`. Owner reports (Desert arena, OnePlus 12): bot moves and shoots, NRE no longer reproduces (1 human + 1 bot). Details: `KNOWN_ISSUES.md` KI-P08, `DEVELOPMENT_LOG.md` 2026-11-08. M1 remains formally NOT STARTED until M0 passes its exit gate (this KI-P08 task was done ahead of the gate and is preserved); M0 remains IN PROGRESS.
**Still pending for KI-P08 (Desert only; Jungle/Moon deferred and untested):** 1 human + 2/3 bots, multi-round and round-end cases, pause/background, AI soak, `logcat` review (TESTING_CHECKLIST AI-01, AI-02, AI-07, A-07, G-08). Secondary AI null paths (KI-P01) intentionally unfixed. Also record the Android version, build/commit id and a startup `logcat` for the tested build (M0 task, A-01/B-04).
**Git:** the `TankAI.cs` change and these documentation edits sit in the working tree with many unrelated modified assets (fonts, materials, prefabs, scenes, settings) that were present before this session; keep the fix in its own commit.

## Roadmap revision (earlier session — documentation only)
Owner decisions (2026-11-08): **Android only; single-player vs AI; touch mandatory; Free-to-Play with Ads + IAP; AdMob, Unity IAP/Play Billing, Firebase and GPGS mandatory; online and shared-screen multiplayer excluded; Desert/Jungle/Moon proposed — later narrowed to Desert only, see below.** The roadmap is now **M0–M8** (`MILESTONE_ROADMAP.md`, `Milestones/M0…M8`). Old M1–M7 files and the old roadmap are archived (historical) in `Archive/2026-11-06_PC-oriented-roadmap/`. New documents: `DECISION_REGISTER.md`, `SERVICE_INTEGRATION_PLAN.md`. Updated: `CLAUDE.md`, `MILESTONE_ROADMAP.md`, `MILESTONE_STATUS.md`, `TECHNICAL_DECISIONS.md` (ADR-012…016), `KNOWN_ISSUES.md`, `TESTING_CHECKLIST.md` (Android-first), M0 milestone file, `DEVELOPMENT_LOG.md`. `PROJECT_AUDIT.md` and `RELEASE_GAP_ASSESSMENT.md` are unchanged historical records. No Unity project file, script, scene, prefab, setting or package was touched.

**Git state:** work is on `feature/android-release-roadmap` (created from `dev` at `6de4c4a`), **uncommitted**. `RELEASE_GAP_ASSESSMENT.md` is untracked. Nothing was staged, committed, pushed or merged. Owner decides when to commit.

## Active milestone
**M0 — Project Stabilization — IN PROGRESS.** Android dev build ran on a OnePlus 12 (owner-reported, partial evidence). The rest of the M0 owner action list below is still open. M1 is formally NOT STARTED (corrected 2026-11-08); the KI-P08 fix was done ahead of the gate. Finish and formally pass the M0 gate before opening M1.

## Exact next actions (M0)
Owner:
1. Confirm an independent full-folder backup outside the project directory.
2. Save all open scenes; set Edit ▸ Project Settings ▸ Editor ▸ Asset Serialization = **Force Text** (Visible Meta Files is already set). Commit the reserialization **alone** on `fix/force-text` after review (never mixed with other changes).
3. Confirm Build Profiles: **Desert at index 0, enabled** (first release is Desert only, DR-C14). Jungle/Moon may stay in the list disabled or be removed. The latest STATIC file read shows only Desert listed (and the `Dev-Android` profile uses the global list); confirm in the Editor. No other scene changes.
4. Connect an Android device (USB debugging); confirm SDK/NDK/OpenJDK paths in Preferences ▸ External Tools.
5. In the Editor read and tell Claude the Android Player Settings (min/target SDK, orientation, architectures, graphics API, application identifier, version, "render outside safe area"), because `ProjectSettings.asset` is binary.
6. Open the Desert scene (Jungle/Moon deferred): confirm whether `MobileControlCanvas` is placed, the `m_SpawnPoints` length, and Console errors (KI-008, KI-P08).
Claude (after owner actions): re-verify via read-only Editor query and `%YAML` headers; check GUID/meta stability; then the Android development build task and startup `logcat` capture (needs approval for any setting changes).

## Pending approvals / decisions
Full list: `DECISION_REGISTER.md` §B (DR-P1…P20). Most urgent for early milestones: **DR-P2 final package ID (before M5)**, DR-P13 audience/age (before consent design), DR-P11 licensing outcome and DR-P17 replacement budget, DR-P8/P10 minimum Android version and performance budgets, DR-P18 AI difficulty (DR-P12 arena count is resolved: Desert only). Technical: ADR-004/005/006/007/009/010/011/015, Git LFS.
Also: commit/merge of this documentation branch and of `RELEASE_GAP_ASSESSMENT.md`.

## Unresolved documentation conflicts (recorded, not silently reconciled)
1. The task referred to an "approved Tanks Android First Release Development Plan" and an "approved Reusable Mobile Game Project Skeleton — CodeDrift Studio" as inputs. **Neither document exists in the repository or in this session's conversation.** The M0–M8 structure was built from the milestone specification in the task message. If those documents exist, review them against `MILESTONE_ROADMAP.md` and the milestone files.
2. `RELEASE_GAP_ASSESSMENT.md` §21 proposed merging/renaming milestones differently (M0–M7); the owner structure M0–M8 overrides it. Its effort figure (95–165 days) is superseded by the roadmap table (it excluded the four mandatory services); the assessment file was left unchanged.
3. Assessment IDs (AND-xx, UI-xx, …) are reused in milestone files as traceability tags; they are not a separate task system.
4. `ProjectSettings.asset` binary vs KI-003 "text" wording (KI-A06). Android settings stay UNKNOWN until read in the Editor.
5. `M0_BASELINE.md` and older KI text still mention "Windows build"/old milestone numbers; they are historical. Mapping is in `KNOWN_ISSUES.md` §5.
6. The mandatory-but-unscoped services (e.g. Firebase Remote Config, GPGS achievements, cloud save) are recorded as PENDING, not assumed.
7. Statements about SDK behavior and Google policies in `SERVICE_INTEGRATION_PLAN.md` are general knowledge, UNVERIFIED for this project/version; each milestone begins with a verification task.
8. **[RESOLVED — DR-C18]** **M5 ordering:** owner directed Firebase + GPGS as the next focus (DR-C17) while the roadmap still says M5 follows M2–M4. Recorded as an overlap; please confirm ordering and whether M2–M4 stay planned before the first release.
9. **[RESOLVED — DR-C19]** **Freeze scope:** M1 still lists Android input guards (AND-01, a code change for Android robustness) and the multi-device report may demand fixes. Undecided whether these are exceptions to the gameplay freeze (DR-C16); until decided they are treated as needing explicit owner approval.
10. **[RESOLVED — DR-C22]** **ID collision:** the assessment's gameplay IDs (GP-01…GP-10) overlap with the Google Play Games rows GP-01…GP-06 in `TESTING_CHECKLIST.md` §GPGS. Meaning is by context; renaming the checklist rows was not done.
11. **[DEFERRED by owner — DR-P21]** **Commit grouping:** `TankAI.cs` now holds both the KI-P08 fix (earlier note: keep it in its own commit) and the GP-03 `OnEnable` reset. Owner to decide the split.
12. **[RESOLVED as documentation — DR-C20]** **Serialization:** the five tank prefabs in the working tree are text YAML, while `HEAD` holds binary versions (seen as 'Bin' in `git diff`), and `CLAUDE.md`/KI-003 still say 'binary'. Force Text therefore appears applied locally but is uncommitted and unrecorded; KI-003 and TESTING_CHECKLIST B-03 were not changed.
13. **[RESOLVED — DR-C21]** **Evidence level:** all gameplay, balance and Android results above are owner-reported without build/commit id, device OS version or `logcat`; checklist rows are PARTIAL, not PASS.

### Resolutions of conflicts 8–13 (owner decisions, 2026-11-08; items above kept as the historical record)
8. M5 (Firebase + GPGS) may overlap M2–M4; they stay in the roadmap and are not automatic blockers for M5 (DR-C18). M5's own gates (DR-P2, P5/P6/P7, P13, P15, P16, package-install approval) still apply.
9. AND-01 Android input guards are exempt from the gameplay freeze (DR-C19). Unrelated gameplay changes are not. Per-fix approval under M1 §8 is unchanged.
10. Google Play Games checklist rows are now PGS-01…PGS-06; gameplay finding IDs GP-01…GP-10 are preserved (DR-C22).
11. Commit grouping is deferred (DR-P21). Nothing is staged or committed; the `TankAI.cs` KI-P08 + GP-03 split is still undecided.
12. Force Text stays on; no extra reserialization (DR-C20). Accurate state: working tree `m_SerializationMode: 2` and YAML for the observed scene/prefab/`ProjectSettings.asset`; `HEAD` holds binary; ~260 modified files uncommitted. `CLAUDE.md` §G / KI-003 'binary' wording describes `HEAD` and was left unchanged. Side note: `ProjectSettings.asset` being YAML in the working tree means the Android Player Settings (KI-A06) may be readable from the file; not done.
13. Owner-reported results stay PARTIAL until build/commit id, device model + Android version and `logcat` are recorded (DR-C21).

## Known issues
`KNOWN_ISSUES.md` (KI-001…KI-011, KI-P01…KI-P12, KI-A01…KI-A06). All static/unverified except what is explicitly marked as repository-state fact.

## Testing requirements carried forward
Android physical-device testing is the primary plan (`TESTING_CHECKLIST.md`); every row is NOT TESTED. Never record PASS without date/commit/device/evidence.

## Recommended next step
Owner completes M0 Stage A actions above. Then: Android development build on a physical device and startup `logcat` capture. After M0 exit gate, M1 begins with a baseline `logcat` of the unmodified build before any code fix.
