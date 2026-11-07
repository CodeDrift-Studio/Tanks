# KNOWN ISSUES

Created 2026-11-06 from static inspection only. Nothing here was reproduced at runtime.
Severity: Critical / High / Medium / Low. Priority: P0–P3. Status: OPEN / INVESTIGATING / FIXED (needs validation) / VERIFIED FIXED / WONTFIX.
Evidence classes: **CONFIRMED (static)** = directly observed in files; **POTENTIAL** = inferred from code, unreproduced; **UNVERIFIED** = observation needing Editor/device check.

---

## 1. Confirmed defects / project-state problems (static)

### KI-001 — Build Settings reference a non-existent scene
- Severity: Critical · Priority: P0 · Status: OPEN · System: Build/Scene flow
- Evidence: `ProjectSettings/EditorBuildSettings.asset` (binary) contains `Assets/_Tanks/_Demo/Scenes/_Complete-Game Jungle.unity`; no `_Demo` folder exists; real scenes are in `Assets/_Tanks/Tutorial_Demo/Demo_Scenes/`. (Editor view of the list: UNVERIFIED.)
- Repro: open File ▸ Build Profiles ▸ Scene List.
- Expected: valid scene(s); index 0 = game/menu scene. Actual: stale path; `GameManager` (`LoadScene(0)`) and `PauseMenu` (reload by build index) depend on it.
- Proposed fix: re-add scenes via Editor; decide index 0 (M0).
- Validation: Windows + Android dev builds reach menu; finish a match → restarts correctly.

### KI-002 — No version control
- Severity: High · P0 · FIXED (2026-11-07) · System: Process
- Evidence (original): `git status` → not a repository; no `.gitignore`.
- Resolution: repo initialized on top of remote `0d9ef83`; baseline commit `784095d` on `dev` and `xyz-backup`; remote heads verified with `git ls-remote --heads origin`; staged set checked for generated folders and missing `.meta` (0). See `GIT_WORKFLOW.md` §2. **Not yet verified**: fresh-clone open in Unity (TESTING_CHECKLIST B-05). Baseline contains binary assets (KI-003).

### KI-003 — Assets serialized as binary
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

### KI-008 — Android touch controls not present in scenes (UNVERIFIED)
- Severity: High · P1 · OPEN · System: Android input
- Evidence: string search for `MobileUIControl` in the three demo scenes found 0 hits (scenes are binary, so the search may be unreliable); `MobileControlCanvas.prefab` exists in `Prefabs/UI`.
- Validation: open each scene in the Editor and check hierarchy; test on device.

### KI-009 — Pre-release/unused packages
- Severity: Low · P3 · OPEN · Evidence: `com.unity.ai.assistant 2.21.0-pre.2`, Visual Scripting, Timeline, Multiplayer Center, AI inference unused by game code. ADR-007.

### KI-010 — PC gamepad appears unwired (STATIC inference)
- **M0 update 2026-11-06:** no `Gamepad.current` pairing anywhere in code (grep) → unwired CONFIRMED statically; runtime check pending.
- Severity: High · P1 · OPEN (UNVERIFIED) · System: Input (MVP requirement)
- Evidence: `Tank_Actions` defines `Gamepad` scheme (leftStick, buttonSouth, start). `TankMovement.Start` activates `Gamepad` only when `MobileUIControl` exists and ControlIndex==1; otherwise `KeyboardLeft`/`KeyboardRight`. `StartMenuSlot` offers only P1/P2/CPU. Gamepad UI navigation bindings exist.
- Validation: plug a gamepad on Windows; select P1; see whether it drives the tank.

### KI-011 — Template leftovers
- Severity: Low · P3 · `Assets/Scenes/SampleScene.unity`, `Assets/InputSystem_Actions.inputactions`, second URP asset set. Decide and clean in M3 (approval required before deletion).

---

## 2. Potential risks (code-reading only; not reproduced)

### KI-P01 — TankAI null-reference paths
- Medium · P2 · System: AI
- Evidence: `TankAI.cs` — `StartFleeing` uses `m_CurrentTarget.position` (L326) and passes possibly-null `m_CurrentPath` to `NavMesh.CalculatePath`; `FixedUpdate` uses `m_CurrentTarget.position` when `!m_IsMoving` (L363); `FleeUpdate` reads `m_CurrentPath.corners`; `m_AllTanks` entries can be destroyed objects. `SeekUpdate` allocates `NavMeshPath` per target per repath.
- Repro: UNKNOWN. Validation: 100+ bot rounds in each arena, console clean; EditMode/PlayMode test for target-lost case.

### KI-P02 — Power-up effects may leak across death/round reset
- **M0 update 2026-11-06:** logic leak CONFIRMED by static reading (m_Speed, m_TurnSpeed, m_ShotCooldown not restored; m_HasActivePowerUp stuck true → pickups blocked). Runtime repro pending. See `M0_BASELINE.md` §7.
- Medium–High · P1 · System: Power-ups
- Evidence: `PowerUpDetector` applies effects by coroutine **on the tank** (`m_Speed += bonus`, `m_ShotCooldown *= reduction`) and reverts after `WaitForSeconds`. `TankHealth.OnDeath` and `TankManager.Reset` call `SetActive(false)`, which stops coroutines → revert never runs. `TankHealth.OnEnable` resets shield/invincibility, `TankShooting.OnEnable` resets special shell, but nothing resets `m_Speed`, `m_ShotCooldown` or `PowerUpDetector.m_HasActivePowerUp`. Possible effect: permanent speed/fire-rate boost and blocked further pickups after dying/round reset mid-effect.
- Validation: pick Speed, die/restart round, compare `m_Speed` and pickup eligibility in Inspector.

### KI-P03 — Input null risks on Android
- Medium · P2 · `TankInputUser.Awake` calls `InputUser.PerformPairingWithDevice(Keyboard.current)`; `Keyboard.current` can be null on touch devices. Related to KI-006. Validation: device log.

### KI-P04 — Setup assumptions in TankManager/GameManager
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
