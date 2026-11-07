# M3 — Game Foundation

**Status: NOT STARTED** · Complexity: L · Tags: `[Editor]` `[Build]` `[Device]` `[Approval]`

## 1. Objective
Turn the tutorial into an owned game structure with a proper flow, config and persistence.

## 2. Current repository state (STATIC)
Single-scene-per-arena flow: menu (`Menus.prefab`) is inside each demo scene; restart = reload build index 0; menu fixed to 4 tank prefabs from `GameManager`; no arena selection, boot scene, settings persistence or data config; all content under `Tutorial_Demo`; namespace `Tanks.Complete`; template leftovers present.

## 3. Scope
Original identity (name TBD), boot + menu flow, match configuration, arena selection, match settings (rounds-to-win, etc.), game config data, persistent settings, folder migration. Not: new gameplay mechanics (M5), polished UI (M4).

## 4. Prerequisites
M0–M2 accepted. Approvals/decisions: final working name (PENDING), ADR-004 (migration), ADR-005 (namespace/asmdef), ADR-006 (ScriptableObjects), ADR-010 (settings storage), ADR-011 (scene flow).

## 5. Tasks
- [ ] `[Approval]` Decide working title and identity guidelines (naming/branding is PENDING DECISION).
- [ ] `[Approval]` Approve target structure (ADR-004) and namespace/asmdef strategy (ADR-005).
- [ ] `[Editor]` Migrate content out of `Tutorial_Demo` in small steps (Editor move only; `.meta` preserved); one commit per step; verify scenes after each step.
- [ ] Remove/merge template leftovers (`SampleScene`, template input asset) — **explicit approval before deletion** (KI-011).
- [ ] Design scene flow: Boot → MainMenu → Arena (ADR-011); update `GameManager`/`PauseMenu` scene loading accordingly.
- [ ] Arena selection UI (3 arenas) + match configuration (players/slots, rounds to win).
- [ ] Introduce config data (ScriptableObjects or constants) only as needed (ADR-006); replace fixed 4-prefab assumption only if required.
- [ ] Persistent settings (volume, controls hints, etc.) via one service (ADR-010).
- [ ] Update `CLAUDE.md` §E (structure) and §C after migration.

## 6. Technical implementation notes
Prefer extending `GameManager`/`GameUIHandler` over rewriting; keep `PlayerData` as the menu→game contract; keep `Tanks.Complete` until the namespace decision. Rename C# types only with `[FormerlySerializedAs]`/GUID safety in mind. Do the folder move before functional changes.

## 7. Files/systems likely affected
`Managers/GameManager.cs`, `UI/GameUIHandler.cs`, `UI/StartMenuSlot.cs`, `UI/PauseMenu.cs`, new settings/config scripts, scenes (Boot/MainMenu), Build Settings, everything moved.

## 8. Risks
GUID/reference breakage; scope creep; hard-coded 4-slot menu; merge/diff noise; renaming breaking serialized data.

## 9. Validation steps
After each migration step: open all scenes (no missing refs), play a match, Windows build smoke test; settings persist across restart.

## 10. Acceptance criteria
- Game starts from Boot/Menu, selects arena and match config, plays a match, returns to menu — on Windows (and Android if M2 done).
- Settings persist across app restarts.
- Content located under the approved structure; no references to `Tutorial_Demo` remain (or its role explicitly decided).
- Docs updated.

## 11. Definition of Done
Criteria met with evidence; ADRs recorded; known issues updated.

## 12. Completion summary
_(empty)_
