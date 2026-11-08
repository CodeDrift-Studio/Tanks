# M2 — Gameplay Reliability and Balancing

**Status: NOT STARTED** · Effort: 14–25 days (impl 8–15, test 6–10) · Tags: `[Editor]` `[Device]` `[Approval]`
References: `RELEASE_GAP_ASSESSMENT.md` §5, AI-01, GP-02, GP-03; KI-P01, KI-P02, KI-P04–P06.

## 1. Objective
Make the single-player game reliable and fair in the **Desert** arena (the only first-release arena, DR-C14) on device. Jungle and Moon are deferred and out of this milestone.

## 2. Scope
- AI null-reference and target-slot risks (AI-01): `StartFleeing`, `FleeUpdate`, unassigned `m_AllTanks` entries, per-repath `NavMeshPath` allocation, bot input/`Fire` binding dependence on a keyboard device, `TankMovement` vs `TankAI` Rigidbody writes.
- Tank lifecycle and reset (death, `SetActive` cycling, coroutine-held state).
- All six power-ups: Healing, Speed, Shell (`DamageMultiplier`), Shooting (`ShootingBonus`), Shield (`DamageReduction`), Invincibility — pickup, activation, duration, expiry, cleanup, interactions.
- Match/round scoring (first-to-5, draws, tie behavior), state reset between rounds (pickups, in-flight shells).
- AI difficulty levels (design per DR-P18) and gameplay balance (damage, speeds, cooldowns, power-up values/spawn rates).
- Desert arena validation (spawn points, NavMesh coverage, colliders/bounds, power-up spawners, missing references). Jungle/Moon validation is deferred to the content update.
- Scene and match-state defects found during testing.

## 3. Out of scope
Menus, results screens, settings (M3/M4); service integration; new tanks/arenas/mechanics (including the deferred Jungle and Moon arenas); PC/two-human balance; pooling/optimization unless profiling (M8) demands it.

## 4. Prerequisites
M1 exit gate. DR-P12 (arena count: resolved, Desert only) and DR-P18 (difficulty tiers, aim aid) preferred before balancing.

## 5. Tasks
- [ ] `[Device]` AI soak: 100+ bot rounds in Desert (can use bot-vs-bot via a dev-only setup if approved); console/`logcat` clean.
- [ ] `[Approval]` AI hardening fixes (AI-01), one per defect, minimal diffs.
- [ ] `[Device]` Power-up matrix: each of six types x {pickup, expiry, death mid-effect, round reset, pickup while another active}; record against P-xx rows.
- [ ] Scoring/round tests: simultaneous death -> draw; win at 5; restart; verify no `timeScale` leak.
- [ ] `[Approval]` Difficulty design and implementation (parameters for reaction, aim error, fire cadence, flee thresholds) — minimal, data-driven only if ADR-006 approved.
- [ ] Balance pass with playtest notes (damage, speed, cooldown, power-up duration/spawn).
  - Note 2026-11-08: an initial four-tank balance pass was applied and accepted as the baseline (DR-C15). This task stays open: no playtest notes or win-rate data exist; no retuning while the gameplay freeze (DR-C16) is in force.
- [ ] `[Editor][Device]` Desert arena validation checklist (G-08, Desert only); fix Desert defects (Editor only for scenes).
- [ ] `[Approval]` EditMode/PlayMode tests for pure logic that can be tested (power-up state restore, scoring) if asmdef approach approved (ADR-005).
- [ ] Triage all findings into `KNOWN_ISSUES.md`.

## 6. Dependencies
M1; devices; Editor for scene work; ADR-005/006 for tests/data.

## 7. Risks
Balance is subjective and iterative; fixes touching AI timing change feel; per-arena NavMesh problems need scene edits (binary until Force Text).

## 8. Required approvals
Code fixes; difficulty/balance values (owner sign-off); scene edits; asmdef introduction.

## 9. Validation procedure
Soak runs on device with log capture; power-up matrix on device; checklist rows re-run after each fix; compile check; test runner if tests exist (state if not run).

## 10. Acceptance criteria
- No open P0/P1 gameplay defect; no AI exception in soak runs.
- All six power-ups behave per spec and clean up on every exit path.
- Scoring and round/match state correct in all tested sequences, in Desert.
- Difficulty tiers (if approved) distinguishable in playtest.
- Gameplay checklist rows PASS with evidence or tracked as issues.

## 11. Evidence to record
Soak logs (counts, device, build), power-up matrix results, balance playtest notes (who/when), fix commits, test results with run evidence.

## 12. Exit gate
Acceptance met with device evidence and owner review. Next: M3.

## Completion summary
_(empty)_
