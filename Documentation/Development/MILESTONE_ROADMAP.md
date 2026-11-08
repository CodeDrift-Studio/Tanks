# MILESTONE ROADMAP — Android-First Release (M0–M8)

Revised 2026-11-08 from owner decisions (`DECISION_REGISTER.md`). **Supersedes** the original 2026-11-06 Windows+Android roadmap (kept unchanged for history in `Archive/2026-11-06_PC-oriented-roadmap/`). Live status: `MILESTONE_STATUS.md`. Task detail: `Milestones/`. Assessment reference (historical, static evidence only): `RELEASE_GAP_ASSESSMENT.md` (its task IDs such as AND-01, AI-01, GP-01, FLOW-01, UI-01 are reused below). Services: `SERVICE_INTEGRATION_PLAN.md`.

**Status note (2026-11-08):** M0 is COMPLETE and M1 is IN PROGRESS (owner-reported; `MILESTONE_STATUS.md`), and a development build has run on a OnePlus 12. Statements below that the game was never built or run are historical (written 2026-11-08 before those events). Effort numbers are developer working days (solo developer), order-of-magnitude, **not a schedule and not a release date**.

## Scope (confirmed 2026-11-08)
IN: Android only; single-player human vs AI; touch controls; **one arena: Desert** (owner-confirmed 2026-11-08, DR-C14); Free-to-Play with Ads + IAP; AdMob, Unity IAP / Google Play Billing, Firebase, GPGS (feature scope per service PENDING).
OUT of first release (code and assets kept, nothing deleted): Windows/PC release, iOS, PC keyboard release controls, PC gamepad, online multiplayer, shared-screen Android multiplayer, **Jungle and Moon arenas (deferred to a future content update; code, scenes and assets kept, excluded from the release build)**, arena-selection UI, additional arenas beyond Desert (unless approved later), battle pass / live operations, any unapproved progression or monetization feature.

## Flow
```text
M0 Stabilize -> M1 Android Vertical Slice -> M2 Reliability & Balance -> M3 Foundation & Flow -> M4 Android UI/UX
   |  licensing investigation starts early (KI-007) and runs alongside M0-M4
   v
M5 Firebase / GPGS / Player Data -> M6 AdMob + IAP -> M7 Content / Branding (final clearance gate) -> M8 Optimization, QA, Release
```
M5/M6 depend on the final **package ID** (DR-P2) and on audience/age (DR-P13). M7 art work depends on the licensing outcome (DR-P11). Overlap between milestones needs owner approval; one milestone at a time remains the default rule. **Approved 2026-11-08 (DR-C18): M5 may overlap M2–M4**, which stay in the roadmap and are not automatic blockers for M5.

## Current position and next focus (2026-11-08)
- **Gameplay baseline frozen (temporary, DR-C16).** Core gameplay stable; round-reset fixes (GP-01…GP-04), GP-08 and GP-10 guards done and owner-verified; all six power-ups verified; the initial four-tank balance pass is the accepted baseline (DR-C15). Statistically balanced win rates are not yet established — that validation stays an M2 task and is not started while the freeze is in force.
- **Android multi-device testing: IN PROGRESS / RESULTS PENDING.** Not a gate that has passed. The freeze lifts only when the report arrives and the owner says so.
- **Freeze exemption (DR-C19):** AND-01 Android input guards (M1) are exempt from the gameplay freeze; unrelated gameplay changes are not.
- **Next development focus: Firebase + GPGS (M5)** may proceed in parallel with that testing (DR-C17). It is **not started**; the exact feature scope is still PENDING (DR-P5/P6/P7) and the start is gated by the package ID (DR-P2), audience/age (DR-P13), account ownership (DR-P15) and package-install approval. AdMob and Unity IAP remain in M6, after M5's consent foundation.
- **Sequencing (resolved, DR-C18):** M5 may overlap M2–M4. M2–M4 (reliability, game flow, UI/UX) remain in the roadmap, NOT STARTED, and are not automatic blockers for M5. M5's own gates still apply; M6 still follows M5's consent foundation.

## Milestones

### M0 — Project Stabilization  (COMPLETE, owner-reported 2026-11-08)
- **Objective**: safe, buildable, version-controlled Android-capable baseline.
- **Scope**: Force Text + Visible Meta Files; git + fresh-clone validation; Build Profiles (first-release build contains **Desert only, index 0**; Jungle/Moon may stay in the project, not in the release build); scene/prefab verification; Android SDK/NDK/JDK verification; first Android development build; physical-device install and startup. Record Android Player Settings. Start licensing evidence collection (KI-007).
- **Dependencies**: owner actions (backup, Editor settings), an Android device.
- **Effort**: 3–7 days + owner/device availability.
- **Exit gate**: M0 acceptance in `Milestones/M0_PROJECT_STABILIZATION.md` met with evidence; Android dev build installs and reaches the menu on a physical device.

### M1 — Android Gameplay Vertical Slice  (IN PROGRESS; opened 2026-11-08)
- **Objective**: one complete human-vs-AI match playable by touch on a physical device, with logs.
- **Scope**: Android input initialization (AND-01), virtual joystick + touch shooting, human vs one AI, shooting/damage/destruction, AI targeting/navigation, power-up lifecycle fixes (GP-01), one complete match, `logcat` inspection.
- **Dependencies**: M0 exit gate.
- **Effort**: 9–18 days.
- **Exit gate**: a full match (first-to-5) completes on device in at least one arena, touch-controlled; `logcat` reviewed, no unexplained exceptions.

### M2 — Gameplay Reliability and Balancing  (NOT STARTED)
- **Objective**: remove known reliability risks; fair play in the Desert arena (first-release arena).
- **Scope**: AI null/target-slot risks (AI-01), tank lifecycle/reset, all six power-ups, round/match scoring, AI difficulty, balance (initial four-tank pass accepted as baseline 2026-11-08, DR-C15; win-rate validation still open and frozen under DR-C16), Desert arena validation (Jungle/Moon deferred), scene/match-state defects.
- **Dependencies**: M1; DR-P12, DR-P18.
- **Effort**: 14–25 days.
- **Exit gate**: soak test (100+ bot rounds in Desert) clean; gameplay checklist rows PASS or tracked; no open P0/P1 gameplay defect.

### M3 — Product Foundation and Game Flow  (NOT STARTED)
- **Objective**: replace the tutorial scene-reload flow with a real game flow.
- **Scope**: boot/startup architecture, main menu, match setup, match initialization (arena passed as data, Desert is the only first-release entry; no arena-selection UI), round and match results, rematch/menu navigation, settings persistence, Android lifecycle and back-button handling (FLOW-01, AND-02).
- **Dependencies**: M2; ADR-004/005/006/010/011; DR-P10, DR-P12.
- **Effort**: 15–28 days.
- **Exit gate**: cold start -> menu -> setup -> Desert match -> results -> rematch/menu works on device; back/background/resume safe; settings survive restart.

### M4 — Android UI/UX and Player Experience  (NOT STARTED)
- **Objective**: touch-first presentation suitable for a commercial game.
- **Scope**: touch-first UI, main menu/setup, gameplay HUD, pause/settings, results screens, safe area, orientation, touch-target sizing and control tuning, first-time instructions, audio/UI feedback.
- **Dependencies**: M3; DR-P9, DR-P20; licensing outcome (DR-P11) before heavy art investment.
- **Effort**: 22–40 days.
- **Exit gate**: playtest sign-off on the device matrix; no clipped UI on notched/tall/small screens; touch targets meet the agreed size.

### M5 — Firebase, GPGS and Player Data  (NOT STARTED)
- **Objective**: crash/analytics visibility, Play Games features, robust player data.
- **Scope**: Firebase project/app registration, Analytics, Crashlytics, Remote Config (assess; integrate where approved), consent/privacy, non-blocking init; GPGS sign-in, auth lifecycle, achievements/leaderboards per approved scope, Saved Games only if approved; local settings/progress, schema/versioning, local/cloud ownership, migration/recovery. See `SERVICE_INTEGRATION_PLAN.md`.
- **Dependencies**: M2–M4 are **not** automatic blockers (DR-C18, owner 2026-11-08: M5 may overlap them; DR-C17: may proceed while multi-device Android testing continues); **DR-P2 package ID, DR-P5, DR-P6, DR-P7, DR-P13, DR-P15, DR-P16**; package-install approval.
- **Effort**: 19–37 days (+ external console/propagation waiting).
- **Exit gate**: services pass device tests; game fully playable with every service failing/offline; manifest/permission/size diff reviewed.

### M6 — Monetization: AdMob + IAP  (NOT STARTED)
- **Objective**: policy-compliant ads and purchases that never block gameplay.
- **Scope**: AdMob account/app/ad units, format assessment, approved placements, cooldowns/frequency caps, consent, test ads, failure handling; IAP catalog, purchase lifecycle (success/cancel/pending/failure), entitlement grants, duplicate prevention, restore/re-query, refund/revocation handling.
- **Dependencies**: M5 consent foundation; DR-P2, DR-P3, DR-P4, DR-P13, DR-P15; a signed build on a Play testing track for Billing.
- **Effort**: 19–35 days (+ AdMob/Play review waiting).
- **Exit gate**: test ads and licence-tester purchases verified on device across success/failure/offline; policy self-review complete.

### M7 — Original Content, Branding and Commercial Identity  (NOT STARTED; licensing investigation starts early)
- **Objective**: legally cleared, original, branded content; final clearance gate.
- **Scope**: sample-asset licensing review (**starts in M0/M1**, tracked as KI-007), third-party asset/font review, replacement of uncleared assets, final name, visual identity, logo/icon/splash, menu and arena presentation, audio/music, store screenshots/feature graphic, store copy.
- **Dependencies**: owner/legal review; DR-P1, DR-P11, DR-P17; M4 UI to brand; M5/M6 for store-listing accuracy.
- **Effort**: 16–78 days, **dominated by licensing replacement risk (0–40 days, unknown until reviewed)**; branding/store art 13–38; early investigation 2–4.
- **Exit gate**: every shipped asset has recorded provenance and an owner-accepted licence status; no unresolved licensing item; final name/identity applied.

### M8 — Android Optimization, QA and Release  (NOT STARTED)
- **Objective**: stable, performant, compliant signed release through Play testing tracks.
- **Scope**: performance budgets, device matrix, texture/audio/lightmap optimization, memory/FPS/thermal tests, EditMode/PlayMode tests, physical regression, signed release AAB, IL2CPP + ARM64 verification, privacy policy, Data Safety, content rating, ads declarations, Play Console internal testing and applicable closed testing, final release approval.
- **Dependencies**: M4–M7 complete; DR-P8, DR-P10, DR-P14, DR-P16.
- **Effort**: 27–50 days (+ external waiting: Play review, closed-test duration requirements — verify current Play policy).
- **Exit gate**: release checklist PASS with evidence; owner approval; `dev` -> `main` PR approved per `GIT_WORKFLOW.md`.

## Effort summary (developer working days)
| Milestone | Implementation | Testing | Total | Main uncertainty |
|---|---|---|---|---|
| M0 | 1–3 | 2–4 | 3–7 | Owner actions, SDK paths, first-build issues |
| M1 | 6–12 | 3–6 | 9–18 | Android input unknowns (never run on a device) |
| M2 | 8–15 | 6–10 | 14–25 | What soak tests reveal; balance iteration |
| M3 | 12–22 | 3–6 | 15–28 | Architecture decisions (ADR-004/011) |
| M4 | 18–32 | 4–8 | 22–40 | Art/UX scope; device diversity |
| M5 | 12–23 | 7–14 | 19–37 | SDK/Gradle conflicts; consent/age decisions; cloud save (+3–6) |
| M6 | 13–24 | 6–11 | 19–35 | Ad policy review; Billing needs a Play-distributed build |
| M7 | 11–66 | 5–12 | 16–78 | **Licensing outcome / asset replacement** |
| M8 | 15–28 | 12–22 | 27–50 | Low-end device performance; compliance paperwork |
| **Total** | **~96–225** | **~48–93** | **~144–318** | Excluding the M7 replacement worst case: ~144–280 |

Not included: owner decision time, Play/AdMob review, closed-testing minimum durations, legal review turnaround, outsourced art lead times, device procurement. The earlier 95–165-day figure (assessment §20) covered no Firebase, GPGS, AdMob or IAP work and is superseded by this table; it stays in `RELEASE_GAP_ASSESSMENT.md` as a historical estimate.

## Optional / deferred (not scheduled)
**Jungle and Moon arenas (future content update, DR-C14)**, arena-selection UI, additional arenas, shared-screen multiplayer, PC/iOS ports, PC gamepad (KI-010), extra tanks/mechanics, localization beyond launch languages, battle pass/live ops. They enter the roadmap only by written owner approval.
