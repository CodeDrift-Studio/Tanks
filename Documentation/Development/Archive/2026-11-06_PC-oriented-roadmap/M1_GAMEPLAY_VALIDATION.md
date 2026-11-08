> **SUPERSEDED 2026-11-08 — historical reference only.** This is the original Windows+Android MVP milestone from 2026-11-06. The authoritative plan is `MILESTONE_ROADMAP.md` (M0–M8, Android-first). Do not execute tasks from this file; its content is kept unchanged below for traceability.

# M1 — Gameplay Validation

**Status: NOT STARTED** · Complexity: M · Tags: `[Editor]` `[Build]` `[Device]` `[Approval]`

## 1. Objective
Find out what actually works at runtime; fix confirmed defects with minimal changes.

## 2. Current repository state
All gameplay code present (STATIC), never run by Claude. Potential defects: KI-P01 (AI nulls), KI-P02 (power-up state leak), KI-P04, KI-P05, KI-P06. No tests.

## 3. Scope
Movement, shooting, health/damage, AI, six power-ups, camera, round transitions, match results, scene restart, arena validation (Desert, Jungle, Moon). No new features.

## 4. Prerequisites
M0 accepted (valid Build Settings, repo baseline, Force Text).

## 5. Tasks
- [ ] `[Editor]` Execute `TESTING_CHECKLIST.md` sections 4 (Gameplay), 5 (AI), 6 (Multiplayer), 7 (Power-ups), 8 (UI) in the Editor, per arena; record PASS/FAIL with evidence.
- [ ] `[Editor]` Soak test: 4 CPU tanks, ≥30 rounds per arena; capture console.
- [ ] Triage each failure into `KNOWN_ISSUES.md` (confirmed vs potential).
- [ ] Confirm or dismiss KI-P02 (speed/cooldown/`m_HasActivePowerUp` after death/reset).
- [ ] Confirm or dismiss KI-P01 (AI null paths).
- [ ] Fix confirmed P0/P1 defects, one per task, minimal diff, each with a regression check.
- [ ] Add a PlayMode smoke test (spawn 2 tanks, run a round) if the test runner is usable `[Editor]` (introduces asmdef — see ADR-005; approval needed).
- [ ] Record arena validation (spawn points, NavMesh coverage, power-up spawner positions, missing refs).

## 6. Technical implementation notes
- Fix pattern for KI-P02 (if confirmed): restore baseline in `OnDisable`/`OnEnable` (stored base values) rather than relying on coroutine completion.
- AI fixes: guard null target/path; don't change behavior tuning.
- Don't refactor `TankAI` structure; keep fixes local.

## 7. Files/systems likely affected
`Scripts/Tank/TankAI.cs`, `Tank/PowerUpDetector.cs`, `Tank/TankShooting.cs`, `Tank/TankMovement.cs`, `Managers/*`, scene/prefab Inspector values only if required.

## 8. Risks
Binary-scene edits (do in Editor only); behavior changes affecting feel; findings expanding into redesign (route to M3/M4 instead).

## 9. Validation steps
Re-run the affected checklist rows after each fix; soak test again; compile check.

## 10. Acceptance criteria
- Every Gameplay/AI/Power-up/Multiplayer/UI checklist row is PASS or tracked as an issue.
- No open P0/P1 gameplay defect.
- Full match completes in all three arenas on Windows build.

## 11. Definition of Done
Acceptance met; evidence logged; KI-P01/P02 resolved or dismissed with evidence; docs updated.

## 12. Completion summary
_(empty)_
