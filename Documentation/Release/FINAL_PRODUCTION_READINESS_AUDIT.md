# Tank Mayhem v1.0 — Final Production Readiness Audit

Date: 2026-11-12 · Branch: `feature/android-release-roadmap` (uncommitted working tree, not switched, nothing staged/committed/pushed)
Method: STATIC code/config read, plus read-only Unity Editor queries (console, Player Settings, active build profile). **No build, no Play Mode run, no device test, no Firebase backend check was performed.**

## 1. Executive Summary

- **Overall status: READY WITH CONDITIONS**
- Findings: **P0 = 0 · P1 = 2 (both owner actions, no code fix) · P2 = 11 · P3 = 3**
- **Fixes applied: none.** No confirmed P0/P1 code defect was found, so no source, scene, prefab or settings file was changed. The only file written is this report.
- The baseline code paths reviewed (round/match flow, pause, mobile controls, Firebase) show no confirmed crash or stuck-input defect by static reading. This is **not** runtime verification.

## 2. Audit Findings

| ID | Pri | Category | Component | Issue / Evidence | Action |
|---|---|---|---|---|---|
| F-01 | P1 | Android build | Version values | Three different values: global `ProjectSettings.asset` 1.0.1 / code 1; Dev profile 1.0.7 / 7; **Release profile (active) 1.0.4 / 4**. Editor confirms active profile is Release with 1.0.4 / 4. The highest code ever uploaded to Play is unknown to Claude. | **Owner action**: pick the code from the Play Console (must exceed every uploaded code, including any Dev uploads). Not changed. |
| F-02 | P1 | Build verification | AAB | No production AAB has been built or inspected. INTERNET permission, ABI list, targetSdk and symbols are unverified in a real artifact. `forceInternetPermission` is off (relies on Firebase AAR manifest merge). | **Owner action**: build, then `bundletool dump manifest` per `MANUAL_ACTIONS_REQUIRED.md`. |
| F-03 | P2 | Android build | Architectures | Editor reports `ARM64, X86` (flag 6) for the Release profile. X86 is not needed for a phone game. Resolver already excludes armeabi-v7a and x86_64. | Propose ARM64-only. **Needs owner approval** (project/profile setting). Not changed. |
| F-04 | P2 | Android build | Orientation | Active Release profile: AutoRotation with only Landscape L/R allowed (OK). Global settings still allow portrait; a build made without the profile would. | Make sure builds use Release-Android. |
| F-05 | P2 | Debug artifacts | In-game debug console | Working-tree `Demo_Game_Desert.unity` has **0** references to the console (HEAD had 71): it was removed from the scene. Release build therefore ships no console UI. `ReleaseBuildDebugStripper` is now a redundant safety net, and **was never shown to run** (no build log). The console plugin and its Android `.aar` are still packaged but not instantiated. Dev builds also have no console now. | Document only. Do not rely on the stripper as proof. |
| F-06 | P2 | Docs | Release docs | Stale statements (see §3 and §6 table): "no release profile exists", "TV flag on", "console is in the Desert scene", "Dev 1.0.6/6", "arch flag 6 unknown". | **Corrected 2026-11-12** (owner-approved, documentation only; see §3). Remaining unverified statements are marked in the docs. |
| F-07 | P2 | Firebase | Remote Config | `match_rounds_to_win` and `powerup_respawn_seconds` are **not read by any gameplay code** (grep: no consumers outside the Firebase folder). Fetch/activate runs but has no effect. | Report only. Not operational for gameplay. |
| F-08 | P2 | Privacy | Analytics / Crashlytics | Collection defaults to ON, persisted in PlayerPrefs, no consent UI (`k_DefaultCollectionEnabled = true`). Audience/age (DR-P13) undecided. | **Owner/legal decision.** Not changed. |
| F-09 | P2 | Firebase | Unused libraries | Firestore, App Check (+debug provider, Play Integrity) AARs are linked in the release. Nothing calls App Check; Firestore is only touched by `FirebasePlayerDataHandler.Activate` (acquires the instance, no read/write). | Report. Removal is a dependency change → needs approval. |
| F-10 | P2 | Packaging | `StreamingAssets/google-services-desktop.json` | Generated desktop config ships inside the Android build. Not needed on Android. Firebase client keys are not secrets but the file is unnecessary. | Report; owner to decide. |
| F-11 | P2 | Lifecycle | Pause on background / Back key | No auto-pause on `OnApplicationPause`; input is released on focus loss (`MobileUIControl.ReleaseAll`) but timeScale is unchanged. Back-button behaviour depends on the Pause action binding (not verified). | Known gap (KI). NOT RUNTIME VERIFIED. |
| F-12 | P2 | Resource | `GameUIHandler.StartGame` | Clones and enables a Pause `InputAction` per match start and never disables/disposes it. After scene reload the old clone stays enabled with a lambda on a destroyed handler. `PauseMenu.TogglePause` guards with a destroyed-object null check, so it is a no-op, not a crash. Small leak per match. | Not fixed (not P0/P1). |
| F-13 | P2 | Git hygiene | Keystore path | `ProjectSettings.asset` and the build profiles contain an absolute local keystore path and alias. No passwords found. | **Owner action** before any commit. Path deliberately not repeated here. |
| F-14 | P2 | Build | Crashlytics symbols | Release profile has debug symbol level/format set (`4`/`5`); what that maps to, and whether Crashlytics/Play receive symbols, is unverified. | Verify in the built AAB / console. |
| F-15 | P2 | Docs/Compliance | Play Console | Privacy policy URL, Data Safety form, content rating, target audience: not verifiable by Claude. | Owner actions §5. |
| F-16 | P3 | Firebase | Handler events | `FirebaseAuthenticationHandler` subscribes `StateChanged` and never unsubscribes. Lives for the app lifetime on a DontDestroyOnLoad host, so no practical leak. | Deferred. |
| F-17 | P3 | Repo | Large untracked trees | `Assets/Firebase` (256 MB), `GeneratedLocalRepo` (26 MB), iOS/tvOS plugin folders (17 MB each) are untracked. Android build should not include iOS/tvOS plugins. | Deferred; decide commit/ignore policy. |
| F-18 | P3 | Cleanup | Jungle/Moon scenes | Exist in project but are not in the build list (Desert only). Left in place per scope rules. | Deferred. |

### Checked and found OK (STATIC unless noted)
- Package ID `com.cds.tankmayhem` in global and Release profile; `google-services.json` has exactly one client, same package, project `tank-mayhem-ff62d`. Product name "Tank Mayhem".
- IL2CPP, min SDK 29, target SDK 36, Development off, script debugging off, profiler off, AAB on (Editor-confirmed for the active profile). Android TV compatibility **off** (Editor-confirmed).
- Build scene list: only `Demo_Game_Desert`. Restart / match end / "select tank" use `LoadScene(0)` / active build index, both resolve to Desert (the menu lives in that scene). `m_NumRoundsToWin = 3` in the scene.
- Firebase: single bootstrap with duplicate guard and "never initialize twice"; init is async with failure leaving the game playable; all Report* calls are null-safe no-ops; auth retry only while signed out; Info logs compiled out of release; no secrets in source.
- Firestore diagnostic: whole file under `#if UNITY_EDITOR || DEVELOPMENT_BUILD`; only caller is an Editor menu. Nothing calls `MergePlayerDocument` / `ReadPlayerDocument`. **No automatic Firestore writes in code.**
- Mobile controls (code): hidden until match start; pause hides via CanvasGroup (object stays active so the virtual gamepad survives); `ReleaseAll` on hide, pause, resume, focus loss and app pause; rotate buttons independent of the stick and of fire; both-pressed cancels.
- Nameplate: recreated (old one destroyed) so no duplicates; follows target, hides when tank inactive.
- Editor console: 0 errors, 0 warnings at query time, not compiling.

## 3. Changes Made

| File | Change | Why |
|---|---|---|
| `Documentation/Release/FINAL_PRODUCTION_READINESS_AUDIT.md` | **Added** | Required report. |
| `Documentation/Release/INTERNAL_TESTING_READINESS.md` | Edited (2026-11-12) | Release profile exists; version, ABI, TV, orientation, console, stripper and symbols statements corrected; keystore path removed. |
| `Documentation/Release/MANUAL_ACTIONS_REQUIRED.md` | Edited | Profile already exists; owner decisions stated; AAB check no longer relies on the stripper log; keystore path removed. |
| `Documentation/Release/PLAY_CONSOLE_CHECKLIST.md` | Edited | Version-specific wording removed; production-stage note added (UNVERIFIED); duplicate section number fixed. |
| `Documentation/Release/ANDROID_RELEASE_TEST_PLAN.md` | Edited | Added pause/resume and package-contents cases (NOT RUN); clarified console, background, Back and update cases. |
| `Documentation/Release/RELEASE_NOTES_v1.0.0.md` | Edited | Version placeholder note; rotate buttons mentioned. |

No source, scene, prefab, package, project-setting, build-profile or signing file was modified. No branch, stash, commit or push. No statuses in §4 were changed.

## 4. Verification Matrix

| Area | Status | Evidence |
|---|---|---|
| Android configuration | PARTIAL | Editor query of active Release profile: IL2CPP, no dev, AAB, TV off, landscape. ARM64+X86 and version mismatch open. |
| Scene configuration | PASS | `EditorBuildSettings` lists Desert only; no other `LoadScene` targets. Static. |
| Gameplay stability | PARTIAL | Static review only; no Play Mode, no device. |
| Mobile controls | PARTIAL | Code reviewed. **NOT RUNTIME VERIFIED.** |
| UI and player identification | PARTIAL | Static: YOU/AI naming, win text, single nameplate. Not seen on screen. |
| Firebase | PARTIAL | Code safe by reading; no backend/DebugView/Crashlytics evidence; Remote Config unused by gameplay. |
| Debug exclusion | PASS | Console absent from the working-tree scene (grep 0 vs 71 at HEAD); Firestore diagnostic is dev-only. Stripper not shown to run. Static. |
| Privacy readiness | OWNER ACTION REQUIRED | Default-on collection, no consent UI, policy URL/Data Safety/age group undecided. |
| Performance | NOT TESTED | No profiling or device data. |
| Compilation | PARTIAL | Editor compiled and ran a command script; console clean. No player (IL2CPP) build. |
| Android device testing | NOT TESTED | None. |
| Release documentation | PARTIAL | Stale statements corrected 2026-11-12 (F-06). Docs still describe unbuilt/unverified items as open; no device or Play Console evidence exists. |

## 5. Owner Actions Required

1. Choose the production `versionCode` from the Play Console history and set one consistent version for the Release profile (F-01).
2. Approve (or decline) ARM64-only architecture (F-03); set it in the Editor.
3. Build the signed AAB and inspect its manifest: INTERNET, targetSdk 36, ABIs, versionCode, package (F-02).
4. Confirm native symbols / Crashlytics mapping upload (F-14).
5. Keystore: enter passwords at build time only; before committing, blank or keep local the keystore path in `ProjectSettings.asset` and the build profiles (F-13).
6. Privacy: privacy policy URL, Data Safety form, content rating, target audience/age (DR-P13), decision on default-on analytics/crash collection (F-08, F-15).
7. Firebase Console: confirm Analytics DebugView events, Crashlytics app registration, published Remote Config values (must not publish 5 for rounds), Firestore rules if the diagnostic is ever used. Leave App Check enforcement off.
8. Run `ANDROID_RELEASE_TEST_PLAN.md` on the OnePlus 12 and at least one other device, with the Release build: especially background/resume, Back key, pause/resume controls, offline start.
9. Decide on the unused Firestore/App Check libraries and `google-services-desktop.json` (F-09, F-10).

## 6. Remaining Risks

- **Unverified at runtime**: everything about controls, pause, lifecycle, UI correctness, performance, and Firebase delivery.
- No auto-pause when backgrounded; a returning player may resume mid-fight (F-11).
- X86 library and unused SDKs enlarge the package (F-03, F-09).
- Doc drift could mislead the next release step (F-06).

### Documentation corrections (APPLIED 2026-11-12 after owner approval; list kept for traceability)
- `INTERNAL_TESTING_READINESS.md`: Release profile now exists (active, AAB, no dev); TV flag is off; arch is `ARM64, X86`; console is no longer in the Desert scene; Dev profile is 1.0.7/7, Release 1.0.4/4.
- `MANUAL_ACTIONS_REQUIRED.md` §A.1/A.2: profile already created; TV already off; orientation already landscape-only in the profile.
- `PLAY_CONSOLE_CHECKLIST.md`: release name/version `1.0.0 (1)` and "Update test 1.0.0 (2)" no longer match 1.0.4 (4).
- `RELEASE_NOTES_v1.0.0.md`: version text; "Back button… not final" stays accurate.
- `ANDROID_RELEASE_TEST_PLAN.md`: case 20 (console absent) should say the console was removed from the scene, not only stripped.

## 7. Production Release Recommendation

**Safe only after listed owner actions** (items 1, 3, 5, 6 at minimum, and a device pass). No code blocker was found, but nothing here is device-verified.

## 8. Next Steps

1. Decide versionCode/versionName; decide ARM64-only; apply in the Editor.
2. Approve the doc corrections in §6 so they can be applied.
3. Build the signed AAB with the Release-Android profile (passwords entered at build time).
4. Inspect the AAB manifest/ABIs/symbols; confirm no console UI on device.
5. Device-test per the plan (two devices); record evidence.
6. Complete Play Console declarations (policy URL, Data Safety, rating, audience).
7. Verify Firebase DebugView and Crashlytics from the release build.
8. Upload to the production track (or promote after a final Internal/Closed check).
