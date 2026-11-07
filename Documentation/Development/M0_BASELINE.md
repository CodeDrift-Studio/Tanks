# M0 BASELINE (2026-11-06)

Evidence labels: **CONFIRMED** (observed this session via file inspection or a read-only Editor query), **STATIC** (read from code/files, not run), **UNKNOWN**, **ACTION** (needs user/Editor).
Nothing was compiled, built, or run by Claude this session. The only Editor interaction was one **read-only** query through the Unity MCP bridge (no setting, asset, or scene was modified).

## 1. Project facts
| Item | Value | Evidence |
|---|---|---|
| Project root | `D:\CodeDrift Studio\UW\Tanks` | CONFIRMED |
| Unity version | 6000.3.6f1 (`ProjectVersion.txt`; Editor reports `6000.3.6f1`) | CONFIRMED |
| Git | not a repo (`fatal: not a git repository`); git 2.56 is installed. Init DEFERRED by user | CONFIRMED |
| Asset serialization (Editor-reported) | **Mixed** → must become Force Text | CONFIRMED (Editor query) |
| Version Control mode | **Visible Meta Files** already (Editor query + `VersionControlSettings.asset` string) | CONFIRMED |
| `.meta` files | text; no asset under `Assets/` lacks a `.meta` | CONFIRMED (file scan) |
| Scenes/prefabs/EditorSettings/EditorBuildSettings/VersionControlSettings format | binary (no `%YAML` header on EditorSettings, EditorBuildSettings, InputManager, VersionControlSettings) | CONFIRMED |
| Project-wide Input Actions (Editor-reported) | `Assets/_Tanks/Settings/Tank_Actions.inputactions` | CONFIRMED (Editor query) |
| Scene files | `Assets/_Tanks/Tutorial_Demo/Demo_Scenes/Demo_Game_{Desert,Jungle,Moon}.unity` (+ `Assets/Scenes/SampleScene.unity` template) | CONFIRMED |
| Tests / asmdefs | none under Assets or Packages | CONFIRMED |
| `packages-lock.json` | present; deep consistency check not yet done | CONFIRMED / pending |

## 2. Discrepancies vs audit / KNOWN_ISSUES (repo wins, per CLAUDE.md §A)
1. **KI-001 differs between disk and Editor.** On disk, `EditorBuildSettings.asset` (modified 2026-10-06) holds the stale `Assets/_Tanks/_Demo/Scenes/_Complete-Game Jungle.unity`. The running Editor reports **1 scene: `Demo_Game_Jungle.unity` (valid path, exists)**. Either the Editor holds an unsaved/updated list or the file was rewritten after our read. Desert and Moon are **not** in the list either way. Treat Build Settings as needing the approved 3-scene order.
2. **KI-006 is probably a false alarm.** `preloadedAssets: []` is true, but Unity 6 stores project-wide actions in `EditorBuildSettings` config objects (key `com.unity.input.settings.actions` is present on disk) and the Editor reports `InputSystem.actions = Tank_Actions`. Residual risk: persisted-in-player-build is UNVERIFIED (needs a Windows/Android player run). Downgrade to "UNVERIFIED – likely OK".
3. **Visible Meta Files is already set**; only Force Text needs changing.
4. Currently open scene in the Editor is Demo_Game_Jungle; active build target is **Android**.

## 3. Development environment
| Component | Status | Evidence | Action Required |
|---|---|---|---|
| Unity Editor 6000.3.6f1 | Installed, **currently open on this project** (PID 40152) | `C:\Program Files\Unity\Hub\Editor\6000.3.6f1`; process list | none |
| Other Unity | 6000.0.66f2 installed (not used); `D:\Unity\Editor` has no `Unity.exe` | dir listing | none |
| Unity CLI | `unity.exe` (Hub CLI) found; **not used** (project is open, batch mode would lock-conflict) | `Get-Command` | Close Editor before any batch-mode run (needs user OK) |
| Unity MCP bridge | **Available** (RunCommand, GetConsoleLogs) | read-only probe succeeded | Using for read-only queries only; setting changes stay with the user |
| Windows build module | Installed | `BuildPipeline.IsBuildTargetSupported` = true; `windowsstandalonesupport` dir | none |
| Android build module | Installed (SDK/NDK/OpenJDK dirs exist under `AndroidPlayer`) | dir listing; supported = true | Verify SDK/NDK/JDK paths in Preferences ▸ External Tools (ACTION, not yet checked) |
| `adb` | `C:\ADB\platform-tools\adb.exe` found | `Get-Command` | none |
| Android device | **None connected** (`adb devices` list empty) | adb output | Connect device with USB debugging for device tests |
| JAVA_HOME / ANDROID_HOME env | JAVA_HOME → JRE 1.8 (not used by Unity, which uses its bundled OpenJDK); ANDROID_HOME unset | env | none unless Unity external-tools points elsewhere |
| Test Framework 1.6.0 | package present; no tests exist | manifest | M1 |
| Compilation status | **NOT TESTED by Claude.** Editor domain reloaded OK; console shows 0 errors, 1 unrelated warning (AI Assistant account API timeout) | console query | Full compile verification still requires an explicit compile run |
| Windows build | **NOT TESTED** | — | after Build Settings fixed |
| Android build | **NOT TESTED** (no device; toolchain looks present) | — | after Windows build |

## 4. Stage A–B readiness
- Backup: **no confirmation yet.** Force Text reserializes the whole project and git is deferred → need a user-made full-folder copy **outside** the project directory (CLAUDE/prompt requirement).
- Editor must have **no unsaved scene changes** before the change.
- Expected scope of reserialization: all `.unity` (4), `.prefab` (154), binary `.asset` files, plus `EditorSettings`, `EditorBuildSettings`, `TagManager`, `VersionControlSettings`, etc. `.meta` GUIDs must not change.

## 5. Scene-loading dependencies (STATIC, grep complete over `Assets/**/*.cs`)
| File | Call | Depends on |
|---|---|---|
| `Managers/GameManager.cs:179` | `SceneManager.LoadScene(0)` after a match is won | index 0 must be a playable arena (approved: Desert) |
| `UI/PauseMenu.cs:42` | `LoadScene(GetActiveScene().buildIndex)` ("select tank") | the active scene must be in Build Settings |
No name/path-based loads exist. With the approved order (0 Desert, 1 Jungle, 2 Moon) **every arena restarts into Desert after a won match** (LoadScene(0)) — behavior consequence of the approved decision, not a bug introduced here; flagged for M3 (ADR-011).

## 6. Input System investigation (STATIC)
Path: `Tank_Actions` → (Editor: `EditorBuildSettings` config object `com.unity.input.settings.actions`; `ProjectWideActionsSetup` is Editor-only fallback) → `InputSystem.actions` → `TankInputUser.Awake` clones via `ToJson/FromJson` → `TankMovement.Start` picks scheme → actions read in `Update`.

| ID | Location | Behavior | Status | Minimal correction | Validate |
|---|---|---|---|---|---|
| I-1 | `TankInputUser.Awake` L24 | `InputSystem.actions.ToJson()` NRE if the asset is not assigned in a *player*. | SUSPECTED (low; Editor shows assigned and the config key exists) | none if player run is clean; else null-guard + clear error | Run Windows player; reach match |
| I-2 | `TankInputUser.Awake` L28 | `PerformPairingWithDevice(Keyboard.current)`; `Keyboard.current` null on touch-only devices → invalid user (returns early in `SetNewInputUser`). | SUSPECTED | guard null | Android log |
| I-3 | `TankMovement.Start` L119–127 | Gamepad scheme only for the **mobile virtual device** (`MobileUIControl` present & ControlIndex 1). Otherwise `KeyboardLeft`/`KeyboardRight`. No physical `Gamepad.current` pairing anywhere (grep `Gamepad\.current` = 0). | **CONFIRMED (static) that PC gamepad is not wired;** runtime effect UNVERIFIED | M2 task: pair Gamepad device + activate `Gamepad` scheme (needs approval; architecture change) | Plug pad, select P1, try driving |
| I-4 | `TankMovement.FixedUpdate` L200 | `controlScheme.Value.name` throws if no scheme is active (e.g. invalid user from I-2). | SUSPECTED | guard `HasValue` | Android log |
| I-5 | `TankManager.Setup` L49 | `SetNewInputUser(m_InputUser)` passes a never-assigned default `InputUser` → returns early (`!valid`). Harmless today; both keyboards share `Keyboard.current` and are separated only by scheme/bindings. | CONFIRMED (static), benign | none | — |
| I-6 | init order | `TankInputUser.Awake` runs on instantiate, after the project-wide asset is loaded (Editor/player startup); order within scene is not an issue. Scheme activated in `Start`, actions enabled in `Start`. | STATIC: OK | — | — |
| I-7 | `GameUIHandler.cs:72` | `InputSystem.actions.FindAction("Pause").Clone()` — same null dependency as I-1. | SUSPECTED | — | — |

## 7. Power-up lifecycle (STATIC)
Effects run as coroutines on the **tank's own GameObject** (`PowerUpDetector.StartCoroutine`). Death (`TankHealth.OnDeath → SetActive(false)`) and round reset (`TankManager.Reset → SetActive(false/true)`) stop them, so the revert lines never run.

| Effect | State after interrupted effect | Reset on re-enable? | Result |
|---|---|---|---|
| Speed | `m_Speed += bonus`, `m_TurnSpeed += bonus` stay | No | **CONFIRMED (static): permanent boost, stacking per interrupted pickup** |
| ShootingBonus | `m_ShotCooldown *= factor` stays | No (`TankShooting.OnEnable` resets only launch force/special shell) | **CONFIRMED (static): permanent faster fire** |
| `m_HasActivePowerUp` | stays `true` for Speed/Shield/Shooting/Invincibility | No | **CONFIRMED (static): tank can no longer pick up power-ups** (`PowerUp.cs:55` gate) |
| Shield / Invincibility | flags reset in `TankHealth.OnEnable` | Yes | OK, but `m_HasActivePowerUp` still stuck |
| Special shell | reset in `TankShooting.OnEnable` | Yes (but `m_HasActivePowerUp` stays true) | partial |
| Healing | instant | n/a | OK |
Severity: Medium–High (affects balance and pickup availability across rounds). Runtime reproduction still pending (Inspector check: pick Speed, die, next round, read `m_Speed` and `m_HasActivePowerUp`). Fix = restore base values and clear flag in `PowerUpDetector.OnDisable` (small, but needs your approval to implement).

## 8. Open items requiring user action
1. Confirm independent full-folder backup (outside project).
2. Apply Force Text in Editor (steps in session message).
3. Apply Build Settings order in Editor.
4. Approve (or not) the two minimal code fixes later (power-up reset; input null guards).
