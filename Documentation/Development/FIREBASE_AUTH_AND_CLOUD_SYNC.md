# Firebase Authentication & Cloud Sync — status and design

Status date: 2026-11-08.
**Editor compilation: VERIFIED** (see "Editor validation" below). **Runtime behaviour, Android build and device tests: NOT validated** — no sign-in, Firestore call, Analytics event or Gradle build has been executed.

> **Installing the Firestore SDK does NOT mean cloud sync is operational.** Today there is no cloud sync, no automatic
> reading or writing, no stored player fields, and no game code that calls Firestore. Only a dormant data-access layer exists.

## 1. Implemented (code written, unvalidated)

| Piece | File (under `Assets/_Tanks/Scripts/Services/Firebase/`) | What it does |
|---|---|---|
| Anonymous authentication | `Authentication/FirebaseAuthenticationHandler.cs` | Reuses the saved Firebase session; signs in anonymously only if none exists (single in-flight guard); retries while signed out. |
| Credential linking hook | same file, `LinkCredential(...)` | Upgrades the anonymous account to a future credential (e.g. GPGS) via `LinkWithCredentialAsync`, keeping the UID. `CredentialAlreadyInUse` changes nothing. No GPGS code exists. |
| Player document access | `PlayerData/FirebasePlayerDataHandler.cs` | Schema-free `ReadPlayerDocument` / `MergePlayerDocument` for `players/{uid}`. Returns explicit statuses (`Unavailable`, `NotSignedIn`, `NotFound`, `Failed`, `Success`). No fields are defined. Nothing calls it. |
| Central access | `Core/FirebaseManager.cs` | `AuthStatus`, `IsSignedIn`, `UserId`, `IsAnonymousAccount`, `AuthStatusChanged`, `Authentication`, `PlayerData`. Reuses the existing init/status lifecycle; no second initializer. |

Responsibilities: auth owns identity; the player-data handler gets the UID only through a delegate and never touches auth state.

### Authentication flow
1. Firebase init succeeds (existing `FirebaseManager` lifecycle).
2. Auth handler activates: `CurrentUser != null` → reuse (no sign-in call). Otherwise one anonymous sign-in.
3. Failure (offline etc.) → status `SignedOut`, game unaffected; retry every 20 s while a connection is reported, and on app focus.

### Installed SDK (VERIFIED 2026-11-08, static + Unity Editor)
Firebase Unity SDK **13.16.0**, imported as a unitypackage under `Assets/Firebase` (not UPM): App, Analytics, Crashlytics, Remote Config, Auth, Firestore, App Check (App Check ships in the same bundle; unused by our code). All module manifests are 13.16.0, so there is no version mix. Managed DLLs are Android + Editor enabled; no duplicate assemblies are loaded.

Android resolver output (`ProjectSettings/AndroidResolverDependencies.xml`, no duplicate artifacts): `firebase-analytics` 23.2.0, `firebase-auth` 24.2.0, `firebase-config` 23.1.0, `firebase-firestore` 26.6.0, `firebase-crashlytics-ndk` 20.1.0, `firebase-common` 22.2.0, `firebase-appcheck` (+ `-debug`, `-playintegrity`) 19.4.1, `play-services-base` 18.10.1, and the `*-unity` 13.16.0 artifacts from `Assets/GeneratedLocalRepo`. The same list is patched into `Assets/Plugins/Android/mainTemplate.gradle`; `settingsTemplate.gradle` adds the local Maven repo.

### Editor validation (VERIFIED 2026-11-08 via Unity MCP, Editor on Android build target)
- Unity not compiling; no console errors (one unrelated Unity AI toolkit warning).
- All `Tanks.Complete.FirebaseSystem.*` handler classes are in `Assembly-CSharp`; `FirebaseFirestore`, `FirebaseAuth`, `FirebaseAnalytics`, `Crashlytics`, `FirebaseRemoteConfig` resolve from their own Firebase assemblies.
- `google-services.json` matches the generated `google-services.xml` (project id, app id, API key) and the Android package id `com.cds.tankmayhem`.
- NOT performed: Play Mode run, any sign-in or Firestore call (would create a real anonymous account in the Firebase project), Android Gradle build, device test.

### OPEN ISSUE — Android Gradle templates are disabled (blocks Android build)
Live PlayerSettings has `useCustomMainGradleTemplate`, `useCustomGradleSettingsTemplate`, `useCustomGradlePropertiesTemplate` (and base/launcher) all **False**, although the template files exist and EDM4U patched them. Unity ignores template files that are not enabled, so the local Maven repo and Firebase dependencies would not reach Gradle. Required before an Android build (project-wide Android setting, owner approval per CLAUDE.md §G): Project Settings → Player → Android → Publishing Settings → enable **Custom Main Gradle Template**, **Custom Gradle Settings Template**, **Custom Gradle Properties Template**, then run Assets → External Dependency Manager → Android Resolver → Force Resolve and confirm the three files are unchanged or only trivially re-patched. Not done by Claude.

### Review notes
- `google-services.json` and EDM4U-generated files are untracked and not git-ignored; decide what to commit.
- `firebase-appcheck-debug` is bundled into release builds although App Check is unused; consider removing the App Check module (owner decision).
- If a stored anonymous account is deleted/disabled server-side, `CurrentUser` stays non-null locally and Auth reports `SignedIn`; Firestore calls would then fail with a permission error. Not handled; low priority.
- If Firebase initialization fails, `AuthStatus` now reports `Unavailable` (fixed 2026-11-09; it used to stay `NotStarted`).

## 2. Planned — NOT implemented, needs decisions
- **Player data model.** The game currently has no player data (no progression, currency, settings persistence, match history). No fields were invented. A real requirement must exist first.
- **`FirebaseCloudSyncHandler`.** Orchestration: initial read after sign-in, upload if no cloud record, download for returning users, sync on change, retry when connectivity returns, write de-duplication.
- **Local save layer.** Only if the actual data requires one; offline-first then uses it as the fallback. Not created.
- **Conflict-resolution policy.** Candidate: per-field `localRevision`/server `updatedAt` comparison; never overwrite higher with lower; counters take max; ask or take newest when settings diverge. Needs owner decision when real fields exist.
- **Account-link merge policy** for `CredentialAlreadyInUse`.
- Consent: the data-collection switch (`firebase_data_collection`) controls Analytics/Crashlytics only. It is not player data and must never be synced as such.
- Transient gameplay state (rounds, health, positions) is never to be synced.

Adding real data later needs: define fields → add the sync handler in `Services/Firebase/Sync/` → construct it in `FirebaseManager` next to the other handlers. `FirebasePlayerDataHandler` and auth need no restructuring.

## 3. Firestore security rules (documented only — NOT deployed)
```
rules_version = '2';
service cloud.firestore {
  match /databases/{database}/documents {
    match /players/{uid} {
      allow read, write: if request.auth != null && request.auth.uid == uid;
    }
    match /{document=**} {
      allow read, write: if false;
    }
  }
}
```
> **These rules do NOT allow the connectivity diagnostic.** The catch-all deny rejects `diagnostics/{uid}` (the diagnostic then fails with `Missing or insufficient permissions` within about 1 s). To run the diagnostic, publish the rule set in section 8, which adds a `diagnostics/{uid}` match before the catch-all.

Add field validation (types, size, `schemaVersion`) once the schema is final.

## 4. Firebase Console requirements (not done by Claude)
- Authentication → enable **Anonymous**.
- Create the Firestore database (production mode; region decision pending) and publish the rules above. Without rules or a database, Firestore calls will fail; the game is unaffected.
- Before GPGS linking: register SHA-1/SHA-256; `google-services.json` has no `oauth_client` entries yet.
- Privacy: the Firebase UID is pseudonymous personal data; cover it in the privacy policy. It is not sent to Analytics or Crashlytics.

## 5. Behaviour notes (Firestore SDK)
- Offline persistence is on by default on Android; settings were left untouched.
- Writes queue offline and their completion callbacks fire only after server acknowledgement, possibly much later.

## 6. Manual verification still required
- Enable the Android custom Gradle templates (see OPEN ISSUE), force-resolve, then build an Android APK/AAB and confirm Gradle resolves Auth/Firestore/App Check together.
- Editor Play Mode: Firebase `Initialized.` log, no exceptions from handlers (use a test project or accept creation of an anonymous user).
- Restart test: UID identical across launches (and not a new account each launch).
- First launch offline → later online: signs in, one account.
- Scene reload: single manager, single sign-in.
- Optional dev-only round trip of `MergePlayerDocument`/`ReadPlayerDocument` once the console database and rules exist.

---

## 7. Authentication diagnostics (added 2026-11-09)

### Defect found and fixed (root cause of "no user in Console")
`FirebaseServiceBase.Activate()` set `IsAvailable = true` only **after** `OnActivate()` returned. The Auth handler issued its first
sign-in from inside `OnActivate()` through `Guard()`, and `Guard()` returned early while `IsAvailable` was still false. Result:
no sign-in request was ever sent, the in-flight flag stayed set, the status stuck at `SigningIn`, and the retry loop (which only
acts on `SignedOut`) never fired. Firebase init still looked successful, so nothing appeared in the Console.
Fix: `IsAvailable` is true while `OnActivate()` runs (reset to false if it throws). Also: a synchronous throw from the sign-in call
now clears the in-flight flag; `SignedIn` is only reported when `CurrentUser != null`; `AuthStatus` is `Unavailable` if Firebase init failed.
**Status: fixed in code and compiled in the Editor. NOT yet confirmed on a device** — the issue is only closed after a successful Android test.

### Actual execution flow
1. `FirebaseManager.Awake` (auto-created before first scene) → `FirebaseInitializer.Run` → `CheckAndFixDependenciesAsync`.
2. Status `Ready` → services activate in order: Crashlytics, Analytics, RemoteConfig, **Auth**, Firestore.
3. Auth: `FirebaseAuth.DefaultInstance` → `CurrentUser` exists? yes: reuse (no sign-in). No: one `SignInAnonymouslyAsync`; the result is checked and `CurrentUser` re-read.
4. `AuthStatusChanged` fires; the manager cross-checks that its public `UserId` equals the handler's UID.
5. Retry every 20 s / on app focus only while `SignedOut`. A 15 s watchdog prints a verdict if still not signed in.

### Log format
All lines: `[FIREBASE][CATEGORY] message`. Info lines exist in the Editor and **Development builds only**; Warning/Error always.
`CORE` manager lifecycle · `INIT` dependency check and FirebaseApp · `AUTH` authentication · `FIRESTORE` data access ·
`ANALYTICS` · `CRASHLYTICS` · `REMOTE_CONFIG`. UID, project id and app id are logged for diagnostics (not secrets); API keys, tokens and credentials never are.

### Expected successful sequence — first launch (new anonymous user)
```
[FIREBASE][AUTH] Authentication handler created.
[FIREBASE][CORE] FirebaseManager created (persistent host). status=NotStarted
[FIREBASE][INIT] Initialization started. platform=Android package=com.cds.tankmayhem version=...
[FIREBASE][INIT] Dependency check started.
[FIREBASE][INIT] Dependency check completed. status=Available
[FIREBASE][INIT] FirebaseApp available. name=[DEFAULT] projectId=<your project> appId=...
[FIREBASE][INIT] Initialization SUCCEEDED. Activating services.
[FIREBASE][CRASHLYTICS] / [ANALYTICS] / [REMOTE_CONFIG] ... activation started / completed
[FIREBASE][AUTH] Authentication: activation started.
[FIREBASE][AUTH] Authentication initialization started.
[FIREBASE][AUTH] FirebaseAuth instance acquired.
[FIREBASE][AUTH] Current user is null (initial activation).
[FIREBASE][AUTH] Auth status NotStarted -> SigningIn (anonymous sign-in requested)
[FIREBASE][AUTH] Anonymous sign-in requested. attempt=1
[FIREBASE][AUTH] Sign-in request completed. faulted=False canceled=False
[FIREBASE][AUTH] Anonymous sign-in SUCCEEDED (new user). uid=... isAnonymous=True created=... lastSignIn=...
[FIREBASE][AUTH] Auth status SigningIn -> SignedIn (anonymous sign-in succeeded)
[FIREBASE][AUTH] Manager view: AuthStatus=SignedIn IsSignedIn=True UserId=... IsAnonymousAccount=True
[FIREBASE][AUTH] Watchdog: signed in after 15s. OK.
```
Later launches: instead of the sign-in lines you must see `Existing user session found; NOT creating a new anonymous user. uid=<same uid>`.
Search tip: `adb logcat -s Unity | findstr FIREBASE` (PowerShell: `adb logcat -s Unity | Select-String FIREBASE`).

### Telling the failure cases apart
| Case | What the log shows |
|---|---|
| **A** Firebase ready, auth never started | No `Anonymous sign-in requested` line; watchdog `A: ...`; `[FIREBASE][AUTH] ... activation FAILED` with exception; or `INIT ... FAILED` and `AuthStatus=Unavailable`. |
| **B** Sign-in started but failed | `Anonymous sign-in FAILED ... ErrorCode=N AuthError=<name>`; watchdog `B: ...`. `OperationNotAllowed` = Anonymous provider disabled in Console; `NetworkRequestFailed` = no connectivity; `ApiNotAvailable`/key errors = Google Play services or API-key restrictions (check the key's Android restrictions: package + signing SHA-1 of this APK). |
| **C** Sign-in OK but UID not at manager | `Anonymous sign-in SUCCEEDED ... uid=X` followed by `[FIREBASE][AUTH] UID NOT propagated` (Error). |
| **D** Wrong Firebase project/console | Success lines present, but `INIT ... projectId=` differs from the project open in the Console, or the Console is filtered/other project. Compare `projectId`, `appId`, `package`. |
| **E** Session restored | `Existing user session found; NOT creating a new anonymous user. uid=...` and `signInAttempts=0` in diagnostics; no new Console user is expected. Compare `created=` with the Console user's creation time. |
Call `FirebaseManager.LogDiagnostics("reason")` at any time for a one-line snapshot of every service, auth state and network.

### Android test procedure
1. Firebase Console → Authentication → Sign-in method: **Anonymous = Enabled**. Confirm the project id matches the `projectId` logged.
2. Build the Development APK (Dev-Android profile), install, **start once with network**, run logcat as above.
3. Expect the first-launch sequence. Check Console → Authentication → Users for the logged `uid` (refresh; may take a few seconds).
4. Force-stop and relaunch: expect `Existing user session found ... same uid`, no new user.
5. Optional: airplane mode on first launch of a fresh install, then network on: expect a failure (`NetworkRequestFailed`), retries every 20 s / on focus, then success with exactly one user.
Do not clear app data repeatedly: each fresh install creates a new anonymous user.

### Still unverified
Android runtime behaviour of everything above. `UserMetadata` timestamps were compiled but not exercised.

---

## 8. Firestore connectivity diagnostic (added 2026-11-09)

**Status (2026-11-09): VERIFIED in the Windows Unity Editor (Android build target) against project `tank-mayhem-ff62d`; NOT yet verified on an Android device.**
Evidence: Play Mode log `PlayMode_2026-11-09_12-35-05.log`, run at 12:35:17 (UTC 07:05:17), anonymous UID `hWyXPUoD99bdPx4k3EZwSNbJTa63` restored from the saved session: write COMPLETED (0.7 s, server-acknowledged), read from `Source.Server` COMPLETED (0.4 s), all 4 fields matched, delete COMPLETED (0.3 s), `RESULT=SUCCESS ... documentRemains=no`, 0 errors. Earlier runs at 12:32 failed with `Missing or insufficient permissions` because only the section 3 rules (no `diagnostics` match) were published; the run succeeded once the section 8 rules were published. This proves: Firestore API enabled, database exists, anonymous auth reaches Firestore, rules allow `diagnostics/{uid}` for the owner. It does not prove Android runtime behaviour, offline behaviour, or the negative cases (another UID denied, unauthenticated denied). Remaining: section 8.2 Android Development APK test, and removing or restricting the `diagnostics` rule before release.
This is a connectivity and security test only. It is not player-data sync and defines no permanent fields.

### What it does
`Services/Firebase/PlayerData/FirebaseFirestoreDiagnostic.cs` (compiled only in the Editor and Development builds) runs, on manual trigger only:
1. Firebase initialized? 2. `AuthStatus == SignedIn` and UID present? (If not, **no Firestore call is made.**) Firestore instance acquired.
3. Writes `diagnostics/{uid}` with `test_id`, `created_at`, `test_message`, `authenticated_uid`.
4. Reads it back with `Source.Server` (so the local cache cannot fake a pass). 5. Compares all four fields.
6. Deletes the document. Cleanup is also attempted if read or verification failed. Final line states whether the document remains (yes / no / UNKNOWN).

Every step has a 15 s timeout (offline writes never complete in the SDK). SUCCESS is printed only if every step passed and the document is known to be removed.
Errors are classified: permission denied (rules/API), unavailable/deadline (network), not found (database missing), unauthenticated.
All lines: `[FIREBASE][FIRESTORE] Diagnostic: ...`; the last line starts `Diagnostic: RESULT=SUCCESS|FAILURE`.

### How to run
Editor Play Mode (Firebase initialized and signed in, wait for the `Anonymous sign-in SUCCEEDED` line) → menu **Tools > Firebase > Run Firestore Diagnostic**.
From code or a debug button: `FirebaseFirestoreDiagnostic.Run(report => ...)`. Never called at startup. This uses the live Firebase project: it writes and deletes one document.

### Required test rules (documented only — NOT deployed by Claude)
Add to the existing rules; the catch-all deny stays last so everything else remains denied:
```
rules_version = '2';
service cloud.firestore {
  match /databases/{database}/documents {
    match /diagnostics/{uid} {
      allow read, write: if request.auth != null && request.auth.uid == uid;
    }
    match /players/{uid} {
      allow read, write: if request.auth != null && request.auth.uid == uid;
    }
    match /{document=**} {
      allow read, write: if false;
    }
  }
}
```
Remove the `diagnostics` match after validation, or restrict it further. Expected negative result: with the catch-all only (no `diagnostics` rule) the run must FAIL with PERMISSION DENIED and report `documentRemains=no`.

### Console steps still required (owner)
Create the Firestore database; Authentication → Anonymous enabled; publish the rules above.

### 8.1 First Editor run: write timeout investigation (2026-11-09)
Evidence: `%LOCALAPPDATA%\Unity\Editor\Editor.log` (Windows Editor, Android build target; NOT a device log). Project `tank-mayhem-ff62d` matches `google-services.json` (project_id, package `com.cds.tankmayhem`).
Sequence in the log, in order:
1. Diagnostic steps 1-2 and Firestore instance acquisition pass; the write starts.
2. Native SDK, repeatedly: `WriteStream ... Permission denied: Cloud Firestore API has not been used in project tank-mayhem-ff62d before or it is disabled.` The SDK treats this as retryable, so the task stays **pending** instead of faulting. This is why the diagnostic saw only a timeout.
3. 15 s later: diagnostic reports "did not complete". (Its message wrongly suggested a network problem; the network was reachable.)
4. Later in the same session the message changes to `Permission denied: Missing or insufficient permissions.` followed by `Write at diagnostics/<uid> failed: Missing or insufficient permissions.` (twice). The API became enabled in the meantime, the database answered, and the **security rules rejected the write**. The pending write therefore **completed as failed**: it will not be applied later; the document was not created.
Conclusion: root cause = Cloud Firestore API/database not ready at first, then rules not permitting `diagnostics/{uid}` (rules not published, or published without the diagnostics match). Auth, init, project id, path and data types are not implicated.
Confirmed implementation defects (fixed in code, compile NOT yet verified): (a) timeout reported as failure/network and the real native error was never surfaced; (b) the old version issued a cleanup delete while the write was still pending (second operation on the same document, which is why the log shows two failed writes); (c) 15 s was too short for the first call against a freshly provisioned database. Now: 30 s timeout, start/end timestamps and task status per operation, late completion logged, no delete or new run while a write is pending, last native SDK message included, `TimedOutPending` distinct from `Failed`.

### 8.2 Validation plan
**Editor:** Console: enable the Cloud Firestore API, create the database, publish the rules from section 8 (wait ~1-2 min). Stop Play Mode (clears any queued write), enter Play Mode, wait for `Anonymous sign-in SUCCEEDED`/`Existing user session found`, run Tools > Firebase > Run Firestore Diagnostic. Expect `write COMPLETED`, `read COMPLETED`, `all 4 fields match`, `delete COMPLETED`, `RESULT=SUCCESS`.
**Android Development APK:** build with the Dev profile (requires the Gradle template open issue in section 1 to be resolved first), add a temporary debug button or script calling `FirebaseFirestoreDiagnostic.Run()` (no UI exists; do not ship it). Run `adb logcat -s Unity | findstr FIREBASE`. Confirm `projectId=tank-mayhem-ff62d`, the same UID as in Console > Authentication (an existing session on a fresh install gets a NEW UID; compare with the UID logged on that device), then the same SUCCESS sequence.
