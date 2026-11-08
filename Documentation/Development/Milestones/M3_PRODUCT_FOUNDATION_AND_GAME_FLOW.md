# M3 — Product Foundation and Game Flow

**Status: NOT STARTED** · Effort: 15–28 days (impl 12–22, test 3–6) · Tags: `[Editor]` `[Device]` `[Approval]`
References: `RELEASE_GAP_ASSESSMENT.md` §5.6, §7, FLOW-01, AND-02, UI-02, UI-04; ADR-004, 005, 006, 010, 011; KI-P06, KI-011.

## 1. Objective
Replace the tutorial flow (single scene, `LoadScene(0)` restart, text-only results) with an owned, Android-aware game flow.

## 2. Scope
- Boot/startup architecture (e.g. Boot -> MainMenu -> Arena scenes; ADR-011).
- Main menu; match setup (one human vs N bots; difficulty). **No arena-selection screen in the first release** (Desert only, DR-C14): the match config carries an arena identifier and the flow loads scenes by explicit name/constant, so adding Jungle/Moon later does not require re-architecture. Do not build speculative multi-arena UI.
- Match initialization from a match-config object (not menu-slot coupling).
- Round and match results flow (data/state; the polished screens are M4); rematch / back-to-menu navigation; no dependence on build index 0.
- Settings persistence layer (ADR-010) — storage service only; the settings UI is M4.
- Android lifecycle (pause on background / interruption, safe `timeScale` restore) and Back-button handling.
- Decide and, if approved, execute folder migration (ADR-004) and namespace/asmdef (ADR-005) — only if justified.
- Cleanup of template leftovers (KI-011) only with approval.

## 3. Out of scope
Final UI art/layout (M4); service SDKs (M5/M6); multiplayer; additional arenas (Jungle/Moon deferred) and arena-selection UI; localization beyond a text-table decision.

## 4. Prerequisites
M2 exit gate; decisions DR-P10 (budgets), DR-P12 (resolved: Desert only); ADR approvals above.

## 5. Tasks
- [ ] `[Approval]` ADRs decided and recorded: scene flow (ADR-011), settings storage (ADR-010), data config (ADR-006), migration (ADR-004), namespace/asmdef (ADR-005).
- [ ] `[Editor][Approval]` Build Profiles final scene list for the new flow (Boot, MainMenu, Desert; Jungle/Moon not included); verify no `LoadScene(0)`/build-index assumptions remain (`GameManager`, `PauseMenu`).
- [ ] Implement Boot/Menu/Arena loading by explicit scene name/constants; async load with a minimal loading state.
- [ ] Match configuration object passed menu -> match (arena id [Desert only in release], bot count, difficulty).
- [ ] Round/match result state exposed to UI (winner, scores, rounds); rematch/menu actions (no next-arena action in the first release).
- [ ] Settings service: versioned local storage, defaults, atomic write, corruption recovery (shared with M5 player data).
- [ ] Android lifecycle: `OnApplicationPause/Focus` handling, safe resume, pause menu on back button, quit confirmation policy.
- [ ] `[Approval]` Folder migration in the Editor in small commits (GUIDs preserved) if approved.
- [ ] `[Device]` Flow test on device: cold start, background/resume mid-match, back at each screen, rotate (if allowed), kill/restart.

## 6. Dependencies
M2; ADRs; Editor for scenes; device.

## 7. Risks
Largest refactor risk in the project (GameManager/menu coupling to 4 fixed slots); migration GUID/diff risk; scope creep into UI (M4) or services (M5).

## 8. Required approvals
Architecture ADRs; folder migration; scene/prefab changes; deletion of leftovers; new asmdefs.

## 9. Validation procedure
Device flow test script (see TESTING_CHECKLIST flow rows); fresh-clone open; compile check; settings persistence across kill/relaunch; log review.

## 10. Acceptance criteria
- Cold start -> menu -> setup -> Desert match -> results -> rematch / menu works on device (Desert is the only first-release arena).
- No build-index-0 dependency; Build Profiles valid.
- Back button and app backgrounding never leave the game paused-stuck, crashed or in a broken state.
- Settings persist across restarts; corrupted data falls back safely.
- Match config drives the match (no hidden menu coupling).

## 11. Evidence to record
ADR entries, scene-flow diagram, device test results (model/Android/commit), log excerpts, diff summaries for refactors, fresh-clone result.

## 12. Exit gate
Acceptance met with device evidence; owner approves the architecture. Next: M4.

## Completion summary
_(empty)_
