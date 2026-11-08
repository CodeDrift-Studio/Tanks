# CLAUDE.md — Tanks! (Enhanced Tanks Arena)

Primary instruction file for every Claude Code session in this repository. Read it first. Then follow **§J (workflow)** and **§M (session procedure)**.

Evidence labels used everywhere: **VERIFIED** (checked by running/inspecting in this session), **STATIC** (read from files, not run), **UNVERIFIED**, **UNKNOWN**, **PENDING DECISION**.
Baseline facts were gathered on 2026-11-06 by static inspection only. **Nothing has been compiled, built or run by Claude.** Do not claim otherwise unless you actually do it.

---

## A. Project Overview

- **Scope update (owner-confirmed, recorded 2026-11-08 — overrides older text in this file and in `PROJECT_AUDIT.md`)**: first commercial release is **Android only**, **single-player vs AI**, touch controls mandatory. Windows/PC, iOS, online multiplayer and shared-screen Android multiplayer are **out of first-release scope** (code/assets stay; nothing is deleted). Business model: **Free-to-Play with Ads + IAP**. **Mandatory integrations: Google AdMob, Unity IAP / Google Play Billing, Firebase, Google Play Games Services.** The exact feature scope of each service is **PENDING** (see `Documentation/Development/DECISION_REGISTER.md` and `SERVICE_INTEGRATION_PLAN.md`); nothing is installed or configured yet. Roadmap: **M0–M8** in `Documentation/Development/MILESTONE_ROADMAP.md`. Where this file still mentions PC/Windows or "2–4 combatants", read it as the code's current capability, not release scope.
- **Game**: Tanks! — "Enhanced Tanks Arena". 3D arcade tank arena combat, short round-based matches. The code supports 2–4 combatants (humans and/or CPU bots); the first release uses one human vs AI.
- **Origin**: Unity's "Tanks!" sample/tutorial project (Asset Store *Tanks! 3D Sample Project*; Unity Learn course "Tanks! – Make a battle game for web and mobile"). `Assets/_Tanks/Documentation.txt` confirms this. The existing code is the **foundation**, not the final product. Goal: an original, standalone, commercial game.
- **Engine**: Unity 6.3 (`6000.3.6f1`), URP 17.3.0, C#. Input System 1.18.0 (Input System only; `activeInputHandler: 1`).
- **Platforms**: **Android only** for the first release. Windows (Editor use only), iOS and WebGL are not release targets (a leftover WebGL branch exists in `PauseMenu`; ignore).
- **Multiplayer**: none in the first release (no online, no shared-screen Android).
- **Business model**: Free-to-Play, Ads + IAP (confirmed). **Final name, package ID, ad formats/placements, IAP catalog and pricing, age-group/audience, release channel: PENDING DECISION** (`DECISION_REGISTER.md`).
- **Audit**: `PROJECT_AUDIT.md` (static, 2026-11-06). If the repo and the audit disagree, **the repo wins** — then record the discrepancy in `Documentation/Development/KNOWN_ISSUES.md`.

## B. Product Vision and Scope

Direction: **Enhanced Tanks Arena** — keep the accessible arcade combat, add original branding, better presentation, polish, and original content.

Pillars: (1) accessible combat, (2) competitive arena battles, (3) power-ups and a polished arena — **Desert is the only first-release arena; Jungle and Moon are deferred to a future content update** (DR-C14), with the code kept extensible for more arenas, (4) replayable short matches, (5) AI competition (local multiplayer is a deferred option).

**First-release scope (required)**: Android; one human vs AI; touch controls; **one arena: Desert** (owner-confirmed 2026-11-08, DR-C14; Jungle and Moon are deferred to a future content update and are not release blockers); movement/shooting; health/damage/elimination; round-based match; the existing six power-ups; camera framing; main menu and match setup (no arena selection in the first release); gameplay HUD; round and match results; pause + settings with persistence; Android lifecycle/back-button handling; Firebase, GPGS, AdMob and IAP integrations (scoped per `SERVICE_INTEGRATION_PLAN.md`); original branding and cleared content; signed AAB through Play testing tracks.

**Excluded from first release** (code stays, nothing is deleted): Windows/PC release, iOS, PC keyboard release controls, PC gamepad (KI-010), online multiplayer, shared-screen Android multiplayer, Jungle and Moon arenas (deferred, DR-C14), arena-selection UI, additional arenas beyond Desert, battle pass / live operations, any progression or monetization feature not approved in writing.

**Pending scope questions** (do not assume): cloud save / GPGS Saved Games, achievements and leaderboards content, exact Firebase services (Remote Config in particular), ad formats and placements, IAP products, tablet support, orientation.

Never silently expand scope. Optional ideas go to `MILESTONE_ROADMAP.md` as OPTIONAL, not into code.

## C. Technical Stack (discovered)

| Area | Fact | Source |
|---|---|---|
| Unity | 6000.3.6f1 | `ProjectSettings/ProjectVersion.txt` |
| Render | URP 17.3.0; `Assets/Settings/{PC,Mobile}_{Renderer,RPAsset}`; a second set in `Assets/_Tanks/Settings/Profiles` (`UniversalRenderPipelineAsset*`) | files |
| Input | Input System 1.18.0; action asset `Assets/_Tanks/Settings/Tank_Actions.inputactions`; template asset `Assets/InputSystem_Actions.inputactions` (unused by game code) | `manifest.json` |
| UI | uGUI 2.0.0 + TextMeshPro (resources under `Assets/_Tanks/Settings/TextMesh Pro`) | scripts |
| AI | AI Navigation 2.0.9, baked NavMesh assets in `Tutorial_Demo/Demo_Scenes/Demo_Scene_Data/Demo_NavMeshes` | `TankAI.cs` |
| Other packages present | Timeline, Visual Scripting, Test Framework 1.6.0, Multiplayer Center, `com.unity.ai.assistant` **2.21.0-pre.2 (pre-release)**, `com.unity.ai.inference` | `manifest.json` |
| Tests | none (no test assemblies, no `.asmdef`) | repo |
| VCS | Git, initialized 2026-11-07; remote `https://github.com/CodeDrift-Studio/Tanks.git`; branches `main`/`dev`/`xyz-backup` — see §N | `git` |
| Scripting backend (Android) | IL2CPP (`1`), min SDK 25, target SDK 0 (auto), architectures flag `6` (meaning UNVERIFIED) | `ProjectSettings.asset` |
| Identity | company `DefaultCompany`, product `Tanks`, bundle id `com.Company.ProductName` (placeholders) | `ProjectSettings.asset` |

Do not invent services, SDKs, backends or packages. **None of the following is installed yet**; they are owner-mandated for the first release and are planned in `Documentation/Development/SERVICE_INTEGRATION_PLAN.md`: Firebase (Analytics, Crashlytics, Remote Config — scope pending), Google Play Games Services, Google AdMob, Unity IAP / Google Play Billing. Installing any of them is a package change and needs approval in the milestone that owns it (M5/M6). Do not add other services, ad networks, or SDKs.

## D. Existing Architecture (STATIC)

Namespace `Tanks.Complete`. 23 scripts in `Assets/_Tanks/Scripts`.

- **GameManager** (`Managers/`) — owns `GameState {MainMenu, Game}`; `StartGame(PlayerData[])` called by the menu; spawns tanks at `TankManager.m_SpawnPoint`; sets `CameraControl.m_Targets`; runs the `GameLoop` coroutine (RoundStarting → RoundPlaying → RoundEnding); first to `m_NumRoundsToWin` (default 5) wins; on game win calls `SceneManager.LoadScene(0)`. Holds 4 tank prefab refs (menu slots) and `TankManager[] m_SpawnPoints`.
- **TankManager** — `[Serializable]` plain class (not a MonoBehaviour) wrapping one tank: caches movement/shooting/AI/canvas, applies player colour by matching material names containing `"TankColor"`, enables/disables control, `Reset()` repositions and toggles `SetActive` false→true. Has an editor-only `PropertyDrawer`.
- **TankMovement** — Rigidbody velocity/`MoveRotation` in `FixedUpdate`; tank-style control or direct (camera-relative) control when scheme is `"Gamepad"` or `m_IsDirectControl`; explosion knockback (`AddExplosionForce`); engine audio. `[DefaultExecutionOrder(-10)]`. Picks scheme in `Start`: mobile UI present & ControlIndex 1 → `Gamepad` (virtual device); else ControlIndex 1 → `KeyboardLeft`, otherwise `KeyboardRight`.
- **TankShooting** — charge-to-fire (min/max force, max charge time, cooldown), human and AI update paths, special-shell damage multiplier, `GetProjectilePosition` ballistic estimate used by AI.
- **TankHealth** — health, shield (damage reduction), invincibility toggle, death explosion (prefab instantiated in `Awake`), death = `SetActive(false)`.
- **ShellExplosion** — `OverlapSphere` on `m_TankMask` ("Players" layer), distance-scaled damage, knockback, particles; self-destruct after lifetime.
- **TankAI** — added by `TankManager` for computer tanks (also auto-added by `TankMovement.Start` in empty scenes). Seek (NavMesh path to nearest tank, repath every 0.3–0.6 s) and Flee states; drives the Rigidbody directly in `FixedUpdate`; fires via `TankShooting.StartCharging/StopCharging`.
- **TankInputUser** — clones `InputSystem.actions` JSON per tank, associates an `InputUser`; `TankMovement`/`TankShooting` read actions `Vertical`, `Horizontal`, `Fire` from the local copy.
- **Power-ups** — `PowerUp` (trigger pickup; types Speed, DamageReduction, ShootingBonus, Healing, Invincibility, DamageMultiplier), `PowerUpSpawner` (random type at fixed y=1.09, respawn cooldown 20 s), `Tank/PowerUpDetector` (applies effects via coroutines on the tank, gates on `m_HasActivePowerUp`), `PowerUpHUD`, `PowerUpFX`.
- **CameraControl** — averages active tank positions, orthographic size to fit all, SmoothDamp in `FixedUpdate`.
- **GameUIHandler** (start menu; also wires pause) + **StartMenuSlot** (4 slots, tank preview, P1/P2/CPU/Off) + **PauseMenu** (pause toggle, controls screen, "select tank" = reload active scene by build index, quit) + **MessageTextReference** (title text locator) + **MobileUIControl** / **UIVirtualJoystick** (on-screen controls; `Instance` singleton; auto-disables off-mobile).
- **Editor-only**: `PowerUpEditor`, `MoveSceneViewCamera`, `ProjectWideActionsSetup` (auto-assigns `Tank_Actions` as project-wide actions via hard-coded GUID `7fb6c1c7…`).

Relationships: Menu → `PlayerData[]` → GameManager → TankManager[] → tank components; Shell → TankHealth/TankMovement; PowerUp → PowerUpDetector → Health/Shooting/Movement/HUD; AI → TankMovement/TankShooting/NavMesh.

**Scenes** (binary-serialized): `Tutorial_Demo/Demo_Scenes/Demo_Game_{Desert,Jungle,Moon}.unity` each contain GameManager, TankManager, PauseMenu, PowerUpSpawners, CameraControl. `Assets/Scenes/SampleScene.unity` is a leftover template scene.

## E. Project Structure

### Current (as of 2026-11-06)
```
Assets/
  Scenes/SampleScene.unity               template leftover
  Settings/                              URP template assets (PC/Mobile)
  InputSystem_Actions.inputactions       template leftover
  _Tanks/
    Art/ Audio/ Prefabs/ Scripts/ Settings/
    Tutorial_Demo/  (Demo_AudioMixers, Demo_Materials, Demo_Prefabs, Demo_Scenes[+Demo_Scene_Data])
    Documentation.txt, Tanks!_Third-PartyNotice.txt
Packages/  ProjectSettings/  Documentation/Development/  PROJECT_AUDIT.md  CLAUDE.md
```
Tank prefabs (`Demo_Tank` + ATV/Heavy/Medium/Shark variants) exist **only** under `Tutorial_Demo/Demo_Prefabs/Demo_Tanks`. `Tutorial_Demo` holds the real playable game.

### Proposed target (PENDING APPROVAL — do not migrate until approved in M3)
The `Arena_Jungle` / `Arena_Moon` entries below are a **future-arena proposal only, not a first-release commitment**; the first release ships Desert only (DR-C14).
```
Assets/Game/{Scripts,Prefabs,Scenes,Art,Audio,UI,Settings,Data}
Assets/Game/Scenes/{Boot,MainMenu,Arena_Desert,Arena_Jungle,Arena_Moon}
Assets/ThirdParty/        (provenance-tracked imports)
Assets/Tests/{EditMode,PlayMode}
```
Migration must use the Unity Editor (or tooling that moves `.meta` files with assets) so GUIDs are preserved. See TECHNICAL_DECISIONS ADR-004.

## F. Coding Standards

Follow existing conventions unless there is a concrete reason not to:
- **Namespace** `Tanks.Complete` for existing code. New namespace for original code is PENDING DECISION (ADR-005); until decided, keep `Tanks.Complete`.
- **Naming**: `m_` prefix for existing serialized/instance fields (match the surrounding file); PascalCase for types, methods, properties; `k_` for constants; one MonoBehaviour per file, filename = class name.
- **Inspector exposure**: prefer `[SerializeField] private` over `public` for **new** fields; add `[Tooltip]` and `[Header]`. Do not convert existing public fields in unrelated tasks (scene/prefab serialized data keys on field names — renaming loses data; use `[FormerlySerializedAs]` if a rename is approved).
- **Component responsibility**: one concern per component; managers coordinate, tank components act. Don't grow `GameManager`/`TankAI` with unrelated features — extract only when the task needs it.
- **Null safety**: validate required references in `Awake`/`Start` with a clear `Debug.LogError` naming the object; guard `FindAnyObjectByType` results; never assume `Camera.main`, `Keyboard.current`, `InputSystem.actions` are non-null on Android.
- **Events**: prefer C# events/`UnityEvent` for new cross-system notifications; **always unsubscribe** (`OnDisable`/`OnDestroy`). Existing direct calls are acceptable; don't rewrite them casually.
- **Coroutines**: fine for timed sequences (existing style). Remember a coroutine started on a GameObject dies when that object is deactivated (tanks are deactivated on death/reset) — any state changed by a coroutine must be restored in `OnDisable`/`OnEnable` too.
- **Allocations**: no per-frame `new`, LINQ, string concat, or `GetComponent` in `Update/FixedUpdate`; cache. Avoid `renderer.materials` in loops (instantiates) — use `sharedMaterials` or `MaterialPropertyBlock` where possible. Cache `WaitForSeconds`.
- **Object pooling**: shells, explosions, pickup FX are currently `Instantiate/Destroy`. Introduce pooling only when on-device profiling (M8) shows GC spikes; use Unity's `ObjectPool<T>`.
- **Update vs FixedUpdate**: read input in `Update`; apply physics in `FixedUpdate` (existing pattern). No physics in `Update`.
- **Input System**: use the cloned per-tank action asset (`TankInputUser`) — never `UnityEngine.Input`. Action/scheme names are string-keyed (`Vertical`, `Horizontal`, `Fire`, `Pause`, `KeyboardLeft`, `KeyboardRight`, `Gamepad`); centralize in constants if you touch them.
- **Scene management**: scenes are loaded by build index today (`LoadScene(0)`, `GetActiveScene().buildIndex`). Don't change load strategy without an ADR; keep Build Settings valid.
- **ScriptableObjects**: none exist today. Introduce for data config (tank stats, match settings) only in/after M3 with an ADR.
- **Third-party services** (Firebase, GPGS, AdMob, IAP): wrap each behind a small interface/service class with **non-blocking initialization**, a defined offline/failure behavior, and a feature flag so gameplay never waits on a network call. No service code in gameplay classes (`TankAI`, `GameManager`). No service work before M5/M6 approval.
- **Errors/logging**: `Debug.LogError` for config faults, `LogWarning` for recoverable; no log spam in per-frame paths; wrap debug logs in `#if UNITY_EDITOR || DEVELOPMENT_BUILD` if frequent.
- **Performance**: target budgets are PENDING (decide before M8; ADR-009). Do not micro-optimize before profiling.
- **Comments**: existing code is heavily commented for learners. New code: comment *why*, not *what*.

## G. Unity Development Rules

- **Never hand-edit** `.unity`, `.prefab`, `.asset`, `.meta`, or `ProjectSettings/*` as text unless the files are Force-Text (**currently they are binary** — see KI-003) *and* the task explicitly requires it. Editing binary assets with text tools corrupts them. Prefer Unity Editor operations (via the user, or an available Unity MCP/Editor tool if connected — verify it works first).
- **Scenes/prefabs**: smallest possible change; never "tidy" unrelated objects; prefer prefab overrides on variants over editing the base prefab; check all variants/scenes that use a prefab before changing it.
- **GUIDs/.meta**: every asset has a `.meta`; never delete, regenerate, or copy one by hand. Move/rename only via Editor/AssetDatabase so references survive. Don't commit an asset without its `.meta`.
- **Build Settings**: must list valid scenes; index 0 is what `GameManager` restarts to. Current state is broken (KI-001). Any change to the list is a project-wide change — explain impact.
- **Packages**: no install/upgrade/remove without approval. Packages were just upgraded on 2026-11-06 (see `Logs/Packages-Update.log`); do not churn. `com.unity.ai.assistant` is pre-release — decision pending (ADR-007).
- **Input Actions**: edit `Tank_Actions.inputactions` (JSON) only deliberately; keep action and scheme names stable or update all string users. Persisted "Project-wide Actions" assignment is empty in ProjectSettings (`preloadedAssets: []`) — see KI-006.
- **URP**: two pipeline asset families exist; don't modify renderer features/quality tiers without a task. PC and Mobile assets differ intentionally.
- **Android**: package name, keystore, ARM64, IL2CPP, min/target SDK are release settings — change only in M0 (dev values) / M7 (final name, package ID) / M8 (release config) with explanation. Never commit keystores, passwords, `google-services.json` secrets beyond what the owner approves, AdMob/Firebase/Play keys or service-account files without owner review. Package ID must be fixed **before** Firebase/GPGS/AdMob/IAP registration (those bind to it).
- **Windows**: out of first-release scope; don't change Windows settings.
- **Generated folders**: `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `obj/`, `*.csproj`, `*.slnx` are generated — never hand-edit, never commit.

## H. Platform Requirements

### Android (the only first-release platform)
- Input: on-screen joystick (`MobileControlCanvas.prefab`, `MobileUIControl`); single human player vs AI. **Presence in the 3 scenes is UNKNOWN** (scenes are binary; string search is inconclusive) — verify in Editor (KI-008). Android input init has statically suspect null paths (`Keyboard.current`, scheme access) — M1, task AND-01.
- UI: portrait is made readable but the game is landscape; orientation/tablet support PENDING; no safe-area code exists (M4).
- Lifecycle: no pause-on-background, no Back-key handling today (M3).
- Performance: use `Mobile_RPAsset`/`Mobile_Renderer` (which asset Android uses is UNVERIFIED); budgets PENDING; test low/mid-tier device (M8).
- Build: IL2CPP, ARM64, signed AAB for Play; package name placeholder; min SDK 25 currently (second-hand, UNVERIFIED); Android player settings are in a binary `ProjectSettings.asset` and must be read in the Editor.
- **Physical-device testing is required**; Editor device simulator is not sufficient.

### Deferred (out of first-release scope; code retained)
- Windows PC: keyboard (P1 `WASD`+`Space`, P2 arrows+`Enter`, pause `Esc`) works in the Editor by design; gamepad appears unwired (KI-010) — **deferred, not a blocker**.

## I. Testing and QA Standards

| Level | Claude can do | Needs Unity Editor / device / human |
|---|---|---|
| Static inspection | Read code/config, grep, cross-check refs | — |
| Compilation | Only if Unity batch-mode or an Editor tool is available and *actually run*; otherwise state "not compiled" | Editor compile / `-batchmode -quit` |
| EditMode tests | Write them (M2/M8); run only if the Unity CLI/MCP is available | Test Runner |
| PlayMode tests | Same | Test Runner |
| Manual gameplay | Provide checklists | A person plays each arena |
| Android build / device test (primary) | Prepare steps, read logs | Editor build, real device, ADB logs |
| Service tests (Firebase, GPGS, AdMob test ads, IAP license-tester purchases) | Prepare steps | Real device + console accounts owned by the owner |
| Windows build | Out of first-release scope | — |
| Regression | Maintain `TESTING_CHECKLIST.md` | Re-execute after changes |

Rules: never write PASS without execution evidence (date, build/commit, who/what ran it). "Compiles" requires an actual compile result. A milestone with unexecuted device tests becomes **REVIEW REQUIRED**, not COMPLETED.

## J. Development Workflow (per task)

1. **Understand** the task; map it to a milestone and requirement (traceability).
2. **Inspect** the relevant scripts/scenes/prefabs; check `KNOWN_ISSUES.md`.
3. **Identify dependencies** (what references this? other prefabs/scenes? inspector fields?).
4. **Plan** — state affected systems, steps, risks, validation method.
5. **Approval** when required (§K and Rule 7); otherwise proceed.
6. **Implement minimal changes**; one logical task at a time.
7. **Validate** with what is actually available; record what could not be validated.
8. **Document** — milestone file, status, log, issues, decisions, NEXT_SESSION (see §L).
9. **Update milestone status** honestly.
10. **Summarize** (§M end-of-session format).

## K. Project Safety Rules

- Never rewrite an existing system without evidence and justification; prefer extending.
- Never refactor unrelated code during a feature task.
- Never delete assets without explicit approval; never rename/move Unity assets casually; preserve `.meta`/GUIDs.
- Never upgrade Unity or packages without approval; never change project-wide settings without explaining impact.
- Never claim a test passed if it was not executed; never claim it compiles/builds/runs unless verified.
- Never invent missing project functionality or history; never fabricate log entries.
- Never silently expand MVP scope.
- Never commit, push, or publish without explicit permission (the one-time repository setup of 2026-11-07 was explicitly approved; see §N). Follow §N for all Git work.
- Never write secrets (keystores, passwords, tokens) into the repo or docs.
- Licensing: never assume sample content is freely redistributable commercially; record provenance; unresolved licensing = release blocker (KI-007). **Licensing investigation starts in M0/M1 (early); M7 is the final clearance gate.** Make no legal conclusions.
- Services/monetization: do not add SDKs, create console projects, register apps or write integration code outside the owning milestone (M5 Firebase/GPGS, M6 AdMob/IAP) and without approval. Never put real ad unit IDs or product IDs into a test path; use test IDs until release.
- **Approval required before**: major refactors, asset deletion, folder restructuring, package/Unity changes, project-wide settings, new architectural dependencies, scope expansion, changes with asset-compatibility risk.

## L. Documentation Maintenance

Files live in `Documentation/Development/`. After any meaningful task update, as relevant: the active `Milestones/Mx_*.md` (tick boxes only with evidence), `MILESTONE_STATUS.md`, `DEVELOPMENT_LOG.md` (append-only, real entries), `KNOWN_ISSUES.md`, `TECHNICAL_DECISIONS.md` (only for decisions actually made), `TESTING_CHECKLIST.md` (results with evidence), `NEXT_SESSION.md` (always at end of a meaningful session). Update `CLAUDE.md` only when persistent instructions or architectural facts change. Don't duplicate content across files — link instead.

## M. Session Procedure and Milestone Execution Rules

**Start of every session**: read `CLAUDE.md` → `MILESTONE_STATUS.md` → `NEXT_SESSION.md` → the active milestone file → `git status`/recent log **if** a repo exists → relevant source → pick the next incomplete task → check blockers/pending approvals.
**Before implementing**: explain the task, affected systems, plan, risks, validation.
**After implementing**: review the diff, run available checks, record results, update docs/status, write the handoff.
**End-of-session summary** must list: Completed · Modified files · Validation performed · Validation NOT performed · Known issues · Pending decisions · Recommended next task.

**Rules**
1. One milestone at a time; don't drift into the next.
2. One logical task at a time; focused, reviewable changes.
3. Inspect before modifying (read, find references, side effects, explain).
4. Preserve existing systems; rewrite only if the existing one cannot meet an approved requirement.
5. Verify before completion; say explicitly when the Editor/device was unavailable.
6. Update documentation (§L).
7. Respect approval boundaries (§K).
8. Traceability: milestone → requirement → files/assets → validation evidence.

**Quality gates** — a milestone is COMPLETED only if: all mandatory tasks done; acceptance criteria met; relevant tests executed or explicitly marked unavailable-with-owner-sign-off; no unresolved critical blocker; docs updated; changes reviewable. Tasks needing the Editor or a device → **REVIEW REQUIRED**.

**Milestones**: M0 Stabilization → M1 Android Gameplay Vertical Slice → M2 Reliability & Balancing → M3 Product Foundation & Flow → M4 Android UI/UX → M5 Firebase, GPGS & Player Data → M6 Monetization (AdMob + IAP) → M7 Original Content, Branding & Commercial Identity → M8 Optimization, QA & Release. Details: `MILESTONE_ROADMAP.md`, `Milestones/`. Superseded PC-oriented plan: `Documentation/Development/Archive/` (historical only).
**Active milestone**: see `MILESTONE_STATUS.md` (**M0 — Project Stabilization, IN PROGRESS**).

---

## N. Git Policy (owner-approved 2026-11-07)

Full detail: `Documentation/Development/GIT_WORKFLOW.md`. Remote: `https://github.com/CodeDrift-Studio/Tanks.git`.

- **`main`** — stable/release. Never develop on it. Only receives `dev` via reviewed pull request, after full-diff review, validation evidence and owner approval. Never merge `dev` into `main` automatically.
- **`dev`** — active development; **default working branch** (M0 onward). Verify the branch before any change. Logical, reviewable commits; don't mix unrelated changes.
- **`xyz-backup`** — archive of removed/deferred material only. Never develop on it; never merge it into `dev`/`main`; not a full backup. Before archiving: record original path, reason, date; verify dependencies/GUIDs/scene/prefab refs (text search is not proof of "unused"); verify it's on the branch; only then remove from `dev` (with approval).
- Feature work: `feature/*`, `fix/*`, `experiment/*` from `dev`, merged back into `dev` after review. No other long-lived branches without approval.
- **Never**: force-push, reset/rewrite published history, `--allow-unrelated-histories` (without approval), discard uncommitted user work, switch branches with uncommitted changes unprotected, commit `Library/ Temp/ Obj/ Logs/ UserSettings/ Build*/`, delete `.meta` files, commit secrets/keystores.
- **Commit/push only when authorized** for the session. Commit messages end with the Claude co-author trailer; on PowerShell use `git commit -F <file>`.
- **Session start**: `git status --short --branch`; confirm branch/tree; fetch if appropriate; confirm task and milestone. **Session end**: review diff, check for stray files, validate, update docs, summarize, commit/push only if authorized.
- Claiming a branch exists remotely requires `git ls-remote --heads origin` evidence.
