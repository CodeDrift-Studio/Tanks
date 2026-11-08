# KNOWN ISSUES

Created 2026-11-06 from static inspection only. Nothing here was reproduced at runtime.
**2026-11-08 Android-first reconciliation:** scope is now Android-only (ADR-012). PC-only items are marked **DEFERRED (out of first-release scope)** and are not release blockers; they are retained for history. Findings from `RELEASE_GAP_ASSESSMENT.md` that are not already listed are added in §4 as **STATIC / UNVERIFIED** — none is runtime-confirmed. Original text of KI-001…KI-011 and KI-P01…P07 is otherwise unchanged. Milestone references use the new M0–M8 plan (old "M2/M3/M6/M7" mentions in older entries refer to the superseded plan; mapping in §5).
Severity: Critical / High / Medium / Low. Priority: P0–P3. Status: OPEN / INVESTIGATING / FIXED (needs validation) / VERIFIED FIXED / WONTFIX.
Evidence classes: **CONFIRMED (static)** = directly observed in files; **POTENTIAL** = inferred from code, unreproduced; **UNVERIFIED** = observation needing Editor/device check.

---

## 1. Confirmed defects / project-state problems (static)

### KI-001 — Build Settings reference a non-existent scene
- Severity: Critical · Priority: P0 · Status: OPEN · System: Build/Scene flow
- Evidence: `ProjectSettings/EditorBuildSettings.asset` (binary) contains `Assets/_Tanks/_Demo/Scenes/_Complete-Game Jungle.unity`; no `_Demo` folder exists; real scenes are in `Assets/_Tanks/Tutorial_Demo/Demo_Scenes/`. (Editor view of the list: UNVERIFIED.)
- Repro: open File ▸ Build Profiles ▸ Scene List.
- Scope update 2026-11-08 (DR-C14): the first release needs only Desert at index 0. STATIC observation of the working-tree `EditorBuildSettings.asset` (not Editor-verified; Editor-side edits happened during the task): latest read lists only Desert (enabled); an earlier read had Jungle and Moon present but disabled. The `Dev-Android` Build Profile does not override the global scene list. This satisfies the revised requirement. Status stays OPEN until confirmed in the Editor.
- Expected: valid scene(s); index 0 = game/menu scene. Actual (at baseline): stale path; `GameManager` (`LoadScene(0)`) and `PauseMenu` (reload by build index) depend on it.
- Proposed fix: re-add scenes via Editor; decide index 0 (M0).
- Validation: Windows + Android dev builds reach menu; finish a match → restarts correctly.

### KI-002 — No version control
- Severity: High · P0 · FIXED (2026-11-07) · System: Process
- Evidence (original): `git status` → not a repository; no `.gitignore`.
- Resolution: repo initialized on top of remote `0d9ef83`; baseline commit `784095d` on `dev` and `xyz-backup`; remote heads verified with `git ls-remote --heads origin`; staged set checked for generated folders and missing `.meta` (0). See `GIT_WORKFLOW.md` §2. **Not yet verified**: fresh-clone open in Unity (TESTING_CHECKLIST B-05). Baseline contains binary assets (KI-003).

### KI-003 — Assets serialized as binary
- **Update 2026-11-08 (DR-C20):** the text below describes `HEAD`. Observed read-only in the working tree: `EditorSettings.asset` `m_SerializationMode: 2` (Force Text); the Desert scene, `Demo_Tank.prefab` and `ProjectSettings.asset` are YAML, while `HEAD` holds binary versions; ~260 modified tracked files are reserialized and uncommitted. Owner decision: keep Force Text, trigger no further reserialization. KI-003 is **not** moved to VERIFIED FIXED (not committed, not fully verified).
- Severity: High · P0 · OPEN · System: Asset pipeline
- Evidence: all 154 `.prefab` files, the 4 `.unity` scenes, and `EditorSettings.asset`, `TagManager.asset`, `EditorBuildSettings.asset` are non-YAML (`head -c` shows no `%YAML`). `ProjectSettings.asset` and `.meta` files are text.
- Impact: unmergeable, undiffable in git; text tools must never touch them. Corrects the audit's "partially binary" wording (it is *mostly* binary).
- Fix: set Asset Serialization = Force Text and Version Control = Visible Meta Files in Editor, then reserialize all assets (approval needed — rewrites everything) (M0).
- Validation: files start with `%YAML`; scenes open identically; no missing refs.

### KI-004 — Placeholder identity settings
- Severity: Medium · P1 · OPEN · System: Release config
- Evidence: `companyName: DefaultCompany`; Android/iOS id `com.Company.ProductName`; version 1.0; default icon (UNVERIFIED).
- Fix: M0 (dev id) / M7 (final). Name/branding PENDING DECISION.

### KI-005 — No automated tests
- Severity: Medium · P2 · OPEN · System: QA
- Evidence: no test folders/asmdefs; Test Framework package present.
- Fix: introduce EditMode/PlayMode tests (M1 smoke, M6 regression).

### KI-006 — Project-wide Input Actions not persisted in ProjectSettings
- **M0 update 2026-11-06:** likely false alarm — Editor reports `InputSystem.actions = Tank_Actions` and `EditorBuildSettings` holds config key `com.unity.input.settings.actions`. Player-build behavior still UNVERIFIED. See `M0_BASELINE.md` §2, §6.
- Severity: High · P1 · OPEN (impact UNVERIFIED) · System: Input
- Evidence: `ProjectSettings/ProjectSettings.asset` has `preloadedAssets: []`; GUID of `Tank_Actions` (`7fb6c1c790fa04865b379a6e8ccd7e9c`) not found in ProjectSettings; `TankInputUser.Awake` does `InputSystem.actions.ToJson()` (NRE if null); editor script `ProjectWideActionsSetup` assigns at editor load only (`#if` Editor-only class).
- Expected: Tank_Actions assigned so player builds have it. Actual: assignment not visible in saved settings (Editor session state may differ).
- Validation: in Editor, Project Settings ▸ Input System Package shows Tank_Actions; a built player starts a match without NRE.

### KI-007 — Commercial licensing unresolved
- Severity: Critical (release blocker) · P0 for M7 · OPEN · System: Legal/Assets
- Evidence: `Assets/_Tanks/Tanks!_Third-PartyNotice.txt` says the asset is governed by the **Asset Store EULA**, with listed components (CC0 Blue Noise; OFL fonts Bowlby One, Big Shoulders Stencil, Exo 2, IBM Plex Mono, Inter, …; Ubuntu font licence) under their own licences. Applicability of Unity Companion License and EULA terms to commercial redistribution of models/audio/prefabs/code: **UNKNOWN — no legal conclusion made**.
- Fix: provenance table + owner review of EULA/licences; replace content if not permitted (M5/M7).

### KI-008 — Android touch controls not present in scenes (UNVERIFIED) — **now P0 for M1 (touch is mandatory); owner Editor check required (string search on binary scenes is inconclusive, assessment D6)**
- Severity: High · P1 · OPEN · System: Android input
- Evidence: string search for `MobileUIControl` in the three demo scenes found 0 hits (scenes are binary, so the search may be unreliable); `MobileControlCanvas.prefab` exists in `Prefabs/UI`.
- Validation: open each scene in the Editor and check hierarchy; test on device.

### KI-009 — Pre-release/unused packages
- Severity: Low · P3 · OPEN · Evidence: `com.unity.ai.assistant 2.21.0-pre.2`, Visual Scripting, Timeline, Multiplayer Center, AI inference unused by game code. ADR-007.

### KI-010 — PC gamepad appears unwired (STATIC inference) — **DEFERRED 2026-11-08: PC is out of first-release scope; not a release blocker**
- **M0 update 2026-11-06:** no `Gamepad.current` pairing anywhere in code (grep) → unwired CONFIRMED statically; runtime check pending.
- Severity: High · P1 · OPEN (UNVERIFIED) · System: Input (MVP requirement)
- Evidence: `Tank_Actions` defines `Gamepad` scheme (leftStick, buttonSouth, start). `TankMovement.Start` activates `Gamepad` only when `MobileUIControl` exists and ControlIndex==1; otherwise `KeyboardLeft`/`KeyboardRight`. `StartMenuSlot` offers only P1/P2/CPU. Gamepad UI navigation bindings exist.
- Validation: plug a gamepad on Windows; select P1; see whether it drives the tank.

### KI-011 — Template leftovers
- Severity: Low · P3 · `Assets/Scenes/SampleScene.unity`, `Assets/InputSystem_Actions.inputactions`, second URP asset set. Decide and clean in M3 (approval required before deletion).

---

## 2. Potential risks (code-reading only; not reproduced)

### KI-P01 — TankAI null-reference paths
- **Update 2026-11-08:** `TankAI.OnEnable` now clears target, path and AI state at every round start (GP-03, owner-verified). The mid-round paths listed below (lost target, `StartFleeing`/`FleeUpdate`/`FixedUpdate`) were not otherwise changed and stay open for M2/AI-01.
- Medium · P2 · System: AI
- Evidence: `TankAI.cs` — `StartFleeing` uses `m_CurrentTarget.position` (L326) and passes possibly-null `m_CurrentPath` to `NavMesh.CalculatePath`; `FixedUpdate` uses `m_CurrentTarget.position` when `!m_IsMoving` (L363); `FleeUpdate` reads `m_CurrentPath.corners`; `m_AllTanks` entries can be destroyed objects. `SeekUpdate` allocates `NavMeshPath` per target per repath.
- Repro: UNKNOWN. Validation: 100+ bot rounds in each arena, console clean; EditMode/PlayMode test for target-lost case.

### KI-P02 — Power-up effects may leak across death/round reset
- **STATUS 2026-11-08: FIXED (implemented, uncommitted) — owner-verified.** `PowerUpDetector` records what each effect applied and reverts it in `OnDisable`/`ResetPowerUps()` (speed, turn speed, cooldown, shield, invincibility, flag); `PowerUpHUD` clears all icons on disable; `TankShooting.OnEnable` clears transient shooting state. Not compiled or run by Claude. Regression list is in `TESTING_CHECKLIST.md` (P-07, G-10). Frozen baseline (DR-C16): reopen only with a reproducible issue.
- **M0 update 2026-11-06:** logic leak CONFIRMED by static reading (m_Speed, m_TurnSpeed, m_ShotCooldown not restored; m_HasActivePowerUp stuck true → pickups blocked). Runtime repro pending. See `M0_BASELINE.md` §7.
- Medium–High · P1 · System: Power-ups
- Evidence: `PowerUpDetector` applies effects by coroutine **on the tank** (`m_Speed += bonus`, `m_ShotCooldown *= reduction`) and reverts after `WaitForSeconds`. `TankHealth.OnDeath` and `TankManager.Reset` call `SetActive(false)`, which stops coroutines → revert never runs. `TankHealth.OnEnable` resets shield/invincibility, `TankShooting.OnEnable` resets special shell, but nothing resets `m_Speed`, `m_ShotCooldown` or `PowerUpDetector.m_HasActivePowerUp`. Possible effect: permanent speed/fire-rate boost and blocked further pickups after dying/round reset mid-effect.
- Validation: pick Speed, die/restart round, compare `m_Speed` and pickup eligibility in Inspector.

### KI-P03 — Input null risks on Android
- Medium · P2 · `TankInputUser.Awake` calls `InputUser.PerformPairingWithDevice(Keyboard.current)`; `Keyboard.current` can be null on touch devices. Related to KI-006. Validation: device log.

### KI-P04 — Setup assumptions in TankManager/GameManager
- **Update 2026-11-08:** the missing-`PowerUpDetector` null path in `PowerUp.OnTriggerEnter` now has a guard (GP-10). Other items here are unchanged.
- Low · P3 · `TankManager.Setup` needs a `Canvas` child (`GetComponentInChildren<Canvas>().gameObject`); `GameManager.Start` returns early when `MessageTextReference` is missing, leaving later `StartGame` to fail. `PowerUp.OnTriggerEnter` calls `GetComponent<PowerUpDetector>()` without null check.

### KI-P05 — Material instancing churn
- Low · P3 · `renderer.materials` accessed in loops (`TankManager.Setup`, `StartMenuSlot.SetTankPreview`) instantiates materials; colour relies on material **name** containing `TankColor`.

### KI-P06 — Pause/restart depends on build index and timeScale
- Medium · P2 · `PauseMenu` "select tank" reloads `GetActiveScene().buildIndex` and resets `Time.timeScale`; if a scene is not in Build Settings (KI-001) this fails in builds. `GameManager.LoadScene(0)` assumes index 0 is a valid game scene.

### KI-P07 — UI/platform
- Low–Medium · P2–P3 · No safe-area handling; `CanvasScaler.matchWidthOrHeight` rewritten each frame; `PauseMenu` quit button hidden in Editor/WebGL only; camera simulated in `FixedUpdate` (possible visual jitter — UNVERIFIED).

---

## 3. Unverified observations / open questions
- Whether the three scenes load without missing references or console errors (never opened by Claude).
- Whether `Assembly-CSharp.dll` in `Library/ScriptAssemblies` is current and warning-free (exists; not analysed).
- Whether Android SDK/JDK/NDK and a test device exist (UNKNOWN).
- Meaning of `AndroidTargetArchitectures: 6` (UNVERIFIED).
- Whether audio mixers (only `Demo_CompleteMainMix.mixer`) are used by scripts (no script references found; wiring in scenes UNVERIFIED) — relevant for volume settings in M3/M4.


---

## 4. Android-first additions from `RELEASE_GAP_ASSESSMENT.md` (2026-11-08) — STATIC / UNVERIFIED
None of these is runtime-confirmed. IDs continue the KI-P series. Milestones use the new M0–M8 plan.

### KI-P08 — AI target list may contain unassigned slots when fewer than 4 tanks play
- Medium–High · P1 · **FIXED (minimal fix implemented, uncommitted) — DEVICE-TESTED for the 1 human + 1 bot case only (owner-reported, 2026-11-08)** · System: AI · Planned: M1 (confirm) — done for 1v1; M2/AI-01 (harden) — still open
- Confirmed cause (static analysis + matching device symptoms): each arena scene's `GameManager.m_SpawnPoints` has 4 entries (Desert, Jungle, Moon checked in the scene YAML); `SpawnAllTanks` fills only `m_PlayerCount` of them, so in 1v1 entries 2–3 have a null `m_Instance`. `TankAI.Setup` copied all four into `m_AllTanks`, and `SeekUpdate` dereferenced `m_AllTanks[i].gameObject` before its `tank == null` check, throwing a `NullReferenceException` every 0.3–0.6 s. No target/path was ever assigned, so the bot neither moved nor shot. Observed on device (OnePlus 12 dev build, Desert arena): bot idle + `NullReferenceException … Tanks.Complete.TankAI.SeekUpdate ()` (IL2CPP, no line number in the trace). The exact faulting line was inferred from code analysis, not read from the trace. Not tried in the Editor.
- Fix (only `Assets/_Tanks/Scripts/Tank/TankAI.cs`): (1) `Setup()` builds `m_AllTanks` with `.Where(e => e.m_Instance != null)` before `.Select`; (2) `SeekUpdate()` uses the array entry directly and checks `tank == null || !tank.activeInHierarchy` before the self-check and any other use. No change to AI behavior, difficulty, scenes, prefabs or settings.
- Validation performed: owner-reported on OnePlus 12, Android Development Build with the fix, **in the Desert arena** (arena confirmed by owner 2026-11-08): AI opponent moves and shoots as expected; the `NullReferenceException` in `TankAI.SeekUpdate()` no longer reproduces in the tested build. Compile: no separate compile run by Claude (Unity MCP console showed 0 errors, not proof of recompile); the owner's successful build is the only compile evidence.
- Validation NOT performed (still pending): **Jungle and Moon: DEFERRED (DR-C14) and UNTESTED — not claimed broken, not claimed validated; not first-release blockers**; Desert-only cases still pending: 1 human + 2/3 bots and 2 humans + 1 bot (if the menu allows), multiple consecutive rounds, bot-death / player-death round ends, pause/resume and app backgrounding, long AI soak (100+ rounds), full `logcat` review, Editor 1v1 run. Tracked in `TESTING_CHECKLIST.md` (AI-01, AI-02, AI-07, A-07, G-08 [Desert]). Note: the fix is arena-independent code (it filters unused spawn slots); testing Jungle/Moon later is still required before they ship.
- Not fixed by this change (separate hardening, KI-P01 / M2 AI-01): `StartFleeing`/`FleeUpdate`/`FixedUpdate` null paths on a lost target; `m_CurrentTarget` not cleared when the target dies.
- Status can move to CLOSED once the pending cases above pass with evidence.

### KI-P09 — Bot and human input paths depend on a keyboard-paired InputUser
- Medium · P1 · STATIC, UNVERIFIED · System: Input/AI · M1
- Evidence: bots (ControlIndex −1) activate `KeyboardRight`; their `Fire` action is read via a keyboard binding; `TankInputUser.Awake` pairs `Keyboard.current` (null-risk, KI-P03). `TankMovement.FixedUpdate/Move/Turn` read `controlScheme.Value.name` for bots too.
- Validation: Android `logcat` of a 1v1 start.

### KI-P10 — TankMovement and TankAI both write the bot Rigidbody each FixedUpdate
- Low–Medium · P2 · STATIC, UNVERIFIED · System: AI/Physics · M2
- Evidence: `TankMovement.Move` sets `linearVelocity` from zero input while `TankAI.FixedUpdate` uses `MovePosition/MoveRotation`. Effect unknown.

### KI-P11 — Shell trigger has no layer/owner filter
- **Update 2026-11-08 (GP-10):** null guards added in `ShellExplosion` (missing `TankMovement`) and `PowerUp` (missing `PowerUpDetector`). The core concern is **not** changed: the shell still detonates on any trigger contact (possibly power-up pickup triggers); it depends on the layer collision matrix, which was not inspected. Needs a runtime check; no layer or collision design change was made (frozen, DR-C16).
- Low–Medium · P2 · STATIC, UNVERIFIED · System: Combat · M2
- Evidence: `ShellExplosion.OnTriggerEnter` detonates on any trigger contact; spawn overlap with the firing tank is possible. Self-damage by radius is by design.

### KI-P12 — Power-up flag leak is wider than speed/cooldown
- **STATUS 2026-11-08: FIXED together with KI-P02** (flag, HUD and effects cleared on disable); owner-verified; see KI-P02.
- Medium–High · P1 · STATIC (extends KI-P02) · M1
- Evidence: after death/reset, `m_HasActivePowerUp` stays true for Speed, Shooting, Shield, Invincibility and the special shell (`TankShooting.OnEnable` resets the shell but not the flag), blocking all pickups; HUD state also not cleared.

### KI-A01 — Android lifecycle, Back key, safe area, orientation not handled
- High · P1 · STATIC (grep: no `OnApplicationPause/Focus`, `safeArea`, `PlayerPrefs`, `targetFrameRate`) · M3 (lifecycle/back), M4 (safe area/orientation)
- Android Player Settings are unreadable statically (binary) -> UNKNOWN; record in M0.

### KI-A02 — Missing release screens and systems
- High · P1 · STATIC · M3/M4: no settings, no arena selection (**not required for the first release: Desert only, DR-C14**), results are title text only, no onboarding, no persistence.

### KI-A03 — Service integrations not present
- High · P1 (release requirement) · M5/M6: Firebase, GPGS, AdMob, Unity IAP / Play Billing do not exist in the project (manifest has none). Plan: `SERVICE_INTEGRATION_PLAN.md`.

### KI-A04 — Asset size / memory candidates (import settings not inspected)
- Medium · P2 · STATIC, partial · M8: ~12.7 MB uncompressed WAV music, five EXR lightmaps of 2.7–10 MB, 28 FBX, 42 textures; Android import overrides UNKNOWN.

### KI-A05 — Pre-release/unused packages and Unity Analytics module
- Low–Medium · P2 · STATIC · M5/M8 privacy and size review (ADR-007, KI-009).

### KI-A06 — ProjectSettings.asset is binary (contradicts KI-003 wording)
- **Update 2026-11-08:** in the working tree `ProjectSettings.asset` is YAML (the 'binary' statement holds for `HEAD`); see KI-003. Android values are still unrecorded until read.
- Medium · P1 · STATIC: header is binary; KI-003 states it is text. Android values (min/target SDK, orientation, architectures, identifier) are second-hand until read in the Editor (M0).

### KI-A07 — Are Jungle/Moon-only assets included in a Desert-only build? (OPEN INVESTIGATION)
- Low–Medium · P2 · UNKNOWN · opened 2026-11-08 (DR-C14) · M8 (optimization), with M7 provenance
- Context: the first release ships Desert only; Jungle and Moon scenes and assets remain in the repository. Unity normally builds only assets reachable from the enabled scenes (plus Resources, always-included shaders, and similar), but this has **not been verified** for this project: which assets are exclusive to Jungle/Moon, whether anything references them from Desert or from `Resources`, and the actual build-size effect are all UNKNOWN. Not analysed.
- Validation: after the first Desert-only release-candidate build, inspect the Editor build report (included assets) and compare with the Jungle/Moon asset sets; record findings and the licensing status of any Jungle/Moon-only assets that do ship.
- Not a first-release blocker by itself; becomes one only if uncleared (KI-007) or oversized assets ship unintentionally.

### KI-P13 — AI left flagged as charging after auto-fire at max charge (GP-08)
- Medium · P2 · **FIXED (implemented, uncommitted) — owner-verified 2026-11-08** · System: AI/Shooting
- Cause (static): `TankShooting.ComputerUpdate` fired at max charge but did not clear `m_IsCharging`, so the AI kept `IsCharging` true with minimum force, never started a new charge, and could later release an unintended minimum-force shell via `StopCharging`. Fix: `m_IsCharging = false` after that `Fire()`. Side effect: an AI that was previously stuck now keeps shooting.

### KI-P14 — Tank win-rate balance not established (OPEN, M2)
- Medium · P2 · System: Balance · frozen under DR-C16
- The initial balance pass (DR-C15) is accepted as the baseline, based on the owner's report that tanks perform much better. No matchup win rates, durations or damage data have been collected; the audit's model (static, idealized) predicts the residual ATV-vs-Heavy edge at full charge. `TankAI` ignores `m_ShotCooldown`, so cooldown tuning does not affect AI tanks. Validation method is in the 2026-11-08 balance audit (report in the conversation; not stored as a file). Do not retune until the freeze lifts.

### KI-A08 — Android multi-device test results pending (OPEN)
- Medium · P1 · UNKNOWN · opened 2026-11-08 · M1/M8
- A working build is confirmed (owner-reported) on the OnePlus 12 only. Testing on additional devices is IN PROGRESS; the report has not been received, and device models/Android versions are unrecorded. Any defect it reports will be filed here as a separate issue with severity. Not complete; gameplay freeze (DR-C16) depends on it.

## 5. Old-plan milestone mapping (for reading older entries)
Old M1 Gameplay Validation -> new M1 + M2 · old M2 Platform Input -> new M1 (Android input) / M3–M4 (lifecycle, UI) · old M3 Game Foundation -> new M3 · old M4 Polish -> new M4 · old M5 Original Content -> new M7 · old M6 Optimization/QA -> new M8 · old M7 Commercial Release -> new M7 (licensing/branding) + M8 (release) · services -> new M5/M6.
