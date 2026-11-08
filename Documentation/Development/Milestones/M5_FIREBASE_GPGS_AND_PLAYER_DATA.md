# M5 — Firebase, GPGS and Player Data

**Status: NOT STARTED** (owner-directed next development focus 2026-11-08, DR-C17: may proceed while multi-device Android testing continues; feature scope still PENDING; M2–M4 are not automatic blockers, DR-C18; start gated by the dependencies below) · Effort: 19–37 days (impl 12–23, test 7–14; +3–6 if cloud save) + external waiting · Tags: `[Editor]` `[Device]` `[Approval]` `[Console]` `[Legal]`
Detail per service: `SERVICE_INTEGRATION_PLAN.md` §0–§4, §7. References: `RELEASE_GAP_ASSESSMENT.md` §9, PKG-01, MON-01, STORE-01 (privacy part).

## 1. Objective
Add the mandatory Firebase and Google Play Games Services integrations and a robust player-data layer without ever blocking gameplay.

## 2. Scope
**Firebase**: project and Android app registration; Analytics (event list approved first); Crashlytics (incl. IL2CPP symbol upload plan); Remote Config **assessed, integrated only if approved (DR-P5)**; consent/privacy flow foundation; non-blocking initialization; manifest/permission/size review.
**GPGS**: sign-in (automatic + manual); authentication lifecycle; achievements and leaderboards **per approved scope (DR-P6)**; Saved Games **only if cloud progression is approved (DR-P7)**; offline and authentication-failure handling.
**Player data**: local settings/progress; data schema and versioning; local vs cloud ownership boundaries; migration and corruption recovery.

## 3. Out of scope
AdMob and IAP (M6); backend servers; accounts other than Play Games; push notifications; attribution SDKs; features not approved in DR-P5/P6/P7.

## 4. Prerequisites
M4 exit gate is **not** an automatic blocker (DR-C18: M5 may overlap M2–M4). **Final package ID (DR-P2)** and signing plan (DR-P16) decided. Account ownership (DR-P15). Audience/age decision (DR-P13). Owner approval to install packages (Firebase SDK, EDM4U, GPGS plugin). Owner creates/controls console projects (Claude does not configure accounts).

## 5. Tasks
- [ ] `[Approval]` Verify current official SDK docs and compatibility (Unity 6000.3.6f1, Gradle, minSdk); record versions chosen (no install yet).
- [ ] `[Console][Approval]` Owner: Firebase project, Android app registration with final package ID + SHA fingerprints, `google-services.json` handling decision.
- [ ] `[Approval]` Install Firebase packages; resolve Android dependencies; first device build check (manifest + size diff recorded).
- [ ] Firebase façade + null implementation; async init with timeout; failure never blocks.
- [ ] Crashlytics: init, custom keys, non-fatal logging, symbol upload; forced test crash on a development build.
- [ ] Analytics: approved event list + parameter rules (no PII); consent gating.
- [ ] Remote Config (only if approved): defaults, fetch/activate at safe points, validation/clamping, kill switches.
- [ ] Consent/privacy flow foundation (shared with M6 UMP); settings toggle for analytics where required.
- [ ] `[Console][Approval]` Owner: Play Console GPGS setup (credentials, achievements, leaderboards, testers).
- [ ] GPGS integration: sign-in, sign-out/account change, achievements and leaderboards (approved list), offline queue policy, guest play.
- [ ] Player data layer: schema v1, migration function, atomic write, backup/recovery; ownership table; Saved Games only if DR-P7 = yes (conflict resolution rules).
- [ ] `[Device]` Test matrix: first run, offline, airplane toggling, Play services missing/outdated, account switch, consent denied.
- [ ] Update privacy artifacts inputs (data collected list) for M8.

## 6. Dependencies
M2–M4 (not automatic blockers, DR-C18); DR-P2, P5, P6, P7, P13, P15, P16; external console propagation time; Firebase/GPGS SDK compatibility with Unity 6.3 (**UNVERIFIED**).

## 7. Risks
Gradle/EDM4U dependency conflicts (also with AdMob/IAP in M6); changing package ID after registration; child-audience restrictions (DR-P13); symbolication failures for IL2CPP; GPGS debug vs release signing mismatches; unexpected added permissions.

## 8. Required approvals
Each package install (package change = approval, CLAUDE.md §K); project-wide Android settings changes; which config files are committed; Remote Config inclusion; cloud-save inclusion.

## 9. Validation procedure
Development build on a physical device with USB debugging; Firebase DebugView; Crashlytics console; GPGS tester accounts; `logcat`; offline/online toggling; compare merged manifest before/after.

## 10. Acceptance criteria
- Game starts and plays normally with each service failing, offline, or consent denied.
- Crashlytics shows a symbolicated test crash and non-fatal; Analytics events validate in DebugView; no PII in events.
- GPGS sign-in is optional; each approved achievement unlocks once; leaderboards submit/read; failure states handled.
- Player data survives restart, schema migration test, and a deliberately corrupted file (falls back safely).
- Added permissions and size are reviewed and recorded.

## 11. Evidence to record
SDK versions and install commit, console screenshots (no secrets), DebugView/Crashlytics screenshots, device test table (model/Android/commit/result), manifest diff, log excerpts, schema doc.

## 12. Exit gate
All acceptance criteria met with device evidence; owner approves data/consent behavior. Next: M6.

## Completion summary
_(empty)_
