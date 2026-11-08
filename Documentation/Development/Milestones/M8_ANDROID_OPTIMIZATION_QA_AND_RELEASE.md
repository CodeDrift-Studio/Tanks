# M8 — Android Optimization, QA and Release

**Status: NOT STARTED** · Effort: 27–50 days (impl 15–28, test 12–22) + external waiting · Tags: `[Device]` `[Build]` `[Approval]` `[Console]` `[Legal]`
References: `RELEASE_GAP_ASSESSMENT.md` §6.4, §8, §13, PERF-01, OPT-01, QA-01, QA-02, REL-01, STORE-01; ADR-009.

## 1. Objective
A stable, performant, policy-compliant signed release that has passed Play internal (and applicable closed) testing and received final owner approval.

## 2. Scope
**Arena scope (DR-C14, 2026-11-08): the first release ships one arena, Desert.** Performance measurement, lightmap/texture optimization, the device-matrix regression and store screenshots cover Desert only; Jungle and Moon are not part of the release build or its QA. Verify at release-build time that the build scene list contains only the intended scenes.
- Android performance budgets (DR-P8, DR-P10 -> ADR-009) and measurement on a device matrix.
- Optimization: textures, audio (e.g. large WAV music), lightmaps, models, quality tiers, frame cap, material churn, AI path allocations; pooling only if profiling shows GC.
- Memory, FPS and thermal testing (sustained sessions).
- EditMode / PlayMode automated tests; physical-device regression of all checklists, including service categories.
- Signed release AAB; IL2CPP and ARM64 verification; target API per current Play requirements (verify at time of release); versioning.
- Privacy policy; Play Data Safety; content rating; ads declaration; target audience declaration (DR-P13).
- Play Console internal testing and applicable closed testing; release monitoring plan (Crashlytics/Play vitals).
- Final release approval; `dev` -> `main` PR per `GIT_WORKFLOW.md`.

## 3. Out of scope
New features; additional platforms; post-release live operations (P3); legal advice.

## 4. Prerequisites
M4–M7 complete (content and features frozen except fixes). DR-P8, DR-P10, DR-P14, DR-P16. Play developer account and signing plan (Play App Signing). Device matrix available.

## 5. Tasks
- [ ] `[Approval]` Record budgets (ADR-009): min Android version, device class, target FPS, memory, size, load time.
- [ ] `[Device]` Baseline profiling on the matrix (Unity Profiler on device, memory snapshots, thermal 10+ min sessions).
- [ ] Investigate KI-A07: use the Editor build report of the Desert-only build to determine whether Jungle/Moon-only assets are included; remove stray references only with approval. Record findings.
- [ ] Optimize to budget (asset import settings, audio compression, lightmap compression, GC hotspots, quality tier); re-measure after each change.
- [ ] Tests: EditMode/PlayMode suites (power-up state, scoring, entitlements logic, data migration) with executed results; smoke test.
- [ ] `[Device]` Full regression of `TESTING_CHECKLIST.md` (gameplay, flow, UI, Firebase, GPGS, AdMob, IAP, lifecycle, performance) on the matrix.
- [ ] `[Approval]` Release build configuration: IL2CPP, ARM64, stripping, version/versionCode, release icon/splash; verify no editor-only code or test IDs in release.
- [ ] `[Console]` Keystore/Play App Signing; upload signed AAB to internal testing; verify install/update from Play.
- [ ] Privacy policy, Data Safety form, content rating questionnaire, ads declaration (from `SERVICE_INTEGRATION_PLAN.md` data lists).
- [ ] Closed testing where required by current Play policy (verify); collect and triage feedback; pre-launch report review.
- [ ] Release monitoring plan; final known-issues review.
- [ ] Final approval record and `dev` -> `main` PR (owner approval).

## 6. Dependencies
M7 clearance; M5/M6 services stable; owner accounts; device availability; external Play review/testing durations.

## 7. Risks
Low-end device performance; thermal throttling; Play policy changes (target API, testing requirements); late compliance findings; real ad/product IDs introduced at release; release-only bugs (stripping/IL2CPP).

## 8. Required approvals
Budgets; release settings; privacy/declaration content; promotion tracks; final release; PR to `main`.

## 9. Validation procedure
Measured results only (device, build, tool, date). Release candidate installed from Play internal track, not sideloaded, for final checks. Compile/test runs executed and recorded; anything not executed is listed as not executed.

## 10. Acceptance criteria
- Budgets met on the matrix with recorded measurements.
- Full regression PASS (or tracked, owner-accepted issues); no open P0/P1.
- Signed AAB installs and runs from Play internal testing; IL2CPP + ARM64 confirmed in the build.
- Privacy policy, Data Safety, content rating, ads declaration complete and consistent with actual SDK behavior.
- Owner final approval recorded.

## 11. Evidence to record
Profiling captures/summaries, device matrix table, test run outputs, AAB version info, Play Console status screenshots (no secrets), declaration copies, approval record.

## 12. Exit gate
All acceptance criteria met with evidence and owner approval; otherwise REVIEW REQUIRED.

## Completion summary
_(empty)_
