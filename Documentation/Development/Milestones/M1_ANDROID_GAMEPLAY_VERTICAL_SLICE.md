# M1 — Android Gameplay Vertical Slice

**Status: IN PROGRESS (formally opened 2026-11-08, after M0 was closed COMPLETE on owner-reported acceptance).** Scope: Android only; Desert arena only; one human vs AI; touch controls; complete core combat and match flow; AI movement, targeting, navigation and shooting; no online or shared-screen multiplayer. One task, the KI-P08 AI fix, was completed before the formal start and is preserved below (ticked, with its owner-reported OnePlus 12 Desert 1v1 evidence). It does **not** make the vertical slice complete: 7 of 9 tasks and all acceptance criteria in §10 remain open (the GP-01 power-up lifecycle task was completed afterwards, see §5). · Effort: 9–18 days (impl 6–12, test 3–6) · Tags: `[Editor]` `[Build]` `[Device]` `[Approval]`
References: `RELEASE_GAP_ASSESSMENT.md` §5–§6, AND-01, GP-01; KI-006, KI-008, KI-P01, KI-P02, KI-P03; M0_BASELINE §6–§7.

## 1. Objective
One complete human-vs-one-AI match, controlled by touch, running on a physical Android device, with the `logcat` reviewed.

## 2. Scope
- Android input initialization (`TankInputUser`, `TankMovement` scheme/device paths; null guards for `Keyboard.current`, `InputSystem.actions`, `Camera.main`, `controlScheme`).
- Confirm/place the on-screen controls (`MobileControlCanvas`) in the arena used for the slice; virtual joystick; touch shooting (hold-to-charge, release-to-fire).
- Human vs one AI; shooting, damage, destruction; AI targeting and navigation.
- Power-up lifecycle fixes (GP-01; KI-P02) — restore base values and clear the active flag on disable/reset.
- One complete first-to-5 match; `adb logcat` inspection.
- Start licensing evidence collection for KI-007 (inventory only; no legal conclusions).

## 3. Out of scope
Menu redesign, settings, results screens (M3/M4); balancing and AI difficulty (M2); services and ads (M5/M6); Windows/PC input, gamepad (KI-010), two-human play; performance optimization (M8).

## 4. Prerequisites
M0 exit gate — MET 2026-11-08 (owner-reported; valid Build Profiles, Android dev build runs on device). Owner approval for the minimal code fixes (CLAUDE.md §K).

## 5. Tasks
- [ ] `[Device]` Capture a baseline `logcat` of the unmodified build through menu -> start -> a few seconds of play (reproduces or dismisses SR-1/SR-2/SR-3 before any change). Record in the log.
- [ ] `[Editor]` Open Desert (first-release arena; Jungle and Moon are deferred, DR-C14): confirm presence/placement of `MobileControlCanvas` and `MobileUIControl`, joystick + fire + pause controls, `m_SpawnPoints` length, EventSystem input module (KI-008, D6).
- [ ] `[Approval]` Implement Android input guards (AND-01): null-safe user pairing when no keyboard exists; scheme access guarded; clear `Debug.LogError` naming the object.
- [ ] `[Device]` Verify joystick movement and touch fire (charge/release, cooldown) with multi-touch (joystick + fire simultaneously).
- [x] `[Editor][Device]` Confirm or dismiss the AI target-list issue with fewer than 4 tanks (SR-3 / new KI-P08); apply minimal guard if confirmed (full AI hardening is M2/AI-01). **DONE for 1 human + 1 bot in Desert, 2026-11-08:** confirmed on device (bot idle + NRE in `TankAI.SeekUpdate`); minimal fix in `TankAI.cs` only; owner-reported on OnePlus 12 dev build (Desert): bot moves and shoots, NRE no longer reproduces. The Editor `m_SpawnPoints` check was done statically from scene YAML (4 entries in each arena). Remaining cases (other player counts, multi-round, lifecycle, soak, all in Desert; Jungle/Moon deferred) are pending in `TESTING_CHECKLIST.md`. Evidence: `KNOWN_ISSUES.md` KI-P08, `DEVELOPMENT_LOG.md` 2026-11-08.
- [x] `[Approval]` Power-up lifecycle fix (GP-01): restore speed/turn/cooldown and clear `m_HasActivePowerUp` when the tank is disabled or reset; clear HUD state. **DONE 2026-11-08 (owner-verified; Claude static only):** implemented with GP-02 (shooting reset), GP-03 (AI reset), GP-04 (HUD), GP-08 and GP-10 guards; see `KNOWN_ISSUES.md` KI-P02, KI-P12, KI-P13. Acceptance criterion on power-ups (§10) is not formally signed off pending device evidence.
- [ ] `[Device]` Play one complete first-to-5 match vs one AI on a physical device in one arena; note frame behavior subjectively (no numbers claimed).
- [ ] Review `logcat` for exceptions/warnings; file each in `KNOWN_ISSUES.md` (confirmed vs potential).
- [ ] Update `TESTING_CHECKLIST.md` rows (A-01…A-06, G-01…G-05, AI-01, P-07) with evidence.

> Gameplay freeze (DR-C16, 2026-11-08): until the additional-device Android report arrives, no tank stat or gameplay-mechanic changes. AND-01 Android input guards are exempt from the freeze (DR-C19; unrelated gameplay changes are not), and the per-fix approval step in §8 is unchanged.

## 6. Dependencies
M0; physical device; Unity Editor; ADR-008 (input strategy, now Android-only); owner approval for code changes.

## 7. Risks
Never run on a device -> unknown failures may be larger than static review suggests; binary scenes (edit only in Editor); fix creep into redesign (route to M2/M3); touch ergonomics may need design input (DR-P18).

## 8. Required approvals
Each code fix (small diffs, one per task); any scene/prefab edit in the Editor; no package changes expected.

## 9. Validation procedure
Rebuild Android development build -> install via ADB -> play the slice -> capture `adb logcat` (filter Unity/crash) -> compare to baseline. Re-run after each fix. Compile check from the Editor console (state explicitly if not performed).

## 10. Acceptance criteria
- Touch-controlled human tank moves, turns, charges and fires on device.
- AI tank seeks, shoots and can be destroyed; human can be destroyed; rounds reset correctly.
- A first-to-5 match completes to the end state in at least one arena.
- Power-up pickups still work in the round after a death mid-effect (speed/cooldown back to base; pickup allowed).
- `logcat` of the full match contains no unexplained exceptions (each is fixed or filed).

## 11. Evidence to record
Date, commit hash, build type, device model/Android version, `adb` command, relevant `logcat` excerpt/location, PASS/FAIL per checklist row, screenshots/video if available. Entries in `DEVELOPMENT_LOG.md`.

## 12. Exit gate
Acceptance met with device evidence and owner review; otherwise milestone = REVIEW REQUIRED. Next: M2.

## Completion summary
_(empty)_
