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
- Status: ACCEPTED

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
- Status: PROPOSED

## ADR-009 — Performance budgets
- Context: none defined. Status: PENDING APPROVAL (set before M6: target FPS, min Android device, memory, GC/frame).

## ADR-010 — Persistent settings storage
- Context: no persistence exists. Proposed: `PlayerPrefs` (or JSON file) behind one small settings service, in M3. Status: PROPOSED

## ADR-011 — Scene flow
- Context: restart relies on `LoadScene(0)`/active build index; no boot or menu scene. Proposed: Boot → MainMenu → Arena scenes with explicit scene names/indices in M3. Status: PROPOSED
