# RELEASE GAP ASSESSMENT — Android-Only First Release

Assessment date: 2026-11-08 · Type: **assessment only** (no code, scene, prefab, setting, package or roadmap change was made)
Git at assessment: branch `dev`, HEAD `6de4c4a4ad96656ac061cd40b292fac72bba448a`, working tree clean, in sync with `origin/dev` (verified by `git status --short --branch`, `git rev-parse`, `git fetch`).

**Evidence method used for every finding below: STATIC INSPECTION ONLY** (source code, text docs, file listings, file headers). Not performed: Unity Editor inspection, compilation, Android build, Android runtime test, physical-device test. Consequently **nothing in this report is `VERIFIED WORKING`**. Wherever a finding says "NEEDS ANDROID TESTING" no device evidence exists. Labels: see §1 legend.

---

## 1. Executive Summary

- The repository is Unity's *Tanks!* tutorial project (Unity 6000.3.6f1, URP 17.3, Input System 1.18). It contains a complete-looking arcade loop (menu → spawn → rounds → first-to-5 → restart), AI, six power-ups, three arenas, a pause menu and an on-screen joystick prefab. It is a **demo, not a releasable product**.
- **Nothing has ever been compiled, built, or run on Android by the project record** (M0_BASELINE, DEVELOPMENT_LOG, TESTING_CHECKLIST: every row NOT TESTED; no Android device connected at baseline). M0 is IN PROGRESS and none of its acceptance criteria are met.
- **Distance to release is large.** The gameplay core is largely reusable. The gap is dominated by: (a) never-validated Android input/boot path, (b) a tutorial-grade front-end (no settings, no results screens, no persistence, no arena selection, no Android back/lifecycle handling, no safe area), (c) all identity/store/legal/compliance work, (d) no QA or performance baseline.
- Highest-risk single items (all static, unproven at runtime): Android input initialization may throw when no keyboard device exists (`TankInputUser.Awake`); touch controls are not confirmed present in any scene (KI-008); AI target list contains unassigned slots when fewer than 4 tanks play (new, §10/§11); power-up state leaks across death/round reset (KI-P02, statically confirmed); Build Settings and scene flow are not yet valid (KI-001); commercial licensing of the sample content is unresolved (KI-007).
- **Estimated remaining effort (developer-days, wide ranges, solo developer, excludes waiting time for stores/legal): ~95–165 days total; P0+P1 ≈ 70–120 days.** See §20. Effort is estimated from code size (≈3,000 LOC of scripts) and scope; it is not measured.
- Recommendation: keep M0 as the immediate priority, then add an Android-first device-validation gate before any M3/M4 investment (see §21).

## 2. Current Project State

| Item | State | Evidence class |
|---|---|---|
| Engine / render / input | Unity 6000.3.6f1, URP 17.3.0, Input System 1.18.0 (Input System handler only), uGUI + TMP | STATIC (`ProjectVersion.txt`, `Packages/manifest.json`) |
| Git | `dev` clean at `6de4c4a`; `main` = `0d9ef83` (initial GitHub commit) by design | VERIFIED (git commands, this session) |
| Serialization | Scenes/prefabs/key ProjectSettings are **binary** (Force Text not yet applied, KI-003, M0 pending) | STATIC (docs; `ProjectSettings/ProjectSettings.asset` header is binary — see Discrepancy D1) |
| Build Settings | Disk list stale (KI-001); Editor reported 1 scene (Jungle) at baseline; Desert(0)/Jungle(1)/Moon(2) decided but not applied | STATIC (M0_BASELINE §2) |
| Compilation | Never compiled by Claude; Editor domain reload OK and 0 console errors at baseline | per M0_BASELINE (not re-verified now) |
| Android toolchain | Android module + SDK/NDK/OpenJDK dirs exist under the Unity install; `adb` found; **no device connected**; SDK/NDK paths in Preferences not verified | per M0_BASELINE §3 (not re-verified now) |
| Tests | None (no asmdefs, no test files) | STATIC (repo scan) |
| Milestones | M0 IN PROGRESS (Stage A, waiting on user: backup + Force Text + Build Settings); M1–M7 NOT STARTED | `MILESTONE_STATUS.md` |
| Android build ever produced | **No evidence** | — |
| Android runtime test ever performed | **No evidence** | — |

### Documentation discrepancies (recorded, not reconciled)
| ID | Discrepancy | Handling |
|---|---|---|
| D1 | `KNOWN_ISSUES` KI-003 says `ProjectSettings.asset` is text. Its file header is now all-zero bytes (binary serialization); its Android values cannot be read by text tools. | Android player settings below are **UNKNOWN/UNVERIFIED** from this session; values quoted from `CLAUDE.md` §C are second-hand (prior session). |
| D2 | `M0_PROJECT_STABILIZATION.md` header says "Status: NOT STARTED"; `MILESTONE_STATUS.md` says IN PROGRESS. | Treated as IN PROGRESS (status file is newer). |
| D3 | `NEXT_SESSION.md` top section says "Not a git repo / git DEFERRED"; its own later "Git update" section supersedes it. | Treated as git initialized. |
| D4 | `KNOWN_ISSUES` KI-001 (disk shows stale path) vs M0_BASELINE (Editor shows 1 valid scene, Jungle). | Both recorded; Build Settings treated as invalid until M0 task completes. |
| D5 | `ADR-002`, `CLAUDE.md` §A/§B/§H and the whole roadmap include Windows/PC (keyboard, gamepad, 2-human) as MVP. The new direction (Android-only) is **not yet recorded as an ADR**. | Needs an owner-recorded decision (see PD-1). This report applies the new direction as instructed. |
| D6 | `KI-008` string search for the mobile canvas in scenes found 0 hits; scenes are binary, so GUID references are not greppable as text (this session re-checked the prefab GUID: 0 text hits in all three scenes). | Result is **inconclusive**, not evidence of absence. |
| D7 | Task prompt's power-up names (Healing, Speed, Shell, Shooting, Shield, Invincibility) vs code enum (`Healing, Speed, DamageMultiplier, ShootingBonus, DamageReduction, Invincibility`). | Mapped in §5. |

## 3. Updated Android-Only Product Scope (authoritative per task)

| Item | Decision | Notes |
|---|---|---|
| Android | IN SCOPE (primary) | |
| Windows / iOS | OUT | PC code stays untouched; PC items are NOT release blockers. Windows remains useful only as an Editor/dev convenience. |
| Android single-player vs AI | IN SCOPE | Initial gameplay target |
| Touch controls | IN SCOPE | |
| Android multiplayer (shared screen) | **DECISION REQUIRED** | Not assumed. Current code hides the P2 button on mobile (`StartMenuSlot.Awake`). |
| Online multiplayer, PC keyboard, PC gamepad | OUT | |
| Monetization (ads/IAP/premium) | **DECISION REQUIRED** | No approved decision in repo. |
| Number of arenas, final name, pricing, target devices/min Android version | **DECISION REQUIRED** | See §18. |

Impact on existing documents: ADR-002, CLAUDE.md §B/§H, M2 (PC input), parts of M0 (Windows build), M1 multiplayer rows, W-xx test rows, KI-010 (PC gamepad) become out-of-scope/deferred once the owner records the decision. This assessment does not edit them.

## 4. Existing Feature Inventory

Runtime Verified is **No** for every row (no Android or Editor play evidence exists).

| Feature | Implementation Found | Evidence | Runtime Verified | Status | Remaining Work |
|---|---|---|---|---|---|
| Tank movement/turn (tank-style + camera-relative "direct" mode) | `TankMovement` (Rigidbody velocity + MoveRotation) | STATIC | No | IMPLEMENTED — UNVERIFIED | Android feel tuning for virtual stick; null/scheme guards (§10) |
| Shooting, charge-to-fire, cooldown | `TankShooting` | STATIC | No | IMPLEMENTED — UNVERIFIED | Touch hold-to-charge UX test; fire button ergonomics |
| Shell, explosion, distance-scaled damage, knockback | `ShellExplosion` (OverlapSphere on "Players" layer) | STATIC | No | IMPLEMENTED — UNVERIFIED | Device test; pooling decision after profiling |
| Health, death, reset | `TankHealth`, `TankManager.Reset` | STATIC | No | IMPLEMENTED — UNVERIFIED | Power-up reset fixes (KI-P02) |
| Round loop, first-to-5, draw, restart | `GameManager.GameLoop` coroutines | STATIC | No | IMPLEMENTED — UNVERIFIED | Restart flow redesign (`LoadScene(0)`); score tracking beyond text |
| AI (seek/flee, NavMesh, shooting) | `TankAI` + baked NavMeshes | STATIC | No | PARTIALLY IMPLEMENTED | Null/slot bugs (§10), difficulty levels, soak testing |
| Six power-ups + spawner + HUD/FX | `PowerUp*`, `PowerUpDetector` | STATIC | No | PARTIALLY IMPLEMENTED | Lifecycle fix (KI-P02), balance, tests |
| Camera framing (ortho, multi-target) | `CameraControl` | STATIC | No | IMPLEMENTED — UNVERIFIED | Narrow/wide aspect checks; FixedUpdate camera jitter check |
| Start/match-setup menu (4 slots, P1/P2/CPU/Off) | `GameUIHandler`, `StartMenuSlot`, `Menus.prefab` | STATIC | No | PARTIALLY IMPLEMENTED | Replace with Android flow; touch sizing; arena choice absent |
| Pause menu (resume, controls screen, "select tank", quit) | `PauseMenu`, `PauseMenu.prefab`, on-screen pause button | STATIC | No | PARTIALLY IMPLEMENTED | Back button, lifecycle pause, settings entry |
| On-screen joystick + fire/pause buttons | `MobileUIControl`, `UIVirtualJoystick`, `MobileControlCanvas.prefab` | STATIC (prefab exists; scene placement **UNKNOWN**, D6) | No | PARTIALLY IMPLEMENTED | Confirm in scenes, device test |
| Gameplay HUD (health bar, aim bar, power-up HUD, round text) | World-space canvas per tank, `TankHealth`, `PowerUpHUD`, `MessageTextReference` | STATIC | No | PARTIALLY IMPLEMENTED | Mobile-size readability, safe area |
| Round results / match results screens | Only a centered title text with colored win counts | STATIC (`GameManager.EndMessage`) | No | NEEDS IMPLEMENTATION (as screens) | Dedicated screens with actions |
| Settings (audio/controls/quality) | None | STATIC (no `PlayerPrefs` anywhere) | n/a | NEEDS IMPLEMENTATION | Volume, haptics?, sensitivity, persistence |
| Arena selection | None (3 separate scenes; no selection UI; restart always `LoadScene(0)`) | STATIC | n/a | NEEDS IMPLEMENTATION | |
| Three arenas (Desert/Jungle/Moon) | Scenes + baked lighting/NavMesh | STATIC (files) | No | IMPLEMENTED — UNVERIFIED | Open/validate, mobile lightmap cost |
| Audio (music ×5, SFX ×7) | Clips present; only `Demo_CompleteMainMix.mixer`; no script uses mixer/music control | STATIC | No | PARTIALLY IMPLEMENTED | Volume control, mixing, import settings |
| Persistence / saves | None | STATIC | n/a | NEEDS IMPLEMENTATION | Settings/progress only if scoped |
| Tests | None | STATIC | n/a | NEEDS IMPLEMENTATION | |

## 5. Core Gameplay Gap Assessment

Everything here is static. Classification is the highest the evidence allows.

### 5.1 Controls and movement
| Area | Finding | Class |
|---|---|---|
| Movement/turning | Velocity set each FixedUpdate from input × `m_Speed`; turning via `MoveRotation`. Android virtual stick uses the *direct-control* (camera-relative) path because the scheme is `"Gamepad"`. This path calls `Camera.main.transform` every FixedUpdate (+`Vector3.Cross`, no allocation) — `Camera.main` null if the camera tag is missing. | IMPLEMENTED — UNVERIFIED; NEEDS ANDROID TESTING |
| Acceleration/feel | None: instant velocity, no acceleration curve. Feel on touch not assessable statically. | NEEDS ANDROID TESTING / GAMEPLAY BALANCING |
| Collision/arena bounds | Depends on scene colliders (binary scenes, not inspected). | NEEDS ANDROID TESTING (Editor inspection needed) |
| Touch responsiveness | `UIVirtualJoystick` (OnScreenControl, writes a Vector2 to a virtual gamepad stick). Fixed `joystickRange=50`, `*2.5` size-delta scaling, single joystick only. Multi-touch with fire button relies on EventSystem pointers (UNVERIFIED). | NEEDS ANDROID TESTING |
| Input isolation between tanks | Each tank clones the action asset (`TankInputUser`). Human = virtual device (ControlIndex 1); bots receive `KeyboardRight` scheme (ControlIndex −1 → else-branch). A bot's `Fire` action is therefore bound to Enter on a keyboard device; with no keyboard (typical on Android) the user pairing is invalid (see 5.1a). | NEEDS ANDROID TESTING |

**5.1a Android input initialization risk (STATIC, high priority, unverified):** `TankInputUser.Awake` calls `InputUser.PerformPairingWithDevice(Keyboard.current)` (null on a touch-only device is plausible but unproven), then `SetNewInputUser`; `TankMovement.FixedUpdate/Move/Turn` dereference `m_InputUser.InputUser.controlScheme.Value.name` (throws if no scheme/invalid user). Already logged as KI-P03 / M0_BASELINE I-2, I-4. `InputSystem.actions` null dependency (I-1, I-7) also unproven. Whether Unity's sample works on Android with these lines is **UNKNOWN**. → Task AND-01.

### 5.2 Shooting and combat
- Input: `Fire` action; human path charges while pressed, fires on release or at max charge; 1.0 s cooldown (reduced by Shooting power-up). Touch-hold semantics UNVERIFIED.
- Projectile: `Instantiate(shell)` per shot; velocity = charge × forward; lifetime 2 s; detonates on any trigger contact (`OnTriggerEnter` — no layer filter on `other`; includes the firing tank's own collider if the shell spawns inside it, UNVERIFIED).
- Damage: `(radius − distance)/radius × maxDamage` over all "Players" colliders in radius, including the shooter (self-damage possible, by design).
- Hit detection/destruction: OverlapSphere + `TankHealth.TakeDamage`; death = particles + audio + `SetActive(false)`.
- Class: IMPLEMENTED — UNVERIFIED for all rows. Tuning for touch (aim assist? auto-aim?) is a design **DECISION REQUIRED** (§18).

### 5.3 Health and lifecycle
- Health reset in `TankHealth.OnEnable`; round reset toggles `SetActive(false→true)`; death uses `SetActive(false)`. Consistent between human and AI at code level. `TankHealth.Awake` instantiates the death explosion prefab per tank (kept inactive).
- `OneTankLeft` checks `activeSelf` of spawned instances only (OK for fewer than 4 tanks). Simultaneous death → DRAW message path exists.
- Class: IMPLEMENTED — UNVERIFIED.

### 5.4 AI (`TankAI`)
| Topic | Finding |
|---|---|
| Target selection | Nearest by NavMesh path length among `m_AllTanks`, repath every 0.3–0.6 s; retarget implicit. |
| Navigation | NavMesh path corners driven with `Rigidbody.MovePosition/MoveRotation` in AI `FixedUpdate`. `TankMovement.FixedUpdate` also still runs for bots (sets `linearVelocity` from zero input each step) — interaction between the two writers is UNVERIFIED. |
| Shooting decisions | Charge when within max range and navmesh-raycast clear; release when projected landing ≈ target; 2 s own cooldown; flee if target static >2 s. No leading of moving targets, no difficulty parameters. |
| **Null-reference risks** | (a) **NEW, STATIC:** `GameManager` calls `TankAI.Setup` with `manager.m_SpawnPoints.Select(e => e.m_Instance)`. `m_SpawnPoints` has one entry per spawn slot (4 per CLAUDE.md §D), but only `m_PlayerCount` instances are created, so a 2-tank match leaves unassigned (null) entries. `SeekUpdate` reads `m_AllTanks[i].gameObject` **before** its `tank == null` check, so a null entry would throw. Whether this surfaces (e.g. null vs Unity fake-null behaviour, spawn-point count in each scene) is UNVERIFIED — it needs Editor inspection of `m_SpawnPoints` length and a play test. This is exactly the Android default case (1 human + 1 bot). (b) `StartFleeing` uses `m_CurrentTarget` / `m_CurrentPath` that may be null (KI-P01). (c) `FleeUpdate` reads `m_CurrentPath.corners` unguarded. |
| Allocation | `new NavMeshPath[]` + `new NavMeshPath()` per target per repath (KI-P01). |
| Difficulty/consistency | None; behaviour fixed. |
Class: PARTIALLY IMPLEMENTED; NEEDS FIX is **not** claimed because no runtime failure was observed.

### 5.5 Power-ups (all six; code names → task names)
Pickup: `PowerUp.OnTriggerEnter` — layer "Players", `GetComponent<PowerUpDetector>()` without null check, gate `!m_HasActivePowerUp`, then destroys pickup and notifies spawner (20 s respawn, random type, fixed Y 1.09).

| Task name (enum) | Activation / duration / expiry | Cleanup on death or reset | Interaction | Assessment |
|---|---|---|---|---|
| Healing (`Healing`) | Instant `IncreaseHealth(20)`; HUD 1 s. Sets no flag. | Nothing to clean. | Can be picked while another effect is active? Gate blocks it (flag true). | IMPLEMENTED — UNVERIFIED |
| Speed (`Speed`) | `m_Speed += 5` for 5 s via coroutine on the tank. | **Confirmed by reading:** deactivation stops the coroutine, so the subtraction never runs → permanent +5 speed, stacking per interrupted pickup; `m_HasActivePowerUp` stays true. | Blocks others. | **NEEDS FIX (static confirmation, KI-P02) — runtime repro pending** |
| Shell (`DamageMultiplier`) | Special shell ×2 damage for next shot; flag cleared on `Fire`. | `m_HasSpecialShell` reset in `TankShooting.OnEnable`, but `m_HasActivePowerUp` is **not** → pickups blocked next round; HUD state not cleared. | Blocks others until a shot is fired. | **NEEDS FIX (static, partial KI-P02)** |
| Shooting (`ShootingBonus`) | `m_ShotCooldown *= 0.5` for 5 s. | Same leak: cooldown stays halved (and compounds on repeat), flag stuck. | Blocks others. | **NEEDS FIX (static, KI-P02)** |
| Shield (`DamageReduction`) | `ToggleShield(0.5)` for 5 s. | `TankHealth.OnEnable` resets shield; flag stays stuck. | `PickUpShield` has an extra `!m_HasShield` guard but pickup is consumed regardless. | **NEEDS FIX (flag)** |
| Invincibility (`Invincibility`) | Toggle invincible for 5 s. | `OnEnable` resets; flag stuck. Toggle-based design: any double-toggle path would invert state (not currently reachable). | Blocks others. | **NEEDS FIX (flag)** |

Cross-cutting: the whole group is one root cause (state held in tank coroutines but cleared inconsistently on `OnEnable`), plus no tests. KI-P02 reassessed: **severity stays Medium–High; for a 1v1 vs bot game the leak is more frequent because rounds reset constantly.** The fix is small but is code change → out of this assessment. Task GP-01.

### 5.6 Match and round system
| Topic | Finding |
|---|---|
| Match/round init | `StartGame(PlayerData[])` spawns tanks at spawn points, sets camera targets, runs `GameLoop`. Failure paths: `GameManager.Start` returns early if the title text is missing (then `StartGame` would fail on `m_TitleText`). |
| Win/loss | Last active tank wins the round; `m_Wins++`; game ends when `m_Wins == m_NumRoundsToWin` (5). `==` rather than `>=` works only because wins increment by 1. Draws award no score (infinite-loop-free but unbounded round count). |
| Transitions | Fixed 3 s start / 3 s end waits; text-only. |
| Match completion/restart | `SceneManager.LoadScene(0)` — returns to build index 0 which, per the approved order, is the **Desert** scene, regardless of the arena played. No separate menu scene (M3 ADR-011 proposal). Pause "select tank" reloads `GetActiveScene().buildIndex` and resets `timeScale`. Both break if the scene is not in Build Settings (KI-001). |
| Score tracking | Wins only, text-formatted; no persistence, no per-match stats. |
| State reset | Tanks reset via active toggle; **pickups/spawners are not reset between rounds** (existing pickups persist; respawn timers continue) — design decision. Shells in flight at round end are not cleared (short lifetime; UNVERIFIED). |
| `Time.timeScale` | Set to 0 in pause; reset only by TogglePause and "select tank". App backgrounding on Android is unhandled (§6). |
Class: IMPLEMENTED — UNVERIFIED; flow redesign required for a commercial game (§12).

## 6. Android Compatibility Assessment (highest priority)

### 6.1 Android input
| Topic | Finding | Class |
|---|---|---|
| Virtual joystick | `UIVirtualJoystick : OnScreenControl` writing to a Vector2 control path; `MobileUIControl` finds the `OnScreenControl` in children and exposes its **device**; auto-disables itself off-mobile (`Application.isMobilePlatform`). | IMPLEMENTED — UNVERIFIED |
| Presence in scenes | `MobileControlCanvas.prefab` exists; placement in Desert/Jungle/Moon **unknown** (D6, KI-008). If absent, the human tank falls into the keyboard branch and is **uncontrollable on a phone**. | NEEDS ANDROID TESTING (Editor inspection first) |
| Fire / pause | `GameUIHandler` references an `OnScreenButton` for pause (emulates Gamepad Start). A fire button is presumably inside the mobile canvas (not inspected). Single-touch conflict with joystick UNVERIFIED. | NEEDS ANDROID TESTING |
| Action init | See 5.1a. `InputSystem.actions` project-wide assignment: Editor-reported assigned; player persistence UNVERIFIED (KI-006 likely false alarm). | NEEDS ANDROID TESTING |
| Null risks | `Keyboard.current` pairing; `controlScheme.Value`; `Camera.main`; `FindAnyObjectByType<MobileUIControl>` result; `GetComponentInChildren<Canvas>()` in `TankManager.Setup`. | NEEDS FIX candidates pending device log |
| UI interaction | EventSystem/InputSystemUIInputModule not inspected (scene). | NEEDS ANDROID TESTING |
| Haptics, sensitivity settings, left-handed layout | None | NEEDS IMPLEMENTATION (P2) |

### 6.2 UI and display
| Topic | Finding |
|---|---|
| Canvas scaling | `CanvasScaler.matchWidthOrHeight` rewritten every `Update` (0 in portrait, 1 in landscape). Reference resolution not verified. |
| Safe area / notches / cutouts | **None in code** (`Screen.safeArea` not referenced). Whether Android is allowed to render under the display cutout is a Player setting (UNKNOWN, D1). → ANDROID COMPATIBILITY task AND-02. |
| Orientation | Player setting UNKNOWN (D1). Code comments say the game is landscape only but tolerates portrait. Needs an explicit decision and lock. |
| Aspect ratios | Camera size fit uses `x/aspect`; ultra-wide and 4:3 tablets unverified. |
| Readability, touch targets | Menu uses a fixed 4-slot layout designed for desktop; target sizes unmeasured. NEEDS ANDROID TESTING. |
| Pause/menu usability | On-screen pause button exists; no Android Back-key handling (no `Escape`/back reference except the `Pause` action's `<Keyboard>/escape` binding, which may or may not receive the Android back key — UNVERIFIED). No `OnApplicationPause/Focus` handler, so the match keeps running if the app is backgrounded (Time.timeScale not set). |

### 6.3 Android project configuration (values are UNVERIFIED unless noted)
| Setting | Value / state | Source |
|---|---|---|
| Build target | Editor active target Android at baseline | M0_BASELINE §2 (not re-verified) |
| Min SDK / Target SDK | 25 / "auto" (0) | CLAUDE.md §C (second-hand, UNVERIFIED) |
| Scripting backend / arch | IL2CPP; architectures flag `6` (meaning UNVERIFIED — may or may not equal ARM64 only) | CLAUDE.md §C |
| API compatibility level | UNKNOWN | binary file |
| Graphics API (Vulkan/GLES3) | UNKNOWN | binary file |
| Orientation / safe-area render | UNKNOWN | binary file |
| Application identifier / company / version | `com.Company.ProductName` / `DefaultCompany` / 1.0 — placeholders (KI-004); Android version code UNKNOWN | CLAUDE.md §C, KI-004 |
| Permissions / manifest | No `AndroidManifest.xml`, no `Assets/Plugins`, no `.aar`, no keystore in the repo (grep). Permissions would be Unity-generated; Internet permission necessity **UNKNOWN** until a build is inspected (the `unitywebrequest`, `unityanalytics` modules, `ai.assistant`, `ai.inference` packages are present; whether they add INTERNET or analytics is unverified). | STATIC |
| Packages | Release-relevant: pre-release `com.unity.ai.assistant 2.21.0-pre.2` (editor tooling; remove/confirm before release, ADR-007), `ai.inference`, Visual Scripting, Timeline, Multiplayer Center, Analytics module → size/privacy/review needed. | `manifest.json` |
| Build pipeline | Gradle/IL2CPP/SDK/NDK/JDK dirs exist under Unity; env `JAVA_HOME` points at JRE 1.8 (Unity uses its own); `adb` found; no device. APK/AAB build ever succeeded: **No evidence**. Unity Test Framework installed (1.6.0), no tests. | M0_BASELINE §3 |

### 6.4 Performance and stability (code review only; no numbers measured)
| Risk | Evidence |
|---|---|
| GC | `TankAI.SeekUpdate` allocates `NavMeshPath` objects every 0.3–0.6 s per bot; per-shot `Instantiate` of shell + explosion particle objects with `Destroy`; death explosion instantiated per tank in `Awake`; string concatenation for UI text is per round only; `HumanUpdate` allocation-free. No pooling (policy: add only after profiling). |
| Materials | `renderer.materials` used in `TankManager.Setup` and `StartMenuSlot.SetTankPreview` — instantiates materials (one-time per spawn; moderate). Tank colour selected by matching material names. |
| Updates | `PowerUp.Update` rotates every pickup each frame (trivial). Camera simulated in `FixedUpdate` → possible visual stutter on devices where render/physics rates differ (UNVERIFIED). |
| Physics | Rigidbody per tank, OverlapSphere per shell: low risk for ≤4 tanks. |
| AI cost | A NavMesh path per other tank per repath; fine for 1–3 bots. |
| Memory-heavy assets | `Art` 17 MB, `Audio` 24 MB (incl. `Music_Classic.wav` ≈12.7 MB uncompressed WAV), `Tutorial_Demo` 38 MB incl. five EXR lightmaps (≈8–10 MB, ≈2.7–10 MB each) per the lighting data folder. Import compression settings **not inspected**. 42 textures, 28 FBX, 10 audio clips (counts from file scan). |
| Thermal / frame rate | No `Application.targetFrameRate` or quality-tier control in scripts; URP has Mobile_RPAsset/Renderer (separate from PC) — which asset the Android quality level uses is UNVERIFIED. Real thermal behaviour requires a 10-min device session (TESTING_CHECKLIST A-07). |
| Loading | Synchronous `LoadScene`; no loading UI. |
Budgets (FPS, memory, APK size) are not defined (ADR-009 PENDING).

## 7. UI/UX Assessment

| Screen | Exists | Connected to flow | Functional | Touch-suitable | Polished | Needs |
|---|---|---|---|---|---|---|
| 1 Main Menu | Partly: the 4-slot start menu doubles as main menu; no title/logo/play screen | Yes (`GameUIHandler`) | UNVERIFIED | UNVERIFIED (small slot buttons likely) | Tutorial-grade | NEW IMPLEMENTATION + design |
| 2 Match Setup | The same slot screen (P1/P2/CPU/Off per slot) | Yes | UNVERIFIED | Needs Android-specific simplification (single human, pick # of bots, difficulty) | Tutorial-grade | UI/UX redesign — **DECISION REQUIRED** |
| 3 Arena Selection | **No** (arena is whichever scene is loaded) | No | n/a | n/a | n/a | NEW IMPLEMENTATION |
| 4 Gameplay HUD | World-space health/aim bars per tank, power-up HUD, title text | Yes | UNVERIFIED | Readability on 5–6" screens UNVERIFIED | Tutorial-grade | Screen-space HUD, scores, power-up timers, safe area |
| 5 Pause Menu | Yes (resume via same button, controls, "select tank", quit) | Yes | UNVERIFIED | UNVERIFIED | Basic | Back button, resume/restart/quit wording, settings link |
| 6 Round Results | Text only ("X WINS THE ROUND" + win counts) | Yes | UNVERIFIED | n/a | Minimal | NEW IMPLEMENTATION |
| 7 Match Results | Text only ("X WINS THE GAME!") then auto reload of scene 0 | Yes | UNVERIFIED | n/a | Minimal | NEW IMPLEMENTATION (rematch / menu / next arena) |
| 8 Settings | **No** | No | n/a | n/a | n/a | NEW IMPLEMENTATION |

Other:
- First-time experience, instructions, tutorial: only a "controls" screen in the pause menu (desktop-oriented controls image, UNVERIFIED for mobile). **No onboarding.** NEEDS IMPLEMENTATION.
- Audio feedback: engine, shot, explosion, pickup SFX and music clips exist; music playback source/volume management not found in scripts (UNVERIFIED in scenes). No UI click sounds found.
- Visual feedback: explosions, dust trails, power-up FX present; no damage numbers, hit flash or screen shake.
- Transitions: none (hard scene reload).
- Error states: only `Debug.LogError` messages; no player-facing error handling.
- Navigation consistency/back button: not handled (§6.2).
- The demo UI should **not** be treated as sufficient for release (per task rule).

## 8. Performance and Stability Assessment (summary)
- Stability is unknown: no runtime log of any kind exists in the project record for any platform.
- Known statically-suspect crash sites: §5.1a (input), §5.4 (AI null), `PowerUp.OnTriggerEnter` (no null check on `PowerUpDetector`), `TankManager.Setup` requiring a child Canvas, `GameManager.Start` early return.
- Lifecycle: no handling of app pause/resume, interruptions (calls), screen rotation, low-memory, or audio focus.
- Required evidence not yet available: frame time, memory, thermal state, APK size, load time, GC per frame, crash rate. All need a device (§13).

## 9. Commercial Release Requirements

Technical vs business/product tagged **[T]** / **[B]**. No legal conclusions are made.

| Item | Status in repo | Type |
|---|---|---|
| Final game name | PENDING (project name "Tanks") | [B] |
| Branding / visual identity / logo | None; sample art & fonts in use | [B]+content |
| App icon | Default Unity icon presumed (UNVERIFIED) | [T]+content |
| Splash screen | Unity default presumed (UNVERIFIED; Unity Personal/Pro splash rules need owner review) | [T]/[B] |
| Store listing: screenshots, feature graphic, promo video, short/long description, category, rating questionnaire | None | [B]+content |
| Privacy policy / Data Safety form | None; data collection not assessed in-device. Candidates to review: Unity Analytics module, AI packages, any default Unity services (`UnityConnectSettings.asset` exists, contents not inspected), ads/IAP if later chosen. | [B]/legal |
| Third-party SDK/dependency review | Packages listed in §6.3; none are ad/IAP/analytics SDKs beyond Unity modules | [T] |
| Asset/font licensing | `Tanks!_Third-PartyNotice.txt` (Asset Store EULA; CC0 Blue Noise; OFL fonts Bowlby One, Big Shoulders Stencil, Exo 2, IBM Plex Mono, Inter…). Applicability of the Asset Store EULA / Unity Companion License to commercial redistribution of models, audio, prefabs and code: **UNKNOWN — documents requiring owner/legal review: the Asset Store EULA terms, the Unity Learn/Tanks sample terms, `Documentation.txt`, the third-party notice, and licences of all audio** | LEGAL / COMPLIANCE |
| Unity sample project licensing | Same as above (KI-007; ADR-001 assumes reuse) | LEGAL |
| Application signing & keystore | No keystore (correctly not in repo); signing plan and Play App Signing enrollment not defined | [T]/[B] |
| Versioning | `bundleVersion 1.0` / version code UNKNOWN; scheme undefined | [T] |
| Release AAB (required by Google Play for new apps) / APK for testing | Never built; target API level must meet the current Google Play requirement at submission time (**to be checked against Google Play's then-current policy; not verified here**). 64-bit (ARM64) requirement noted in CLAUDE.md; arch flag `6` unverified. | [T] |
| Google Play requirements | Developer account, content rating, target audience (children?) declaration, ads declaration, Data Safety, app-access info, closed-test requirements for new personal accounts (**verify current policy**) | [B] |
| Internal / closed testing | None planned in repo | [T]/[B] |
| Crash reporting / monitoring | None (Play Console vitals only unless a crash SDK is chosen; adding an SDK is a privacy/dependency decision) | [T]/[B] |
| Monetization | Undecided (no ads/IAP/pricing ADR) | [B] DECISION REQUIRED |

## 10. Confirmed Defects
"Confirmed" = directly observed in source or files (static). None reproduced at runtime.

| ID | Defect | Evidence |
|---|---|---|
| CD-1 | Power-up effects (Speed, Shooting) leak permanently when the tank is deactivated mid-effect; `m_HasActivePowerUp` stays true for Speed/Shooting/Shield/Invincibility/Shell, blocking all further pickups | `PowerUpDetector.cs` coroutines on the tank; `TankHealth.OnEnable`/`TankShooting.OnEnable` do not restore speed, cooldown or the flag; matches KI-P02 / M0_BASELINE §7 |
| CD-2 | Build Settings invalid / scene flow dependent on build index 0; with the approved order every arena restarts into Desert | KI-001, M0_BASELINE §5 |
| CD-3 | No version of the shipped game has a settings screen, results screens, arena selection, persistence, app-lifecycle or Back-key handling | grep: no `PlayerPrefs`, `OnApplicationPause/Focus`, `safeArea`, `targetFrameRate` |
| CD-4 | Placeholder identity (company, package id) and binary-serialized assets | KI-004, KI-003 |
| CD-5 | Template leftovers present (`SampleScene`, `InputSystem_Actions`) | KI-011 |

## 11. Suspected Risks
| ID | Risk | Evidence class |
|---|---|---|
| SR-1 | Android input init throws when `Keyboard.current` is null / invalid `InputUser` scheme (KI-P03, I-2/I-4) | STATIC, high impact |
| SR-2 | Touch controls not placed in any scene (KI-008) | UNVERIFIED (binary scenes) |
| SR-3 | **AI `m_AllTanks` contains unassigned slots in matches with fewer than 4 tanks; `SeekUpdate` dereferences `.gameObject` before the null check** (new; see §5.4) | STATIC, needs Editor + play confirmation |
| SR-4 | Bot `Fire` action bound to a keyboard; bot shooting also depends on the same keyboard-paired user | STATIC |
| SR-5 | TankMovement and TankAI both write the bot's Rigidbody each FixedUpdate | STATIC, UNVERIFIED effect |
| SR-6 | Other AI null paths: `StartFleeing`, `FleeUpdate` (KI-P01) | STATIC |
| SR-7 | Project-wide Input Actions not present in a *player* build (KI-006, downgraded) | UNVERIFIED |
| SR-8 | `GameManager.Start` early return / `TankManager.Setup` missing-canvas / `PowerUp` missing detector (KI-P04) | STATIC |
| SR-9 | Large uncompressed audio and lightmaps → APK size / memory (import settings not inspected) | STATIC, partial |
| SR-10 | Shell trigger without layer filter → self-hit / premature detonation | STATIC, UNVERIFIED |
| SR-11 | Camera in FixedUpdate → jitter on high-refresh devices (KI-P07) | UNVERIFIED |
| SR-12 | UI built for desktop slot layout; small touch targets; no safe area | STATIC |
| SR-13 | Licensing of sample content may require replacing models/audio/UI (KI-007) | UNKNOWN |
| SR-14 | Pre-release/unused packages and Analytics module may add size or data-collection obligations | STATIC |

## 12. Missing Features (needed for a commercial Android game)
Settings (audio, controls, graphics), results screens, arena selection, difficulty levels, persistent settings, app-lifecycle (pause on background), Back-button handling, safe-area handling, orientation lock, onboarding/tutorial, proper main menu and branding, loading/transition screens, localization decision, accessibility basics (UI scale, colour contrast), crash/diagnostic logging strategy, test suite, store assets, privacy policy, signing/versioning/AAB pipeline, original or cleared content (M5). *Not* missing by decision: PC input, online play.

## 13. Testing and Validation Gaps
- No Editor play evidence, no compile verification by Claude in this session, no automated tests, no device, no Android build.
- All TESTING_CHECKLIST rows are NOT TESTED; rows tied to Windows (W-xx), PC gamepad (W-04), two-human and 4-player (M-01…M-03) become out of scope; A-01…A-07, G-xx, AI-xx, P-xx, U-xx, PF-xx remain required.
- Needs a **physical Android device matrix** (at least one low-end, one mid-range, one tall-aspect/notched phone, one tablet), `adb logcat` sessions, Unity Profiler on device, Android Studio/Play pre-launch report.

## 14. Release Blockers (P0)

| ID | Title | Work type | Status | Depends on | Effort (days) |
|---|---|---|---|---|---|
| M0-* | Finish M0: backup, Force Text, valid Build Settings, scene open/log audit | TESTING / BUG FIX | IN PROGRESS | owner actions | 2–4 (M0 existing tasks) |
| AND-00 | First Android development build installs and reaches the menu (M0 Android task) | ANDROID COMPATIBILITY | NEEDS ANDROID TESTING | M0, device, SDK paths | 1–3 |
| AND-01 | Android input path works end-to-end: touch moves/fires a human tank vs a bot with a clean `logcat` (covers SR-1, SR-2, SR-4, SR-7, KI-008, KI-P03, I-1…I-7) | ANDROID COMPATIBILITY / BUG FIX | NEEDS ANDROID TESTING | AND-00 | 3–6 |
| AI-01 | Fix AI null/slot risks (SR-3, SR-6, KI-P01) and 100+ round bot soak per arena | BUG FIX / TESTING | SUSPECTED | AND-00 | 2–4 |
| GP-01 | Power-up lifecycle fix and tests (CD-1, KI-P02) | BUG FIX | NEEDS FIX | M1 | 1–2 |
| FLOW-01 | Valid scene/match flow: menu → match → results → rematch/menu/arena, no `LoadScene(0)` dependency (CD-2, KI-P06) | NEW IMPLEMENTATION | NEEDS IMPLEMENTATION | M0, PD-3 | 4–8 |
| LEG-01 | Resolve licensing of Tanks! sample content and all fonts/audio; replace anything not cleared (KI-007) | LEGAL / COMPLIANCE + CONTENT | DECISION REQUIRED | owner/legal | 3–40 (depends on outcome; replacement is the large case) |
| ID-01 | Final name, package id, company, signing/keystore plan, version scheme (KI-004) | PRODUCT DECISION + config | DECISION REQUIRED | PD-2 | 1–3 |
| PERF-01 | On-device profiling baseline vs agreed budget (min device, FPS, memory, APK size); fix blockers found | OPTIMIZATION / TESTING | NEEDS ANDROID TESTING | AND-00, PD budget | 4–10 |
| REL-01 | Signed release AAB that installs from a Play internal-testing track | TESTING / ANDROID COMPATIBILITY | NEEDS IMPLEMENTATION | ID-01 | 2–4 |

## 15. Required Before Release (P1)

| ID | Title | Work type | Status | Effort (days) | Acceptance (summary) |
|---|---|---|---|---|---|
| UI-01 | Android main menu + match setup (single human vs N bots, difficulty), touch-sized | UI/UX / NEW IMPL. | NEEDS IMPLEMENTATION | 6–10 | Playable from cold start with one thumb; ≥48 dp targets |
| UI-02 | Arena selection screen (Desert/Jungle/Moon) | NEW IMPL. | NEEDS IMPLEMENTATION | 2–4 | Choice persists to match |
| UI-03 | Round results + match results screens (rematch, menu, next) | UI/UX | NEEDS IMPLEMENTATION | 4–7 | Replace title-text-only flow |
| UI-04 | Settings screen + persistence (volume, music, SFX, controls options) | NEW IMPL. | NEEDS IMPLEMENTATION | 4–7 | Values survive relaunch (ADR-010) |
| UI-05 | Gameplay HUD for phones (screen-space, power-up timers, scores) | UI/UX | PARTIALLY IMPLEMENTED | 4–8 | Readable on small screens |
| AND-02 | App lifecycle: pause on background/interruption; Back-key behavior; safe-area; orientation lock | ANDROID COMPATIBILITY | NEEDS IMPLEMENTATION | 3–6 | No state loss; no overlap of cutouts |
| AND-03 | Touch control tuning: joystick dead-zone/range, fire/charge ergonomics, optional aim assist (design decision), layout for different screens | GAMEPLAY BALANCING / UI/UX | NEEDS ANDROID TESTING | 4–8 | Playtest sign-off on ≥3 devices |
| GP-02 | Gameplay validation of all systems on device (M1 checklist: G-01…G-10, AI-xx, P-xx) and fixes | TESTING / BUG FIX | NEEDS ANDROID TESTING | 8–15 | Checklist PASS with evidence |
| GP-03 | Balance pass: round count, damage, AI difficulty tiers, power-up values for 1v1 and up to 4 | GAMEPLAY BALANCING | NEEDS IMPLEMENTATION | 4–8 | Playtest notes |
| AUD-01 | Audio: music/SFX mixing, compression import settings, UI sounds, mixer volume hooks | OPTIMIZATION / CONTENT | PARTIALLY IMPLEMENTED | 3–6 | APK-size and loudness checks |
| OPT-01 | Mobile asset optimization: texture/lightmap/audio import settings, quality tier, frame cap, material churn, AI path allocations | OPTIMIZATION | NEEDS IMPLEMENTATION | 5–10 | Meets agreed budget |
| QA-01 | EditMode/PlayMode regression tests (power-ups, scoring, AI target selection) + smoke test | TESTING | NEEDS IMPLEMENTATION | 5–9 | Tests execute green |
| QA-02 | Device test matrix execution + soak (thermal, memory, 10 matches) + `logcat` clean | TESTING | NEEDS ANDROID TESTING | 5–10 | Documented evidence |
| BRAND-01 | Original identity: name, logo, icon, splash, menu art | CONTENT | DECISION REQUIRED | 5–12 | Owner sign-off |
| STORE-01 | Store assets, listing text, privacy policy, Data Safety, content rating | LEGAL / COMPLIANCE / CONTENT | NEEDS IMPLEMENTATION | 5–10 | Passes Play Console review checks |
| PKG-01 | Package review: remove unused/pre-release packages if approved (ADR-007), minimal permissions, Unity services/analytics stance | ANDROID COMPATIBILITY | PENDING APPROVAL | 1–3 | Verified manifest from build |
| MON-01 | Crash/diagnostic strategy (Play vitals vs SDK) | PRODUCT DECISION + impl. | DECISION REQUIRED | 1–3 | Decision recorded |

## 16. Recommended Improvements (P2)
Onboarding/tutorial tooltips (UI-06, 3–6 d); first-run controls overlay for touch; hit feedback/screen shake/vibration (2–4 d); pooling of shells/FX if profiling shows GC (2–4 d); camera interpolation fix (0.5–1 d); AI improvements (target leading, difficulty tiers) (3–6 d); tank stats polish; left-handed layout and control size options (2–4 d); localization framework (3–6 d if scoped); accessibility (colour-blind-safe tank colours, UI scale) (2–4 d); remove template leftovers (KI-011) (0.5 d, needs approval); Force-Text clean diffs (done in M0).

## 17. Deferred / Optional (P3)
Additional tank types/mechanics (M5 optional), extra arenas beyond three, online/Android shared-screen multiplayer (if later approved), achievements/leaderboards, cloud save, ads/IAP/battle pass (not assumed), WebGL leftovers, PC/iOS ports, Timeline/Visual Scripting package cleanup (unless P1 PKG-01), PC gamepad (KI-010).

## 18. Product Decisions Required
| ID | Decision | Marked |
|---|---|---|
| PD-1 | Record Android-only scope as an ADR (supersedes ADR-002 / CLAUDE.md §B) and decide what happens to Windows in the repo | DECISION REQUIRED |
| PD-2 | Final game name, company/developer account, package id | DECISION REQUIRED |
| PD-3 | Menu/scene architecture: boot → menu → arena scenes (ADR-011), folder migration (ADR-004) | DECISION REQUIRED |
| PD-4 | Android shared-screen multiplayer in release 1 or later? (default: no) | DECISION REQUIRED |
| PD-5 | Monetization: premium / free + ads / IAP / none | DECISION REQUIRED |
| PD-6 | Performance budget: min Android version, min device class, target FPS, max APK/AAB size, memory | DECISION REQUIRED (ADR-009) |
| PD-7 | Orientation (landscape-only?) and tablet support | DECISION REQUIRED |
| PD-8 | Number of arenas, round length (first to 5?), AI difficulty tiers, aim-assist on touch | DECISION REQUIRED |
| PD-9 | Licensing policy: reuse vs replace sample content; who reviews legal terms | DECISION REQUIRED |
| PD-10 | Analytics/crash reporting/Unity services stance; privacy posture; audience (kids?) | DECISION REQUIRED |
| PD-11 | Settings storage (PlayerPrefs vs JSON) — ADR-010; ScriptableObject config — ADR-006; namespace — ADR-005 | PROPOSED, needs approval |
| PD-12 | Release channel: Google Play only? other stores? | DECISION REQUIRED |
| PD-13 | Package cleanup (ADR-007), Git LFS (M0) | PENDING |

## 19. Dependency Map
```text
M0 stabilize (backup → Force Text → Build Settings → scene audit → Android dev build [AND-00])
   → AND-01 input path ──► GP-02 gameplay validation ──► (AI-01, GP-01 fixes) ──► GP-03 balance
   → PD-1/PD-3/PD-6 decisions ──► FLOW-01 ──► UI-01..UI-05, AND-02, AND-03
   → LEG-01 licensing ──► BRAND-01 / content replacement ──► STORE-01
   → ID-01 (name/package) ──► REL-01 signed AAB ──► Play internal → closed testing
   → PERF-01 / OPT-01 / QA-01 / QA-02 run after UI+content are stable, then regress
```
Critical path: M0 + device → AND-01 → gameplay validation/fixes → flow/UI → content/licensing → optimization/QA → signed release → Play testing tracks.

## 20. Effort Estimates
Developer working days, solo developer, ranges include rework but not external waiting (store review, legal turnaround, art outsourcing).

| Bucket | Days |
|---|---|
| M0 completion + Android dev build (M0-*, AND-00) | 3–7 |
| Android input & compat (AND-01/02/03, PKG-01) | 11–23 |
| Gameplay fixes/validation/balance (AI-01, GP-01/02/03) | 15–29 |
| Flow + UI (FLOW-01, UI-01…UI-05) | 24–44 |
| Audio/optimization/perf (AUD-01, OPT-01, PERF-01) | 12–26 |
| QA (QA-01, QA-02) | 10–19 |
| Identity/branding, store, legal prep (ID-01, BRAND-01, STORE-01, MON-01) | 12–28 |
| Licensing resolution (LEG-01) | 3–40 |
| Release pipeline (REL-01) | 2–4 |
| **Total (all above, incl. P2 excluded)** | **~92–220**, practical planning range **~95–165** if licensing replacement is partial |
| P2 (§16) | +20–40 |
Treat as order-of-magnitude. The largest uncertainty is LEG-01 (content replacement) and what device testing reveals.

## 21. Proposed Revised Android Roadmap (proposal only; no milestone file changed)

| Existing | Android recommendation |
|---|---|
| **M0 Stabilization** | KEEP as immediate priority. Complete first. Add: Android dev build + `logcat` capture (AND-00). Windows build task → DEFER/optional (Editor-only convenience). |
| **M1 Gameplay Validation** | KEEP, re-target to **device**: run G/AI/P checklists on Android; add AND-01 here; fix AI-01/GP-01 inside M1 (small, scoped fixes require approval). Drop two-human/4-player rows. |
| **M2 Platform Input & Compatibility** | REDEFINE as "Android Compatibility": touch controls, lifecycle, safe area, orientation, back-key, aspect ratios, target API. REMOVE PC keyboard/gamepad (KI-010, W-xx) from release scope. |
| **M3 Game Foundation** | KEEP but slim: scene/menu flow (FLOW-01), match config (single human vs bots), arena selection, settings persistence, optional folder migration (ADR-004) — only if it pays for itself. |
| **M4 Gameplay & UX Polish** | KEEP; merge UI-03/UI-05/AND-03/GP-03 here. |
| **M5 Original Content** | KEEP; its minimum scope is driven by LEG-01. Optional content stays P3. |
| **M6 Optimization & QA** | KEEP; make Android budgets from PD-6; includes QA-01/02. |
| **M7 Commercial Release** | KEEP; add Play Console, signing, store assets, privacy, closed testing; remove PC distribution. |
| **New** | "M-Decisions" checkpoint after M0: close PD-1…PD-10 before M3. "Android device gate": no M3/M4 UI investment before AND-01 passes on a real device. |

## 22. Release Readiness Scorecard
No percentages are given. Rating scale: **Not started / Early / Partial / Unknown**. "Unknown" means no evidence either way; it is **not** zero and not complete.

| Category | Rating | Basis |
|---|---|---|
| Core gameplay | Partial (code exists, unverified) | §4–§5 STATIC |
| Android controls | Early / Unknown (joystick code exists; scene presence & device behaviour unknown) | §6.1 |
| Android compatibility | Unknown (settings unreadable, never built) | §6.3 |
| UI/UX | Early (tutorial-grade; 4 of 8 required screens missing or text-only) | §7 |
| Performance | Unknown (no profiling, no budget) | §6.4 |
| Stability | Unknown (no runtime logs) | §8 |
| Content | Early (sample content; originality unresolved) | §9 |
| QA | Not started (no tests, no executed checklist) | §13 |
| Legal / compliance | Not started (KI-007 unresolved; privacy, Data Safety none) | §9 |
| Store readiness | Not started | §9 |

## 23. Immediate Next Actions
1. **Owner:** confirm independent backup; apply Force Text and Build Settings order in the Editor (M0 Stage A).
2. **Owner:** connect an Android device (USB debugging) and confirm SDK/NDK/JDK paths in Preferences → External Tools; record the device model/Android version.
3. **Owner/Editor:** open Desert, Jungle, Moon; confirm `MobileControlCanvas` and `m_SpawnPoints` length in each scene; read Android Player Settings (min/target SDK, orientation, arch, graphics API, identifier) and record them.
4. **After approval:** produce Android development build, install, capture `adb logcat` through menu → one 1v1 match (AND-00/AND-01).
5. **Owner:** record decisions PD-1, PD-4, PD-6 and name an owner/process for LEG-01.

## 24. Assessment Limitations
- Static inspection only; no Unity Editor session, compile, build or device was used. Unity MCP/Editor was not queried in this assessment.
- Binary assets (scenes, prefabs, ProjectSettings) were not parsed; scene contents, Android player settings, import settings, quality tiers, mixer wiring, EventSystem and prefab details are UNKNOWN or second-hand.
- Previous-session claims (Editor query results, toolchain presence) are quoted from `M0_BASELINE.md` and were not re-verified.
- Effort numbers are estimates without a measured velocity; Play policy statements (target API, testing rules) need confirmation against current Google documentation at submission time.
- No legal conclusion is made on any licence or EULA.
- No performance figures are given.
- Files created by this task: this report only. No other file was modified.
