# PROJECT AUDIT — "Tanks" (Unity 6, URP)

> Audit type: read-only forensic audit. Nothing in the project was modified, built, opened in the Editor, or run. Every statement below comes from static inspection of files on disk. **No gameplay, build, or test run was performed** — "works" always means "the code appears consistent", not "verified in play mode".
> Audit date: 2026-11-06. Repository root: `D:\CodeDrift Studio\UW\Tanks`.
> Evidence labels: **CONFIRMED / HIGH / MEDIUM / LOW / UNKNOWN**.

---

## 1. Executive Summary

| Question | Answer |
|---|---|
| What is it? | A Unity 6 (6000.3.6f1) project that is **Unity Learn's official "Tanks! – Make a battle game for web and mobile" tutorial project** (CONFIRMED: `Assets/_Tanks/Documentation.txt` links the course and the Unity Play demo). It is not original game code. |
| Is there original/custom work? | **None identified.** All 23 C# files, the `Tanks.Complete` namespace, the prefabs, scenes and art are the Unity-provided tutorial content (HIGH). Nothing indicates the owner has added gameplay of their own. |
| Current state | The project is the tutorial's **"complete" content imported into a fresh Unity 6.3 project**, with packages upgraded to current versions. The finished game exists only as the three `Demo_Game_*` scenes. The non-demo `Prefabs/Levels/Levels_Empty` are the blank starting points the tutorial has the learner fill in. |
| Development history | Not recoverable. **No git repo, no changelog.** Timestamps show only: Unity template files (31 Jan 2025), tutorial asset import (6 Oct 2026), and Editor/package activity on 6 Nov 2026 (today). |
| Last activity | Today's Editor session opened `Demo_Game_Jungle.unity` (CONFIRMED: `Library/LastSceneManagerSetup.txt`), upgraded packages (`Logs/Packages-Update.log`), and re-saved `PC_RPAsset.asset` / URP global settings. |
| Biggest concrete defect | `ProjectSettings/EditorBuildSettings.asset` points to a scene path that **does not exist** (`Assets/_Tanks/_Demo/Scenes/_Complete-Game Jungle.unity`), and the real scenes are not in the build list. A player build today would have no valid scene 0 (HIGH — see I-01). |
| Release readiness | **Not releasable as a product of its own**: it is a tutorial. As a learning/prototype base it is close to runnable once the build scene list is fixed. |
| Recommended strategy | **Decide intent first (Section 34).** If the goal is "follow the tutorial": continue untouched. If the goal is "build my own game on this base": *Stabilize, then continue* (fix build settings, set up a repo, rename/relocate `Tutorial_Demo` content into real game content). Do **not** rewrite. |

---

## 2. AI Handoff Context

- **Project**: Unity project "Tanks" (company `DefaultCompany`, bundle id `com.Company.ProductName` — default placeholders, CONFIRMED in `ProjectSettings/ProjectSettings.asset`). Local multiplayer/vs-AI top-down tank arena battler (2–4 tanks, last tank standing wins a round, first to 5 round wins takes the game).
- **Technology**: Unity `6000.3.6f1`, C#, Universal Render Pipeline 17.3.0, Input System 1.18.0 (Input System *only*; `activeInputHandler: 1`), uGUI 2.0.0 + TextMeshPro, AI Navigation 2.0.9 (NavMesh used by bots), Timeline, Visual Scripting (unused). Several AI-assistant/AI-inference packages are also present (editor tooling, not game code).
- **Current state**: Full tutorial game appears present and wired in three `Demo_Game_*` scenes (Desert, Jungle, Moon). Scripts compile (compiled `Assembly-CSharp.dll` exists in `Library/ScriptAssemblies`; I did not parse it for errors, HIGH).
- **Last known development**: Opening the Jungle demo scene and upgrading packages (today). No functional edits to game scripts after the original 6 Oct import (all `.cs` timestamps are identical-minute, i.e., a bulk import).
- **Architecture**: `GameManager` (state + round loop coroutines) → `TankManager` (plain serializable per-tank wrapper) → per-tank MonoBehaviours `TankMovement`, `TankShooting`, `TankHealth`, `TankAI`, `TankInputUser`, `PowerUpDetector`. Menu: `GameUIHandler` + `StartMenuSlot`. Power-ups: `PowerUp`, `PowerUpSpawner`, `PowerUpHUD`, `PowerUpFX`. Camera: `CameraControl` (multi-target framing).
- **Completed (appears)**: menu → tank selection (4 slots, P1/P2/CPU) → rounds → win → reload scene 0; AI with Seek/Flee on NavMesh; charge-to-fire shooting; splash damage + knockback; 6 power-up types; pause menu; mobile virtual joystick; three themed levels.
- **Partial/Missing**: no tests, no save/progression, no audio mixer for player settings (only a demo mixer folder), no options menu, no analytics/monetization, no title screen scene, placeholder identity (`DefaultCompany`), no version control.
- **Known problems**: dangling build scene list; scene index 0 hard-coded for restart; `TankAI` per-tick allocations; fragile string-based material-name matching; no git.
- **Scope**: realistic scope = "playable local vs-AI arena game with 3 levels" — essentially what already exists.
- **Strategy**: Stabilize → decide product direction → continue. No rewrite.
- **Immediate priorities**: (1) `git init` + `.gitignore` for Unity, (2) fix build scene list, (3) play-test one full match in each scene, (4) decide fate of the `Tutorial_Demo` folder.

---

## 3. Project Identity

| Item | Finding | Confidence |
|---|---|---|
| Project name | `Tanks` (solution `Tanks.slnx`, `productName: Tanks`) | CONFIRMED |
| Product name | "Tanks!" (tutorial title) | CONFIRMED |
| Product type | Video game (3D top-down arena shooter; local multiplayer + AI) | CONFIRMED |
| Core purpose | Educational/tutorial base game: Unity Learn course "Tanks! – Make a battle game for web and mobile" | CONFIRMED (`Documentation.txt`) |
| Target users | Learners following the course; players of the Unity Play web demo | HIGH |
| Target platforms | Web (Unity Play/WebGL) and mobile (code has `Application.isMobilePlatform` branches, virtual joystick, portrait/landscape canvas scaling); desktop works via keyboard | HIGH |
| Distribution | Unity Play (demo URL in docs). Project-side store config absent. | CONFIRMED / UNKNOWN for stores |
| Monetization | None. | HIGH |
| Maturity | **Tutorial-complete content, prototype for any original product.** Own-product maturity: "concept/prototype base". | HIGH |

Third-party content: fonts (OFL / Ubuntu licence files present), `Tanks!_Third-PartyNotice.txt` exists (not read in detail — licensing review pending, see Q-06).

---

## 4. Technology Stack

| Area | Detail | Evidence |
|---|---|---|
| Engine | Unity 6000.3.6f1 (Unity 6.3) | `ProjectSettings/ProjectVersion.txt` |
| Language | C# (23 scripts incl. 3 editor-only) | `Assets/_Tanks/Scripts` |
| Render pipeline | URP 17.3.0, separate `PC_Renderer` / `Mobile_Renderer` + RP assets | `Assets/Settings`, `manifest.json` |
| Input | Input System 1.18.0 only. Actions asset `Assets/_Tanks/Settings/Tank_Actions.inputactions` (schemes: KeyboardLeft, KeyboardRight, Gamepad). A second default `Assets/InputSystem_Actions.inputactions` (Unity template) also exists. | scripts, `ProjectWideActionsSetup.cs` |
| UI | uGUI Canvas + TextMeshPro; on-screen controls via `UnityEngine.InputSystem.OnScreen` | `GameUIHandler.cs` |
| AI | `NavMesh` via AI Navigation 2.0.9; baked data in `Demo_Scene_Data/Demo_NavMeshes` | `TankAI.cs` |
| Physics | 3D Rigidbody; `linearVelocity` API (Unity 6) | `TankMovement.cs` |
| Packages of note | Timeline, Visual Scripting, Test Framework 1.6.0 (no tests), AI Assistant 2.21.0-pre.2, AI Inference 2.6.1, Multiplayer Center | `Packages/manifest.json` |
| Tooling | VS Code launch config for Unity attach; `.csproj`/`.slnx` generated; Rider + VS IDE packages | `.vscode`, root files |
| Build system | Unity build pipeline; no CI, no scripted builds | repo |
| Backend/DB/cloud | None | repo |

Note: `com.unity.ai.assistant` is a **pre-release** package (`2.21.0-pre.2`) and the `Library/AI.MCP` folder indicates an MCP bridge is active in the Editor — editor tooling only, not shipped (HIGH).

---

## 5. Repository Structure

```text
Tanks/
├─ Assets/
│  ├─ Scenes/SampleScene.unity           Unity template scene (31 Jan 2025). Unused (no game components).
│  ├─ Settings/                          Unity URP template assets (PC/Mobile renderers + RP assets, volume profiles)
│  ├─ InputSystem_Actions.inputactions   Unity template default actions (unused by game)
│  └─ _Tanks/
│     ├─ Documentation.txt, Tanks!_Third-PartyNotice.txt
│     ├─ Art/ (Animators, Fonts, Materials, Models[fbx], Sprites, Textures)
│     ├─ Audio/ (Music, SFX)
│     ├─ Prefabs/ (DustTrails, Environment/{Desert,Jungle,Moon,Misc}, Explosives, Levels, PowerUps, UI)
│     ├─ Scripts/ (Camera, Editor, Managers, PowerUp, Shell, Tank, UI[/MobileControl])
│     ├─ Settings/ (Tank_Actions.inputactions, Profiles, TextMesh Pro)
│     └─ Tutorial_Demo/ (Demo_AudioMixers, Demo_Materials, Demo_Prefabs, Demo_Scenes[+Scene_Data])
├─ Packages/ (manifest.json, packages-lock.json)
├─ ProjectSettings/
├─ Library/, Temp/, Logs/, UserSettings/    Generated / local (should not be versioned)
├─ .vscode/, *.csproj, Tanks.slnx           Generated IDE files
```

| Directory | Purpose | State | Notes |
|---|---|---|---|
| `Assets/_Tanks/Scripts` | All gameplay code (namespace `Tanks.Complete`) | Complete tutorial code | 23 files, ~150 KB |
| `Assets/_Tanks/Tutorial_Demo` | The **finished** demo: scenes, tank variants (ATV, Heavy, Medium, Shark), shell variants, mixers, lighting & navmesh data | The only playable game content | Name implies "demo/reference"; the real game *lives* here |
| `Assets/_Tanks/Prefabs/Levels/Levels_Empty` | Blank level prefabs (Desert/Jungle/Moon) for learners | Starting scaffolding | No Tank prefabs exist outside `Tutorial_Demo` (CONFIRMED by listing) |
| `Assets/Scenes`, `Assets/Settings`, `Assets/InputSystem_Actions.inputactions` | Unity project-template leftovers | Unused/leftover | `SampleScene` has none of the game components |
| `Library`, `Temp`, `Logs`, `UserSettings` | Unity-generated | N/A | No `.gitignore`/repo exists to exclude them |

Anomalies: no duplicate/backup folders found. Two input-action assets (template + tank). Binary-serialized `EditorBuildSettings.asset` (see I-01).

---

## 6. Development History

- **Git history**: none — directory is not a git repository (CONFIRMED). No tags, branches, changelog, or TODO files.
- **Evidence from timestamps** (MEDIUM; clocks/ import times can mislead):
  - 31 Jan 2025 — Unity 6 URP template assets (`SampleScene`, `Assets/Settings`, `InputSystem_Actions`).
  - 6 Oct 2026 — bulk import of all `_Tanks` content (all scripts/scenes within the same minute).
  - 6 Nov 2026 — Editor opened; packages merged/updated (`Logs/Packages-Update.log`: Input System 1.12→1.16→1.18, URP 17.0.1→17.3.0, AI Navigation 2.0.0→2.0.9, etc.); `PC_RPAsset.asset` and URP global settings re-saved.
- **Phases (inferred)**: (1) created project from Unity 6 URP template (an older 6000.0.x Editor — `EditorBuildSettings.asset` header says `6000.0.23f1`); (2) imported Tanks tutorial package; (3) upgraded to 6000.3.6f1 and current packages; (4) opened Jungle demo scene.
- **Hint of distribution method**: `ProjectWideActionsSetup.cs` comments say the project "is imported as a .unitypackage", i.e. the tutorial's own packaging (CONFIRMED in code comment).
- **Abandoned work**: none identifiable — nothing started by the owner.
- **Replaced systems**: the code itself notes the menu "was originally written to have any number of prefabs and player, but this was fixed to always 4 tanks" (`GameUIHandler.cs` comment) — a vestige of the original authors, not the owner.
- **Last known state**: Editor open on the Jungle demo scene; no unsaved-work indicators inspected.

---

## 7. Current Project Status

> Percentages are labelled estimates, not computed from file counts.

```text
Overall: 🟡 Complete tutorial base, no original product yet
Product completion (vs. the tutorial game):  ≈ 90 %  (estimate, untested)
Product completion (vs. an original release): not meaningful / ≈ 0 % original scope defined
Core gameplay:        🟢 appears implemented
AI opponents:         🟢 implemented (Seek/Flee), 🟡 unverified
UI/UX:                🟡 start menu, pause, mobile controls present; no options/title
Data/persistence:     ⚪ none (by design)
Integrations:         ⚪ none
Testing:              🔴 no tests
Architecture:         🟡 tutorial-grade, readable, some coupling
Build/Deploy:         🔴 build scene list broken, placeholder identity, no CI
Documentation:        🟡 external course + dense code comments; no README
Version control:      🔴 none
```

Product completion, technical health, and release readiness are deliberately kept separate (see §21).

---

## 8. Feature Inventory

| Feature | Status | Conf. | Evidence | Files | Dependencies | Notes |
|---|---|---|---|---|---|---|
| Start menu with 4 tank slots, assign P1/P2/CPU/Off | 🟢 | HIGH | `GameUIHandler.Start`, `StartMenuSlot` | `GameUIHandler.cs`, `StartMenuSlot.cs`, `Menus.prefab` | GameManager, URP camera stack | Requires ≥2 tanks to start |
| Tank preview (rotating, recolored) | 🟢 | HIGH | `SetTankPreview` | `StartMenuSlot.cs` | `TankColor` material naming | Disables movement/shooting on preview |
| Round loop (start→play→end), 5 wins to take game | 🟢 | CONFIRMED (code) | `GameManager.GameLoop` | `GameManager.cs` | TankManager | `m_NumRoundsToWin`, delays inspector-set |
| Game restart on win | 🟡 | CONFIRMED | `SceneManager.LoadScene(0)` | `GameManager.cs:179` | Build index 0 | **Depends on broken build list** |
| Tank movement (tank-style & direct control) | 🟢 | HIGH | `TankMovement` | `TankMovement.cs` | Input System, `Camera.main` | Gamepad scheme uses camera-relative direct control |
| Charge-to-fire shooting, cooldown, aim slider | 🟢 | HIGH | `TankShooting` | `TankShooting.cs` | ShellExplosion | Human & AI update paths |
| Shell splash damage + knockback | 🟢 | HIGH | `ShellExplosion` | `ShellExplosion.cs` | Layer "Players" mask | |
| Health, death explosion, shield/heal hooks | 🟢 | HIGH | `TankHealth` | `TankHealth.cs` | Slider UI | |
| Bot AI (NavMesh seek, shoot, flee) | 🟢 | HIGH | `TankAI` | `TankAI.cs` | NavMesh baked data | Needs baked NavMesh per scene |
| Power-ups (healing, speed, shell, shooting, shield, invincibility) | 🟢 | MEDIUM | `PowerUp*` + prefabs | `PowerUp.cs`, `PowerUpDetector.cs`, `PowerUpSpawner.cs`, `PowerUpHUD.cs`, `PowerUpFX.cs` | TankShooting/Health/Movement | Not reviewed line-by-line |
| Camera auto-framing of all tanks | 🟢 | HIGH | `CameraControl` | `CameraControl.cs` | GameManager | |
| Pause menu | 🟢 | MEDIUM | `PauseMenu`, `GameUIHandler.TogglePause` | `PauseMenu.cs` | Pause action | |
| Mobile virtual joystick | 🟡 | MEDIUM | `MobileUIControl`, `UIVirtualJoystick` | `UI/MobileControl/*` | Gamepad scheme | `MobileUIControl` is **not** referenced in any scene (0 hits) — present only in `MobileControlCanvas.prefab`; scene use unverified |
| 3 themed levels (Desert/Jungle/Moon) | 🟢 | HIGH | 3 demo scenes + 3 level prefabs | `Demo_Game_*.unity` | Lighting & NavMesh data | |
| Audio (engine, charge, fire, music) | 🟢 | MEDIUM | audio assets + AudioSource fields | `Art`/`Audio` | | Mixers only in `Tutorial_Demo` |
| Editor helpers (power-up inspector, SceneView cam, project-wide actions fix) | 🟢 | HIGH | `Scripts/Editor` | 3 files | | Editor-only |
| Title screen | ⚪ | MEDIUM | `GameManager` comment mentions "Title Screen which will enable the Menu" | — | | Not present |
| Options/settings, save data, localization | ⚪ | MEDIUM | no code | — | | Absent |
| Online multiplayer | ⚪ | LOW | Multiplayer Center package only | — | | Package ≠ feature; no netcode in code |

---

## 9. User / System Flows

**Main flow (CONFIRMED from code)**

```text
Scene load (Demo_Game_*)
  → GameManager.Start: find MessageTextReference (error if Menus prefab missing), validate 4 tank prefabs
  → GameUIHandler.Awake: add menu camera to URP camera stack of Camera.main
  → Start menu: 4 StartMenuSlot (preview tank rotating)
  → player clicks "Add" → assign P1 (first) / CPU (others); buttons P1/P2/CPU/Off
  → Start (enabled when ≥2 slots) → GameUIHandler.StartGame → PlayerData[]
  → GameManager.StartGame → SpawnAllTanks (instantiate at TankManager.m_SpawnPoint) → TankManager.Setup → SetCameraTargets
  → GameLoop: RoundStarting (reset, disable control, "ROUND n") → RoundPlaying (until ≤1 tank active) → RoundEnding (winner++, message)
  → if someone has m_NumRoundsToWin wins → SceneManager.LoadScene(0) else loop again
```

Error/edge conditions seen: missing `MessageTextReference` or missing tank prefabs → `Debug.LogError` and (for the first) early `return` from `Start` (game then cannot start). Draw → "DRAW!". Tank death = `SetActive(false)`; round reset toggles active.

**Persistence**: none. All state is in-memory; scene reload resets everything.

**Missing/broken steps**: restart relies on scene index 0 (I-01).

---

## 10. Architecture

- **Style**: Component-based Unity architecture with a central manager; loosely MVC-ish UI. Namespace `Tanks.Complete` (CONFIRMED).
- **GameManager** owns game state enum (`MainMenu`, `Game`) and the round coroutine loop. `TankManager` is a `[Serializable]` plain class (not a MonoBehaviour) with a custom `PropertyDrawer` to simplify the inspector.
- **Per-tank components**: movement, shooting, health, AI, input user, power-up detector — each a MonoBehaviour on the tank prefab.
- **Input**: each tank clones the actions asset via `TankInputUser` and pairs it with an `InputUser` so keyboard schemes don't cross-trigger (design visible from comments in `TankManager`/`TankMovement`).
- **Patterns present**: state (enum + coroutines), observer (UI button listeners, `InputAction.performed`), singleton-ish static `MobileUIControl.Instance`, prefab-variant inheritance (Demo tank variants).

```mermaid
graph TD
  Menus[Menus prefab: GameUIHandler + StartMenuSlot x4] -->|PlayerData[]| GM[GameManager]
  GM --> TM[TankManager[] spawn points]
  GM --> Cam[CameraControl]
  TM --> Tank[Tank prefab]
  Tank --> Mov[TankMovement] & Shoot[TankShooting] & HP[TankHealth] & AI[TankAI] & PU[PowerUpDetector]
  Shoot --> Shell[ShellExplosion] --> HP
  Shell --> Mov
  PUS[PowerUpSpawner] --> PUp[PowerUp] --> PU
  AI --> NavMesh
```

---

## 11. Dependency Analysis

- **Central dependencies**: `GameManager` (referenced by UI, `TankManager`, `TankAI.Setup`), `TankMovement` (used by Shell, AI, Menu, PowerUps), `Tank_Actions` asset (by name: "Vertical", "Horizontal", "Fire", "Pause"; schemes "KeyboardLeft/Right/Gamepad").
- **Hidden dependencies (CONFIRMED in code)**:
  - String-based: input action names, control scheme names, material name containing `"TankColor"` (`TankManager.Setup`, `StartMenuSlot.SetTankPreview`).
  - Physics layer named "Players" must be set on the `ShellExplosion.m_TankMask` (inspector-set).
  - `Camera.main` required (`TankMovement`, `GameUIHandler`).
  - `SceneManager.LoadScene(0)` → build index 0 is the game scene.
  - `FindAnyObjectByType` for `MessageTextReference`, `PauseMenu`, `MobileUIControl`.
  - `InputSystem.actions` (Project-wide Actions) must contain "Pause": `ProjectWideActionsSetup` auto-assigns it via a hard-coded GUID (`7fb6c1c7…`) of `Tank_Actions.inputactions`.
  - NavMesh must be baked and present per scene (stored under `Demo_Scene_Data/Demo_NavMeshes`).
- **Circular dependencies**: `GameManager` ↔ `GameUIHandler` (UI holds manager reference and manager finds UI text) — mild, intentional.
- **Packages**: see §4. Unused-but-installed: Visual Scripting, Timeline, Multiplayer Center, AI packages, Collab Proxy, Rider/VS IDE.

---

## 12. Code Quality

**Strengths** (CONFIRMED by reading): extensive explanatory comments; readable names; consistent `m_` prefix; careful guards for learner errors (e.g., `GameManager.SpawnAllTanks` forces `m_IsComputerControlled = false`).

**Weaknesses (specific)**
1. `TankAI.SeekUpdate` allocates `new NavMeshPath[...]` plus `new NavMeshPath()` per target every 0.3–0.6 s per bot, and uses LINQ in `Awake`/`Setup` — GC churn, minor at 2–4 tanks.
2. `TankAI.FleeUpdate` reads `m_CurrentPath.corners.Length` — can throw `NullReferenceException` if state is Flee without a path; `StartFleeing` passes the existing `m_CurrentPath` which may be `null` if the first target never found a path and `m_CurrentTarget` was null (`m_CurrentTarget.position` also unguarded at `TankAI.cs:326`). Potential, not reproduced.
3. `TankAI.FixedUpdate`: `if (!m_IsMoving) orientTarget = m_CurrentTarget.position;` — NRE potential if a path exists but target destroyed/null.
4. `TankManager.Setup` gets `GetComponentInChildren<Canvas>().gameObject` — NRE if a custom tank prefab has no Canvas. `Setup` also does `m_AI.enabled` on a tank whose AI is added later; `DisableControl` uses `m_AI` only when computer-controlled (fine).
5. `TankManager.Setup` calls `inputUser.SetNewInputUser(m_InputUser)` where `m_InputUser` is a never-assigned default struct field — behavior depends on `TankInputUser` handling a default value (not fully analysed, MEDIUM).
6. Material color matching: `renderer.materials[j].name.Contains("TankColor")` — `renderer.materials` instantiates materials each access (called in loops → repeated instancing; leaks material instances until scene unload) and relies on naming.
7. `GameManager.GetGameWinner` uses `==` (not `>=`) with `m_NumRoundsToWin`; safe only because it's checked after every increment.
8. `GameManager.Start` returns early when text ref missing, leaving no state initialised; later `StartGame` would NRE.
9. `TankMovement.FixedUpdate` accesses `m_InputUser.InputUser.controlScheme.Value.name` every physics tick (string compare ×3 per tick) and `Camera.main` (cached by Unity but still a lookup).
10. `TankMovement.OnDisable` uses `m_particleSystems` assigned in `OnEnable`; `Reset()` disables/enables the tank so fine, but a disable-before-enable ordering could NRE (LOW).
11. Mixed Unity naming (`m_`, PascalCase props, `fireAction`) and tabs/spaces mix (`TankMovement.cs:23`).
12. Coroutine `GameLoop` restarts itself recursively via `StartCoroutine(GameLoop())` (acceptable but unconventional).

**Error handling**: mostly `Debug.LogError` on setup errors; no runtime recovery paths; AI has "stuck" detection timers.

**Global/static state**: `MobileUIControl.Instance` only; otherwise instance fields.

---

## 13. Data & State Management

- **Runtime state only**: wins, round number, tank instances, per-tank health/force. Reset on scene reload.
- **Persistence**: none (no PlayerPrefs / file I/O found in the scripts read). No user settings, no high scores.
- **Data flow**: menu → `PlayerData` (color, prefab, control index, IsComputer) → `GameManager` → `TankManager` fields → tank components.
- **Risks**: `TankManager` instances live inside `GameManager.m_SpawnPoints` (serialized); runtime fields such as `m_Wins` are serialized-hidden public fields on a `[Serializable]` class — they can leak into scene serialization if edited at runtime and saved (LOW).

---

## 14. UI / UX

- **Screens present**: Start menu (4 slots, stats text "Speed / Damage / Health"), in-game round text, per-tank world-space health + aim sliders, pause menu, on-screen pause button, mobile control canvas prefab.
- **Technical UI issues**: menu camera must be stacked onto main camera (done in `Awake`); `CanvasScaler.matchWidthOrHeight` is rewritten every `Update` (minor); `StartMenuSlot` uses `GetComponentInParent<Camera>()`, so the menu must be under a camera.
- **UX/product gaps**: no title screen, no settings/volume/controls screen, no tutorial/controls hint, no end-of-match screen beyond text, no back-to-menu from pause (not verified), P2 hidden on mobile.
- **Accessibility**: none evident (text colors only for player identity; colour-coded players with no pattern/label beyond "PLAYER n" coloured text).

---

## 15. Assets / Content

- Counts (excluding Library/Temp): ~69 FBX, ~154 prefabs, ~73 materials, ~36 PNG, ~66 `.asset`, 14 shaders (+4 ShaderGraphs), 6 WAV, 4 OGG, 2 AIF, 8 TTF, 3 content scenes + template scene. Roughly **536 `.meta` files**, none flagged as orphaned (not exhaustively checked).
- Placeholders: `Assets/Scenes/SampleScene`, template `Assets/Settings/*` sample volume profile; default company/bundle id.
- Unused (likely): template `InputSystem_Actions.inputactions`; several fonts (7 families imported — Big Shoulders Stencil, Bowlby One, Exo 2, IBM Plex Mono, Inter, Lora, Ubuntu) — usage by UI not verified.
- Missing: localization, store icons/splash, app icon (default), music mixer outside demo folder.

---

## 16. External Services & Integrations

| Integration | Purpose | Configured? | Used by code? | Complete? | Risk |
|---|---|---|---|---|---|
| Unity Play (web demo) | Hosting of original demo | N/A (docs link) | No | N/A | None |
| Unity AI Assistant / MCP | Editor tooling | Yes (pre-release package; `Library/AI.MCP`) | Editor only | N/A | Pre-release package may change/break on upgrade |
| Unity Analytics module | Engine module enabled by default | Default | No code | N/A | Privacy review if shipped |
| UnityConnectSettings | Cloud project link | Unknown | No | N/A | Not inspected |
| Everything else (ads, IAP, Firebase, auth, backend) | — | No | No | — | — |

---

## 17. Security

- Hard-coded secrets: **none found** in `Assets/_Tanks/Scripts` (searched for api key/password/secret patterns); only GUIDs.
- Network: none in game code. Packages `UnityWebRequest*` are default modules.
- Hard-coded asset GUID in `ProjectWideActionsSetup.cs` — harmless, but breaks silently if the asset is recreated.
- Client-trust issues: not applicable (offline).
- Not inspected: `UserSettings/`, `UnityConnectSettings.asset`, `.vscode/settings.json` for personal data/tokens (content unread) — recommend manual check before any repo is made public.
- Privacy: the user's email appears nowhere in files inspected; Unity Analytics module and AI Assistant package are enabled — decide before shipping (Q-05).

---

## 18. Performance

- No material risks at 2–4 tanks (HIGH on code reading; unprofiled).
- Potential: per-tick GC in `TankAI` (see §12); repeated `renderer.materials` in setup; `TankMovement` string comparisons per FixedUpdate; `Update()` in `GameUIHandler` per frame (trivial).
- Mobile: URP Mobile renderer/RP asset exist; web (WebGL) settings not verified; shader count (14 + ShaderGraphs) could matter for WebGL build size.

---

## 19. Testing & QA

- **No tests** (no `Tests` folders, no `.asmdef`). `com.unity.test-framework` is installed but unused. No CI, no manual test docs.
- High-risk untested areas: round loop edge cases (simultaneous death = draw), AI null paths (§12), multi-device input pairing (keyboard-left/right + gamepad + mobile), power-up stacking, scene reload (`LoadScene(0)`).

---

## 20. Build & Deployment

- Can it likely build? **Not as configured.** `EditorBuildSettings.asset` is a binary-serialized file whose scene entry is `Assets/_Tanks/_Demo/Scenes/_Complete-Game Jungle.unity` — a path that does not exist in this repo (actual: `Assets/_Tanks/Tutorial_Demo/Demo_Scenes/Demo_Game_Jungle.unity`). A build would either fail or include no valid scene. (HIGH — string read from the binary; the Editor may have displayed it as missing.)
- Identity defaults: `companyName: DefaultCompany`, bundle id `com.Company.ProductName`, version `1.0`, `AndroidBundleVersionCode: 1`, `AndroidTargetSdkVersion: 0` (auto). Not store-ready.
- Dependencies: all packages come from the Unity registry via `manifest.json`; no local/git packages.
- Platform: no signing config, no CI, no build scripts, no WebGL template customisation observed.
- `Tanks.slnx`/`.csproj` indicate VS 2022+/VS Code workflow.

---

## 21. Release Readiness

Separate scores (qualitative):

```text
Product completion (tutorial game): 🟢 high
Technical health:                   🟡 good readability, limited robustness
Release readiness:                  🔴 not ready
```

- **Blockers**: broken build scene list (I-01); placeholder company/bundle id; ownership/licence of assets for any public release (Q-06).
- **Critical quality**: AI null-reference risks; no automated tests; no full-match smoke test recorded.
- **Production prep**: icons, splash, version, store listing, privacy, target-platform build tests (WebGL/Android/iOS).
- **Polish**: title screen, settings, back-to-menu, accessibility, controls hints.
- **Optional**: online multiplayer, more levels/tanks, progression, analytics.

---

## 22. Bug & Issue Inventory

| ID | Issue | Pri | Status | Evidence | Impact | Likely cause | Direction |
|---|---|---|---|---|---|---|---|
| I-01 | Build settings reference non-existent scene; real scenes not listed | P0 | Open | `EditorBuildSettings.asset` string; scene folder listing | No valid build; `LoadScene(0)` breaks | Project created on 6000.0.23f1 with older tutorial layout (`_Demo/Scenes`) and then replaced by `Tutorial_Demo/Demo_Scenes` | Re-add scenes to build list (decision: which scene is index 0) |
| I-02 | No version control | P0 (process) | Open | not a git repo | No history, no rollback | — | `git init` + Unity `.gitignore`; set `Asset Serialization: Force Text` and Visible Meta Files |
| I-03 | `TankAI` possible NRE (target/path null) | P2 | Potential | `TankAI.cs:297,326,363` | Bot crash/stall | Unguarded paths | Add guards after play-testing |
| I-04 | `TankManager.Setup` requires a `Canvas` child; custom tanks without it throw | P3 | Potential | `TankManager.cs:44` | Custom-prefab failure | Tutorial assumption | Validate/log |
| I-05 | `GameManager.Start` early return leaves manager half-initialised | P3 | Potential | `GameManager.cs:62-66` | Later NRE on StartGame | Learner-guard design | Fail fast with clearer state |
| I-06 | Pre-release `com.unity.ai.assistant` in manifest | P3 | Open | `manifest.json` | Instability/unexpected package changes | Editor feature | Decide keep/remove (do not remove without owner) |
| I-07 | Two competing Input Actions assets; project-wide assignment is repaired by an editor script | P3 | Open | `InputSystem_Actions` vs `Tank_Actions`, `ProjectWideActionsSetup` | Confusing; "Pause" lookup fails if project-wide actions revert | Package import ordering | Keep helper until stable; document |
| I-08 | `MobileUIControl` absent from all scenes inspected | P2 | Unverified | 0 matches in 4 scenes | Mobile controls may be missing from scenes (only in prefab) | Might be added at runtime or by learner | Verify in-Editor |
| I-09 | Repeated `renderer.materials` calls in setup loops | P3 | Open | `TankManager.cs:77-83`, `StartMenuSlot.cs:144-155` | Material instance duplication | Convenience API | Use `sharedMaterials`/cache |
| I-10 | `Library`/`Temp` etc. unignored | P3 | Open | no ignore file | Giant commit if repo created naively | — | .gitignore first |

---

## 23. Dead Code / Unused Content

Likely unused (not proven): `Assets/Scenes/SampleScene.unity`; `Assets/InputSystem_Actions.inputactions`; Visual Scripting, Timeline, Multiplayer Center packages; unused font families; `Tutorial_Demo` demo-prefix duplicates of normal prefabs (`Demo_CompleteShell Variant_*` vs `Prefabs/Explosives/CompleteShell`). `TankAI.m_TimeBetweenShot` etc. fine. No commented-out blocks of significance found in scripts read.

---

## 24. Technical Debt

| Item | Problem → Cost → Risk → Direction |
|---|---|
| String-keyed coupling (actions, schemes, material names) | Renames silently break gameplay → runtime errors → Centralise constants |
| `Tutorial_Demo` holds the real game | Confusing ownership; folder may be deleted by mistake → Plan relocation after deciding scope |
| Fixed 4-slot menu | Hard-coded 4 prefabs → limits tank roster growth → Data-driven roster (ScriptableObject) if roster will grow |
| No tests | Regressions undetected → Add PlayMode smoke test for round loop |
| No repo | See I-02 |
| AI allocations/NREs | See §12 |
| Placeholder project identity | Blocks store builds → set before first build |

---

## 25. Missing / Implied Features

| Feature | Evidence | Conf. | Current state | Importance |
|---|---|---|---|---|
| Title screen | `GameManager` comment | MEDIUM | Not present | Low |
| Persistent settings / audio sliders | none; mixers only in demo | LOW | Absent | Medium |
| Tutorial-style "build it yourself" scaffolding | `Levels_Empty`, no Tank prefabs outside demo | HIGH | Learner is meant to create tank prefab | Context only |
| More tanks | `Demo_Tank` variants (ATV, Heavy, Medium, Shark) | HIGH | Present as variants | — |

---

## 26. Original Scope Reconstruction

- **Core concept**: Local arena tank battle — last tank standing, charged artillery shots, power-ups, 3 themed maps, 2–4 combatants incl. bots.
- **MVP**: Everything the demo scenes already do, plus a valid build.
- **Likely original (Unity authors') release scope**: the Unity Play web demo (CONFIRMED link) — matches `Tutorial_Demo`.
- **Owner's intended scope**: **UNKNOWN** — see Q-01.
- **Optional**: online play, progression, additional levels.
- **Out of scope unless asked**: AI/inference packages' features, Visual Scripting.

---

## 27. Restart Options

| Option | Pros | Cons | Risk | Complexity | When |
|---|---|---|---|---|---|
| A Continue | Fast | Inherits build issue, no repo | Low | Low | Following the tutorial |
| **B Stabilize then continue (recommended)** | Fixes build list/repo/identity; keeps all working content | Small upfront work | Low | Low | Building own game on this base |
| C Selective rebuild | Replace AI/menu with data-driven versions | Effort | Medium | Medium | Roster/AI redesign planned |
| D Major refactor | Clean architecture | Not warranted by code size | Medium | Medium-High | Only if scope grows a lot |
| E Full rebuild | Fresh start | Throws away working game | High | High | Not justified |

---

## 28. What to Preserve
Gameplay scripts (readable, coherent), `Tank_Actions` input setup, NavMesh/lighting data, prefabs/variants, three levels, URP PC/Mobile renderer split, `ProjectWideActionsSetup` helper (until input setup is stable).

## 29. What to Rework
Build settings (P0); AI null guards/allocations; scene/identity metadata; project structure (`Tutorial_Demo` → real game folders) once the product direction is chosen; input-asset duplication.

## 30. What Not to Touch Yet
Shell/explosion maths, camera framing, shaders/ShaderGraphs, lighting & NavMesh bakes (re-baking changes feel and may invalidate prefab refs), font imports, package versions (just upgraded; avoid more churn).

---

## 31. Prioritized Development Backlog

| Priority | Task | Reason | Deps | Complexity | Area |
|---|---|---|---|---|---|
| P0 | `git init`, Unity `.gitignore`, force text serialization | No safety net | — | Low | Process |
| P0 | Fix Build Settings scene list | Build + restart rely on it | — | Low | Build |
| P1 | Play-test full match in each scene (2P, P+CPU, 4 tanks) | Nothing run yet | P0 | Low | QA |
| P1 | Set company/product/bundle id/icons | Needed for any build | — | Low | Release |
| P1 | Decide scope/intent | Drives everything | Owner | Low | Product |
| P2 | Guard AI NREs; cache materials | Robustness | P1 | Low | Code |
| P2 | Build WebGL/Android test | Target platform validation | P0 | Medium | Build |
| P3 | Title/options/pause-to-menu | Polish | P1 | Medium | UX |
| P4 | New tanks/levels/netcode | Expansion | Scope | High | Features |

---

## 32. Recommended Development Order

```text
1 Version control + ignore rules
  ↓
2 Fix build scene list; verify game runs from a build
  ↓
3 Play-test + log defects (esp. AI, mobile controls, input pairing)
  ↓
4 Decide product direction / scope with owner
  ↓
5 Restructure folders (rename Tutorial_Demo content into game content) — only after 4
  ↓
6 Robustness fixes (AI guards, setup validation)
  ↓
7 New features / content
  ↓
8 Platform builds, polish, release prep
```

---

## 33. Project Risks

| Risk | Severity | Prob. | Impact | Mitigation |
|---|---|---|---|---|
| Loss of work (no VCS) | High | Medium | Total | Init repo now |
| Unclear product intent | High | High | Wasted effort | Answer Q-01 |
| Licence/ownership of tutorial assets in a commercial release | High | Medium | Legal | Review third-party notice & Unity terms (Q-06) |
| Pre-release/AI packages breaking upgrades | Medium | Medium | Editor instability | Pin/decide |
| Input-actions assignment regression | Medium | Low | Pause/inputs fail | Keep helper |
| Platform differences (mobile/WebGL) untested | Medium | Medium | Rework | Early builds |

---

## 34. Questions for the Project Owner

- **Q-01** Is the goal to complete the Unity tutorial, or to turn this into your own published game? (Changes almost every recommendation.)
- **Q-02** Is `Tutorial_Demo` meant to stay as read-only reference, or become the game?
- **Q-03** Which platforms are actually targeted (WebGL, Android, iOS, PC)?
- **Q-04** Was `MobileControlCanvas` meant to be in the scenes (mobile support wanted)?
- **Q-05** Are the Unity AI Assistant / Analytics / Multiplayer Center packages wanted?
- **Q-06** Are you allowed to redistribute the tutorial's art/audio commercially (see `Tanks!_Third-PartyNotice.txt`)?
- **Q-07** Is there other work (different folder, another machine, cloud/Plastic SCM) not in this directory?

---

## 35. Final Assessment

**Project**: Unity's "Tanks!" tutorial game imported into a Unity 6.3 URP project.
**State**: Content-complete tutorial base; packages freshly upgraded; no custom code, tests, history, or valid build configuration.
**History**: Template project → tutorial import (Oct 2026) → upgrade & scene open (6 Nov 2026).
**Last activity**: Opening `Demo_Game_Jungle` and package upgrades.
**Completed**: Menu, round loop, movement, shooting, AI, power-ups, camera, pause, 3 levels (code-inspected, unrun).
**Partial/Risky**: Build list (broken), mobile controls wiring (unverified), AI null paths.
**Missing**: Repo, tests, title/options, project identity, owner-defined scope.
**Architecture health**: Good for its size; readable and coherent; string coupling and tutorial-oriented guard code are the main debt. Not a rewrite candidate.
**Realistic scope**: What the demo already shows, built and tested on the target platform(s).
**Release readiness**: Far from releasable as a product; very close to "runs from a build" once I-01 is fixed.
**Strategy**: Stabilize, confirm intent, continue.

**Top 10 next actions**
1. Answer Q-01/Q-02 (intent).
2. `git init` with a Unity `.gitignore`; force text serialization.
3. Fix Build Settings scene list (choose scene 0).
4. Play-test every demo scene (2P, 1P+CPU, 4 tanks, power-ups, win → restart).
5. Verify mobile controls in scenes (I-08) if mobile is a target.
6. Set product name, company, bundle id, icon, version.
7. Add AI/setup null guards (I-03..I-05).
8. Make a WebGL (or target platform) test build.
9. Review third-party/licence notice (Q-06).
10. Plan folder restructuring and roster/data-driven design only after scope is decided.

---

### Audit limitations
Not examined in depth: `PowerUp*.cs` internals, `CameraControl.cs`, `PauseMenu.cs`, `TankInputUser.cs`, `UIVirtualJoystick.cs`; scene YAML is partially binary-serialized (Unity Editor serialization mode is not Force Text for some assets), so scene wiring was checked by string matching only; the `Library/` Editor logs were not analysed for compile warnings; nothing was executed.
