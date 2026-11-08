# TECHNICAL DECISIONS (ADR log)

Only decisions actually made are `ACCEPTED`. Everything else is `PROPOSED` or `PENDING APPROVAL`. Do not record a decision here until it is made.

Status values: ACCEPTED · PROPOSED · PENDING APPROVAL · SUPERSEDED · REJECTED.

---

## ADR-001 — Product direction: Enhanced Tanks Arena
- Date: 2026-11-06 (decided by owner before this documentation task)
- Context: project is the Unity Tanks! tutorial; owner wants an original commercial game.
- Options: finish tutorial as-is; enhance (keep core, original identity/content); full rebuild.
- Decision: **Enhanced Tanks Arena** — keep the accessible arcade combat, add original branding/polish/content.
- Consequences: existing gameplay is reused; licensing and originality must be resolved (KI-007); `Tutorial_Demo` content becomes game content.
- Status: ACCEPTED

## ADR-002 — Platforms and multiplayer scope
- Date: 2026-11-06 (owner)
- Decision: Windows + Android; local multiplayer and AI only; no online in MVP; Android shared-screen multiplayer **not** assumed.
- Status: **SUPERSEDED 2026-11-08 by ADR-012** (Android-only first release). Kept as history; the Windows/PC and local-multiplayer parts are no longer first-release scope.

## ADR-003 — Treat static audit findings as unverified
- Date: 2026-11-06
- Decision: nothing is "working" until verified in Editor/device (M1). Defects found by reading code are "potential".
- Status: ACCEPTED

## ADR-004 — Target folder structure and migration method
- Context: real game lives in `Assets/_Tanks/Tutorial_Demo`; sample/demo naming everywhere. Assets are binary-serialized today.
- Options: (a) keep in place; (b) migrate to `Assets/Game/...` via Editor with GUID preservation; (c) copy-and-rebuild.
- Proposed: (b) in M3, after Force Text + git baseline, in small reviewable commits.
- Consequences: large diff; must keep `.meta`; scenes/prefabs reference by GUID so Editor moves are safe.
- Status: PENDING APPROVAL

## ADR-005 — Code namespace and assembly definitions
- Context: all code is `Tanks.Complete` (sample name); no `.asmdef`; tests need assemblies.
- Options: keep namespace; rename to the game's namespace; add asmdefs (`Game.Runtime`, `Game.Editor`, tests).
- Proposed: keep `Tanks.Complete` until final game name is decided; add asmdefs in M3/M6 when tests are introduced.
- Status: PROPOSED

## ADR-006 — Game data configuration (ScriptableObjects)
- Context: tank stats live in prefab inspector fields; match config in `GameManager`; none data-driven; menu fixed at 4 prefabs.
- Proposed: introduce ScriptableObjects for tank stats / match settings / arena list in M3 only if needed by arena selection and settings.
- Status: PROPOSED

## ADR-007 — `com.unity.ai.assistant` (pre-release) and other unused packages
- Context: pre-release package + AI inference, Visual Scripting, Timeline, Multiplayer Center present; none used by game code.
- Options: keep; remove before release.
- Proposed: decide in M0/M7; no removal without owner approval; privacy/size review in M7.
- Status: PENDING APPROVAL

## ADR-008 — Input strategy for PC gamepad and Android
- Context: `Gamepad` scheme exists but desktop code only activates `KeyboardLeft/Right`; Android uses virtual device paired to ControlIndex 1; `InputSystem.actions` used by `TankInputUser.Awake`.
- Options: extend `TankMovement.Start`/menu to assign devices per slot; or restructure input assignment in `TankInputUser`.
- Proposed: extend existing path minimally (no input rewrite) after M1 confirms behavior.
- **2026-11-08 update**: the PC gamepad / desktop-keyboard part is out of first-release scope (ADR-012). The remaining question is only the **Android virtual-device path** and null-safe input initialization (M1, AND-01); no input rewrite is proposed.
- Status: PROPOSED (narrowed to Android)

## ADR-009 — Performance budgets
- Context: none defined. Status: PENDING APPROVAL (set before M8, ideally during M3: target FPS, min Android version/device class, memory, APK/AAB size, GC/frame). See DR-P8, DR-P10.

## ADR-010 — Persistent settings storage
- Context: no persistence exists. Proposed: `PlayerPrefs` (or JSON file) behind one small settings service, in M3. Status: PROPOSED

## ADR-011 — Scene flow
- Context: restart relies on `LoadScene(0)`/active build index; no boot or menu scene. Proposed: Boot → MainMenu → Arena scenes with explicit scene names/indices in M3. Status: PROPOSED
- Note 2026-11-08 (DR-C14): the first release has one arena (Desert), so the "Arena scenes" set is a single scene. Keep the arena as data (scene name/id in the match config) so Jungle/Moon can be added later without re-architecture. Today's `LoadScene(0)` and `PauseMenu` reload-by-build-index both resolve to Desert when Desert is the only/first enabled scene (STATIC).

---

## Android-first revision (2026-11-08) — ADR-012 … ADR-016
Product (business) decisions and pending items are tracked in `DECISION_REGISTER.md`; ADRs below record the technical consequences.

## ADR-012 — Android-only first release; single-player vs AI
- Date: 2026-11-08 (owner)
- Context: earlier scope was Windows + Android with local multiplayer (ADR-002).
- Decision: **Android is the only first-release platform. Initial gameplay is single-player vs AI. Touch controls are mandatory.** Windows/PC, iOS, PC gamepad, PC keyboard release controls, online multiplayer and shared-screen Android multiplayer are **out of first-release scope**. ~~Existing Desert, Jungle and Moon arenas are the proposed first-release content (final count PENDING, DR-P12).~~ **Amended 2026-11-08 (DR-C14): the first release ships one arena, Desert; Jungle and Moon are deferred to a future content update.** Related code and assets are **retained, not deleted**.
- Consequences: supersedes ADR-002; narrows ADR-008; KI-010 (PC gamepad) becomes deferred; Windows build is no longer an acceptance item.
- Status: ACCEPTED

## ADR-013 — Business model and mandatory services
- Date: 2026-11-08 (owner)
- Decision: **Free-to-Play with Ads + IAP.** **Google AdMob, Unity IAP / Google Play Billing, Firebase and Google Play Games Services are mandatory.** The scope of each service is **not** assumed: ad formats/placements/frequency, IAP catalog/pricing, exact Firebase services (incl. Remote Config), GPGS achievements/leaderboards, and cloud save remain PENDING (DR-P3…P7).
- Consequences: reverses the earlier "IAP / accounts / cloud saves out of MVP" statements for IAP and Play Games sign-in (cloud save remains undecided); adds privacy/consent, age-group (DR-P13) and Play compliance work; package installs still need per-milestone approval (M5/M6). Detail: `SERVICE_INTEGRATION_PLAN.md`.
- Status: ACCEPTED (scope items PENDING)

## ADR-014 — Milestone structure M0–M8
- Date: 2026-11-08 (owner-approved plan, as given in the task instructions)
- Decision: M0 Stabilization · M1 Android Gameplay Vertical Slice · M2 Gameplay Reliability & Balancing · M3 Product Foundation & Game Flow · M4 Android UI/UX & Player Experience · M5 Firebase, GPGS & Player Data · M6 Monetization (AdMob + IAP) · M7 Original Content, Branding & Commercial Identity · M8 Android Optimization, QA & Release. Licensing investigation begins early (M0/M1); M7 is the final clearance gate.
- Consequences: old M1–M7 definitions archived (`Archive/2026-11-06_PC-oriented-roadmap/`); effort figures in the roadmap are estimates, not commitments.
- Status: ACCEPTED

## ADR-015 — Service integration architecture
- Date: 2026-11-08
- Proposal: each service (Firebase, GPGS, AdMob, IAP) sits behind one façade interface with a null/offline implementation, feature flag, non-blocking time-boxed initialization, and defined failure/offline behavior; no service code in gameplay classes; consent gates data collection and personalized ads; test IDs only until release.
- Status: PROPOSED (approve before M5 starts)

## ADR-016 — Early licensing investigation
- Date: 2026-11-08 (owner)
- Decision: the sample-asset and third-party licensing investigation starts early (M0/M1: provenance inventory, facts only) rather than waiting for M7; M7 remains the final clearance gate; unresolved licensing remains a release blocker (KI-007). Claude makes no legal conclusions.
- Status: ACCEPTED

## Superseded / narrowed decisions
| ADR | State |
|---|---|
| ADR-002 Platforms and multiplayer scope | SUPERSEDED by ADR-012 |
| ADR-008 Input strategy | Narrowed to Android (PC gamepad part out of scope) |
| ADR-009 Performance budgets | Needed before M8 (milestone reference updated) |

## Pending technical decisions (unchanged unless noted)
ADR-004 folder migration, ADR-005 namespace/asmdef, ADR-006 data config, ADR-007 packages (now also includes whether pre-release `com.unity.ai.assistant` stays), ADR-009 budgets, ADR-010 settings storage, ADR-011 scene flow, ADR-015 service architecture. Product-level pending items: `DECISION_REGISTER.md` §B.
