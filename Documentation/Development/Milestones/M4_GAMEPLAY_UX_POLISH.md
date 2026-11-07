# M4 — Gameplay and UX Polish

**Status: NOT STARTED** · Complexity: M–L · Tags: `[Editor]` `[Build]` `[Device]` `[Approval]`

## 1. Objective
A clear HUD, results, settings and pause experience with good feedback.

## 2. Current repository state (STATIC)
Round/win messages are a single TMP text (`MessageTextReference`) written by `GameManager`; per-tank world-space health + aim sliders; power-up icons via `PowerUpHUD`; pause menu with controls screen; no settings UI; no match results screen beyond text; no title screen.

## 3. Scope
Improved HUD, round/match results, settings menu, pause flow, controls feedback, gameplay feedback (hit/death/pickup feedback, audio mixing). **OPTIONAL**: AI improvements — only where M1 evidence shows a need.

## 4. Prerequisites
M3 accepted (menu flow/settings persistence). Art/UX direction for the HUD (PENDING).

## 5. Tasks
- [ ] Define HUD requirements (what players must see: health, ammo/charge, power-up, round/score) — owner confirmation.
- [ ] Gameplay HUD improvements (readable on small Android screens).
- [ ] Round-result and match-result screens (winner, scores, rematch/menu).
- [ ] Settings menu (audio volume via mixer, controls hints, graphics quality tier if justified).
- [ ] Pause flow: resume / settings / controls / quit-to-menu consistent on PC and Android.
- [ ] Controls feedback (touch joystick visuals; PC key hints; gamepad prompts).
- [ ] Gameplay feedback (hit flash, camera shake only if justified, audio balance); verify mixer wiring (UNVERIFIED today).
- [ ] **OPTIONAL** `[Approval]` AI tuning where M1 shows weakness.
- [ ] Update checklist UI rows (U-01…U-05).

## 6. Technical implementation notes
Keep uGUI + TextMeshPro (existing). Use canvas scalers and safe-area utility. No per-frame allocations in HUD updates. Don't expand into new mechanics.

## 7. Files/systems likely affected
`UI/*`, `Prefabs/UI/*`, `GameManager` (result events), audio mixer assets, `PowerUpHUD`.

## 8. Risks
UI scope creep; platform scaling issues; art dependencies; breaking existing prefab references.

## 9. Validation steps
Execute UI checklist on Windows + Android; aspect-ratio matrix; playtest feedback.

## 10. Acceptance criteria
HUD, results, settings, pause flow implemented and working on both platforms; settings persist; no UI clipping on the test matrix.

## 11. Definition of Done
Criteria met with evidence; docs/status updated; optional items clearly marked done/deferred.

## 12. Completion summary
_(empty)_
