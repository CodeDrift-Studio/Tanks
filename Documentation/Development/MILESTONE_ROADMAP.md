# MILESTONE ROADMAP

Created 2026-11-06. All milestones **NOT STARTED** (no repository evidence of any milestone work). Live status: `MILESTONE_STATUS.md`. Task detail: `Milestones/`. Complexity = relative effort estimate (S/M/L/XL), not a schedule. No completion percentages are given on purpose.

Scope legend: **MVP** = required for the approved MVP (see CLAUDE.md §B). **OPTIONAL** = desirable, only with owner approval.

```text
M0 Stabilize → M1 Validate gameplay → M2 Platform input → M3 Foundation → M4 Polish → M5 Content → M6 Optimize/QA → M7 Release
(M1 and M2 may overlap only if the owner approves; M3 must not start before M0–M2 acceptance.)
```

---

## M0 — Project Stabilization  (complexity: S–M)
- **Objective**: a safe, buildable baseline under version control.
- **Scope (MVP)**: git init + Unity `.gitignore`; Force Text serialization; Visible Meta Files; correct Build Settings; scene validation (open each scene, check missing refs/console); Windows dev build; Android dev build; (decide) persisted project-wide Input Actions.
- **Tasks**: see `Milestones/M0_PROJECT_STABILIZATION.md`.
- **Dependencies**: owner approval for git init and for Force-Text re-serialization (rewrites every scene/prefab).
- **Risks**: bulk re-serialization diff; Android SDK/JDK/NDK setup; binary assets today (KI-003); stale build list (KI-001).
- **Deliverables**: repo + first baseline commit (owner-approved), valid Build Settings, working Windows/Android dev builds, baseline notes.
- **Acceptance**: see milestone file.
- **Definition of Done**: all acceptance criteria met with evidence; docs updated.
- **Blockers**: owner approvals; Android toolchain availability (UNKNOWN).
- **Exit**: baseline commit exists; both dev builds launch to the menu.

## M1 — Gameplay Validation  (M)
- **Objective**: establish what actually works at runtime.
- **Scope (MVP)**: movement, shooting, health/damage, AI, six power-ups, camera, round transitions, results, restart, three arenas.
- **Dependencies**: M0 (valid build list, repo).
- **Risks**: latent defects (KI-P01 AI nulls, KI-P02 power-up state leak, KI-P03 input null) surface; runtime verification needs a human with the Editor.
- **Deliverables**: filled `TESTING_CHECKLIST.md` (Gameplay/AI/Power-ups/Multiplayer), triaged `KNOWN_ISSUES.md`.
- **Acceptance**: every checklist row PASS or has a filed issue; no P0/P1 gameplay blocker open.
- **Exit**: gameplay baseline documented; fixes for confirmed defects done or scheduled.

## M2 — Platform Input and Compatibility  (M–L)
- **Objective**: reliable input and UI on Windows and Android.
- **Scope (MVP)**: PC keyboard (2 players), PC gamepad (currently appears unwired, KI-010), Android touch controls (not in scenes, KI-008), UI scaling, resolutions/aspect ratios.
- **Dependencies**: M0; M1 findings.
- **Risks**: input-user/device pairing on Android (`Keyboard.current` null), project-wide actions null in player (KI-006), no safe-area handling.
- **Deliverables**: working input matrix; platform checklists executed.
- **Acceptance**: milestone file. **Exit**: each supported control scheme verified on target hardware.

## M3 — Game Foundation  (L)
- **Objective**: turn the tutorial into an owned game structure.
- **Scope (MVP)**: original identity (name TBD), boot + menu flow, match configuration, arena selection, match settings, game config data, persistent settings, folder migration out of `Tutorial_Demo`.
- **Dependencies**: M0–M2 accepted; approvals for migration (ADR-004), namespace (ADR-005), data config approach (ADR-006), final name (PENDING).
- **Risks**: GUID breakage during migration; scope creep; hard-coded 4-slot menu.
- **Acceptance/Exit**: milestone file.

## M4 — Gameplay and UX Polish  (M–L)
- **Objective**: HUD, results, settings, pause, feedback.
- **Scope (MVP)**: improved HUD, round/match results screens, settings menu, pause flow, controls feedback, gameplay feedback. **OPTIONAL**: AI improvements (only where M1 shows a justified need).
- **Dependencies**: M3.

## M5 — Original Content and Differentiation  (L–XL)
- **Objective**: original, legally clean content that differentiates from the sample.
- **Scope**: **MVP-minimum**: replace/transform enough sample-derived content to satisfy the licensing review outcome (decision depends on KI-007). **OPTIONAL**: new tank variations, new mechanics, weapon/power-up variations, new arena content (extra arenas are out of MVP).
- **Dependencies**: M3; licensing findings; owner decisions on scope.
- **Risks**: scope expansion; art pipeline cost.

## M6 — Optimization and Quality Assurance  (L)
- **Objective**: meet performance/stability targets on real hardware.
- **Scope (MVP)**: Android + Windows profiling, memory, GC allocations, regression tests (EditMode/PlayMode), device compatibility, input validation, stability.
- **Dependencies**: M4 (and M5 if content added). Budgets are PENDING DECISION (ADR-009).

## M7 — Commercial Release Preparation  (L)
- **Objective**: a shippable release candidate.
- **Scope (MVP)**: asset licensing review (blocker if unresolved), branding, store assets, versioning, build configs (package id, keystore, ARM64), privacy review (Unity Analytics module / AI packages), release candidate, distribution prep.
- **Dependencies**: M6; owner decisions on name, pricing, business model, stores.
- **Blockers**: KI-007 licensing; business model and name undecided.

---

## Scope Summary
| Item | MVP | Optional | Out of MVP |
|---|---|---|---|
| Existing 6 power-ups, 3 arenas, 2–4 tanks, AI | ✔ | | |
| Windows + Android builds | ✔ | | |
| Gamepad (PC) / touch (Android) | ✔ | | |
| New tanks, mechanics, weapons | | ✔ | |
| AI improvements | | ✔ (justified only) | |
| More than 3 arenas, online, IAP, accounts, battle pass, customization | | | ✔ |
| Android shared-screen multiplayer | | PENDING DECISION | |
