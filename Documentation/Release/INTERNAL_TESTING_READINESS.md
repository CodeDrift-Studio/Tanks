# Tank Mayhem v1.0.0 — Internal Testing Readiness

Audit date: 2026-11-09 · **Corrected 2026-11-12** against the current project state (read-only Editor query of the active build profile, plus static reads; see `FINAL_PRODUCTION_READINESS_AUDIT.md`). Branch: `feature/android-release-roadmap` · **No build was made. No device test was run by Claude. Nothing was uploaded.**

## Decision: **READY WITH CONDITIONS (owner actions below; nothing here is build- or device-verified)**
No code blocker was found by static review. The `Release-Android™` build profile now exists and is the active profile (Editor-verified 2026-11-12: Development off, Build App Bundle on). **No AAB has been built**, so the manifest, ABIs, symbols and the debug-console absence are still unverified in a real artifact. Building needs the Editor and the keystore passwords.

## 1. Configuration: actual vs required
Sources: `ProjectSettings/ProjectSettings.asset` (global), `Assets/Settings/Build Profiles/Dev-Android™.asset` and `Release-Android™.asset` (the active profile overrides the global values). All are uncommitted. "Editor-verified" = read via the Unity Editor on 2026-11-12; nothing was built.

| Setting | Actual (STATIC) | Required | Status |
|---|---|---|---|
| Unity | 6000.3.6f1 | same | OK |
| Package ID | `com.cds.tankmayhem`; matches `google-services.json` | same | OK |
| Product / company | Tank Mayhem / CodeDrift Studio | same | OK |
| Version name / code | global 1.0.1 / 1; Dev profile 1.0.7 / 7; **Release profile (active) 1.0.4 / 4** (Editor-verified) | owner call | **Owner decision.** Three different values exist. versionCode must exceed every code ever uploaded to Play, including any Dev uploads; Claude cannot see the Play history. Not changed. |
| Min / target SDK | 29 / 36 | target must meet Play's current requirement | Target 36 is expected to be fine. **Confirm in Play Console.** |
| Scripting backend | IL2CPP | IL2CPP | OK |
| Architectures | flag `6` = **ARM64 + X86** (Editor-verified) | ARM64 only | **Owner decision.** X86 is not needed for a phone game. Resolver gradle excludes armeabi-v7a and x86_64 only. Not changed. |
| Stripping | 1 (Minimal) | any that works with Firebase | OK. Do not raise it before testing. |
| Output | Release profile `m_BuildAppBundle: 1` (AAB); Dev profile is an APK. Editor: Build App Bundle on, active profile `Release-Android™` | AAB | Configured. **No AAB built yet** (NOT TESTED). |
| Development / script debugging | Release profile OFF / OFF (Editor: development off); Dev profile ON / ON | OFF / OFF | Configured for Release. Not verified in a built artifact. |
| Autoconnect profiler | OFF | OFF | OK |
| Scenes | only Desert enabled | Desert only | OK |
| Orientation | Release profile: AutoRotation with only Landscape Left/Right allowed (Editor-verified). The **global** settings still allow portrait | landscape | OK for Release-Android. A build made without the profile would allow portrait. |
| Android TV compatibility | **off** (Editor-verified `androidTVCompatibility = false`) | off for a touch game | OK. |
| Internet permission | `ForceInternetPermission: 0` (Editor-verified) | present in final manifest | Firebase's AARs normally merge it. **Verify in the built AAB manifest.** UNVERIFIED. |
| Custom gradle templates | main, properties, settings | — | Present, resolver-managed. Machine-neutral. |
| Signing | custom keystore is configured (an absolute local keystore path and alias are stored in `ProjectSettings.asset` and the build profiles; not repeated here) | upload key, passwords entered at build time | **Do not commit those files as they are.** Signing was not changed or tested. |
| Debug console | The working-tree `Demo_Game_Desert.unity` has **no** reference to `IngameDebugConsole` (0 references; HEAD had 71). The plugin folder and its Android `.aar` are still in the project | absent from release | Not in the scene, so not in a release build by that route. **Confirm in a built release on device.** See §2 about the stripper. |
| Native symbols | Release profile sets a debug symbol level/format (`4` / `5`); what that means for Crashlytics/Play is UNKNOWN | symbols or Crashlytics mapping uploaded | Unverified. See manual actions. |

## 2. Changes made
1. **New** `Assets/Editor/Utilities/ReleaseBuildDebugStripper.cs`. It is Editor-only. In a player build with Development OFF it removes a debug console from the scene copy being built. **It has never been shown to run** (no build log exists). Since the console was later removed from the Desert scene itself (2026-11-12 audit, grep: 0 references), the stripper is now only a safety net and is not evidence of anything. The Editor compiled the project with a clean console, but no player build has run it.
2. `FirebaseRemoteConfigHandler.cs`: the in-code default `match_rounds_to_win` was changed from 5 to 3. Gameplay does not read this key (grep: only the definition), and Desert's `m_NumRoundsToWin` is 3, so **match behaviour is unchanged**. The Firebase Console may still hold a published value of 5. Check it before any code reads the key.

No gameplay, balance, AI or power-up code was touched. Nothing was staged, committed or pushed.

## 3. Firebase: evidence by layer
Legend: S = static read, C = Unity compile, A = Android runtime, F = Firebase Console.

| Service | S | C | A | F | Notes |
|---|---|---|---|---|---|
| Core init | OK. Async, non-blocking, single instance (DontDestroyOnLoad, duplicate destroyed), failure leaves the game running | not run | owner-reported init works | n/a | |
| Anonymous Auth | OK. Reuses `CurrentUser` and does not create a new account on restart; retry loop only while signed out | not run | owner-reported sign-in | not checked | UID stability across restarts untested |
| Analytics | OK. 6 events registered and sent, params typed, `game_session_start` fired once from `Start`. No PII. Buffer is bounded | not run | service activation only | **no DebugView evidence** | Delivery not proven |
| Crashlytics | OK. Native and managed crashes are automatic, plus non-fatal API, keys and breadcrumbs. `LogNonFatal` is never called from gameplay | not run | activation only | **none** | |
| Remote Config | OK. Defaults, fetch, activate between matches. Fetch interval is 12 h outside dev builds. Falls back to defaults | not run | not observed | not checked | No gameplay reads any key, so it has no gameplay effect |
| Firestore diagnostic | `#if UNITY_EDITOR || DEVELOPMENT_BUILD`, so **absent from release**. Trigger is an Editor menu only | not run | instance acquired only | none | **No write/read/delete round-trip has ever been shown.** Needs security rules for `diagnostics/{uid}`. Rules unchanged by Claude. |
| App Check | Libraries linked (appcheck, debug provider, play-integrity), no code uses them, no enforcement | — | — | — | Low risk while enforcement is off. The debug provider ships in the binary. Do not remove without an approved dependency change. |

### Privacy and policy items (owner or legal, not decided here)
- **Analytics and Crashlytics collect by default** (`k_DefaultCollectionEnabled = true`) with no consent UI. DR-P13 (audience and age group) is still open and it determines whether this is acceptable. For internal testing with named testers, the risk is limited.
- Firebase Auth creates an anonymous user ID. Analytics adds Firebase's app-instance ID and device data automatically.
- The Play **Data Safety** form and a **privacy policy URL** are owner tasks. Declare per the SDKs' actual collection. This document makes no compliance claim.

## 4. Stability and performance
No profiling data exists. The historical frame skips at startup are **unexplained**. Firebase initialisation is asynchronous, but the native libraries still load and `CheckAndFixDependenciesAsync` runs early. On-device profiling is required before claiming either way. Back button and pause on background are known gaps (KI list, M3/M4): there is no auto-pause when the app is backgrounded (held on-screen inputs are released on focus loss, static code only). A `MobileControlSafeArea` script exists for the on-screen controls, but it has not been verified on a device. `androidRenderOutsideSafeArea: 1` is set. All of this is **NOT RUNTIME VERIFIED**.

## 5. Risks
| Risk | Level |
|---|---|
| X86 ABI included alongside ARM64 (larger package, unneeded) | Low. Owner decision on ARM64-only. |
| versionCode lower than a previously uploaded one (three different values in the project) | Medium. Owner must check the Play Console. |
| Shipping with the debug console | Low: not in the Desert scene (static). Verify on a built release. |
| Keystore path or secrets committed | Medium if `ProjectSettings.asset` or the build profiles are committed as-is |
| Frame skips on devices other than the OnePlus 12 | Unknown |
| Analytics on by default | Policy decision |

## 6. Build evidence
None. No AAB was built. On 2026-11-12 the Unity Editor was not compiling and its console showed 0 errors and 0 warnings, and an Editor command script compiled and ran. That is an Editor compile result only, not an IL2CPP player build.

## 7. Open items, in priority order
1. No AAB has been built or inspected (manifest, ABIs, symbols, targetSdk, versionCode). Needs the Editor and the keystore passwords.
2. Owner decisions: versionCode/versionName (three values exist), ARM64-only.
3. The Play Console and Data Safety items are not done. Privacy policy URL: not verified to exist.
4. No device test has been run (see `ANDROID_RELEASE_TEST_PLAN.md`).
