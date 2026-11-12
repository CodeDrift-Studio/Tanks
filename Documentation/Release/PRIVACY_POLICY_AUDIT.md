# Privacy Policy Audit — Tank Mayhem

Date: 2026-11-12 · Branch: `feature/android-release-roadmap` · Method: **static inspection only** (source, Gradle, Firebase config, and the merged manifest read out of the existing `Builds/Playstore/TankMayhem.aab`). Nothing was run on a device; no network traffic was captured. Labels: **VERIFIED** (read in this session from files/build output), **DOCS** (Google's published behaviour, not checked against this build), **UNVERIFIED**, **OWNER**.

Deliverables: `PRIVACY_POLICY.md`, `PRIVACY_POLICY.html` (generated from the .md; keep them in sync), this file.

## 1. Reference policy

`https://codedriftstudio.blogspot.com/2020/12/privacy-policy.html` was fetched successfully. It is the policy for a different game ("Save the Monkey", `com.cds.savethemonkey`, effective 1 Sep 2026) with 19 sections, including AdMob, a local-storage section and a data-deletion section. Its structure, tone and contact block were followed; the AdMob/ads sections were **not** carried over because Tank Mayhem has no ads SDK. Notably, that policy says its game is "directed to children under the age of 5" while using analytics/ad SDKs. That wording was not reused; the audience question is an open owner decision (§6).

## 2. What the game actually collects (VERIFIED from code unless marked)

| Item | Finding | Source |
|---|---|---|
| Services started at launch | `FirebaseManager` bootstraps itself before the first scene (`RuntimeInitializeOnLoadMethod`) and starts Crashlytics, Analytics, Remote Config, Auth, Firestore handler. | `Services/Firebase/Core/FirebaseManager.cs:71-141` |
| Collection default | `k_DefaultCollectionEnabled = true`; Analytics and Crashlytics are **on by default with no consent prompt**. | `FirebaseManager.cs:21,242-258` |
| Opt-out in UI | `SetDataCollectionEnabled` has **no caller anywhere**; there is no settings/consent screen. | grep over `Assets/_Tanks` |
| Analytics events | 6 custom events: `game_session_start`, `match_start`, `tank_selected`, `round_complete`, `match_complete`, `powerup_collected`, with the parameters listed in policy §4.2. Event comment says no personal data. | `FirebaseEventRegistry.cs`, `FirebaseAnalyticsHandler.cs` |
| Analytics call sites | `GameManager` (match start/round/match complete, game state), `GameUIHandler:202` (tank name), `PowerUp:81` (power-up type). | grep |
| User ID in analytics | Not set. `SetUserId` / user properties are not used; the UID is not passed to Analytics or Crashlytics (code comment says do not). | grep |
| Crashlytics | Collection flag, custom keys `app_version`, `game_state`, `round_number`, `arena`. `LogBreadcrumb`, `LogNonFatal`, `Log` exist but have **no gameplay callers**. Uncaught crashes are captured by the SDK. | `FirebaseCrashlyticsHandler.cs`, grep |
| Anonymous Auth | `SignInAnonymouslyAsync` once if no saved user; retries while signed out and online; reuses the saved session. No email/name/credential. `LinkWithCredentialAsync` exists for a future GPGS link but is not called. | `FirebaseAuthenticationHandler.cs` |
| Remote Config | Keys `config_version`, `match_rounds_to_win`, `powerup_respawn_seconds`; fetch interval 12 h in release builds (0 in dev); activated only between matches; **no gameplay code reads the values**. No custom signals/user properties sent. | `FirebaseRemoteConfigHandler.cs`, MANUAL_ACTIONS_REQUIRED.md |
| Firestore | Handler acquires `FirebaseFirestore.DefaultInstance` only. `ReadPlayerDocument` / `MergePlayerDocument` (`players/{uid}`) have **no callers**. No automatic sync. | `FirebasePlayerDataHandler.cs`, grep |
| Firestore diagnostic | Wrapped entirely in `#if UNITY_EDITOR \|\| DEVELOPMENT_BUILD`; manual trigger only; writes/deletes `diagnostics/{uid}`. Not in a non-development build. | `FirebaseFirestoreDiagnostic.cs:1` |
| App Check | Libraries (core, debug, Play Integrity) are in the build; **no C# code activates App Check**. | Gradle, grep |
| Other network code | No `UnityWebRequest`/`HttpClient` in game scripts. `IngameDebugConsole` plugin matched the grep (not investigated; see §7). | grep |
| Unity Analytics / Cloud | Disabled in `UnityConnectSettings.asset` (`m_Enabled: 0`); `cloudEnabled: 0`. | settings |
| Ads / IAP / GPGS | **Absent.** No AdMob, IAP, GPGS or attribution SDK in `Packages/manifest.json`, Gradle dependencies or Assets. | manifest, `mainTemplate.gradle` |
| Identifiers read directly | No `deviceUniqueIdentifier`, advertising-ID API, microphone, camera or location calls in game scripts. | grep |

## 3. SDK-by-SDK

| SDK (version) | Active in production? | Data it handles | Notes |
|---|---|---|---|
| Firebase Unity SDK 13.16.0 / `firebase-common` 22.2.0 | Yes | Installation ID, config from `google-services` values | Needed by all |
| `firebase-analytics` 23.2.0 (+ `play-services-measurement`) | Yes | Events above + automatic events, app-instance ID, device/app info, IP-derived coarse location, Google Advertising ID if available (DOCS) | Declares AD_ID, ACCESS_ADSERVICES_* |
| `firebase-crashlytics-ndk` 20.1.0 (+ unity 13.16.0) | Yes | Crash traces, device state, custom keys (DOCS for the exact device-state fields) | Includes `firebase-sessions` (`SessionLifecycleService` seen in manifest) |
| `firebase-auth` 24.2.0 | Yes | Anonymous UID | Brings `RecaptchaActivity`, `GenericIdpActivity`, credentials/Play-services sign-in components into the manifest, though only anonymous sign-in is used |
| `firebase-config` 23.1.0 | Yes | Fetch request with installation identifiers (DOCS) | Values unused by gameplay |
| `firebase-firestore` 26.6.0 | Linked, **idle** | Nothing written/read in normal play | SDK may still keep a local cache/connection when a document is accessed; none is accessed |
| `firebase-appcheck` 19.4.1 (+debug, playintegrity) | Linked, **not activated** | None unless activated | Debug provider ships in release; see §7 |
| Unity engine / IL2CPP | Yes | Not investigated for engine-level telemetry | Unity Analytics/cloud diagnostics disabled in settings |

## 4. Android permissions (VERIFIED from the merged manifest in `Builds/Playstore/TankMayhem.aab`)

Present: `INTERNET`, `ACCESS_NETWORK_STATE`, `WAKE_LOCK`, `com.google.android.gms.permission.AD_ID`, `android.permission.ACCESS_ADSERVICES_AD_ID`, `android.permission.ACCESS_ADSERVICES_ATTRIBUTION`, `com.google.android.providers.gsf.permission.READ_GSERVICES`, `com.google.android.finsky.permission.BIND_GET_INSTALL_REFERRER_SERVICE`, `com.google.android.gms.auth.api.signin.permission.REVOCATION_NOTIFICATION`, `android.permission.BIND_JOB_SERVICE` (service-level), and the app-specific `com.cds.tankmayhem.DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION`.
Not present: camera, microphone, location, contacts, storage, phone state.
Caveats: the AAB was read from disk; I did not confirm it is the build that will be submitted (several versions have been built; `Builds/Playstore` holds symbols for 1.0.1–1.0.5). The policy lists the permissions at category level. Re-check the final AAB (`bundletool dump manifest`) before publishing. The binary manifest was inspected by extracting strings, not with a full manifest decoder.

## 5. Verified vs unverified claims in the policy

**Verified (from code/build output):** no ads/IAP/GPGS; anonymous auth, no PII requested; the six events and their parameters; Crashlytics keys; Remote Config keys and 12 h interval; Firestore idle in normal play; diagnostic is Editor/Development only; no UI toggle for collection; collection default on; no in-game deletion; permissions list; SDK version 13.16.0.

**From Google documentation, not checked against this build (wording in the policy is hedged: "typically", "may"):** exact automatic Analytics fields; IP-derived location; Advertising ID collection conditions; Crashlytics device-state fields; what Remote Config requests carry.

**Not verified / owner must confirm:**
- That Firebase data does not leave Google's processing (Google may also use some data under its own terms; the policy says so but does not characterise it legally).
- "We do not run our own servers" — true for this repository; confirm nothing external exists.
- "We do not sell your information" — nothing in code contradicts it, but it is a business statement.
- "Limit access to the Firebase project to authorised members" and any Firestore/App Check protections — not inspectable from the repo (Firestore security rules are not in the repository).
- Retention periods — deliberately **not stated**.
- All external URLs in policy §11 were **not opened**; confirm they resolve (the Crashlytics anchor in particular).
- Policy text was not reviewed by a lawyer; it makes **no** GDPR/CCPA/COPPA/DPDP compliance claims.

## 6. Missing business / legal information

- Effective date.
- Confirmation of the contact email and location. These were taken from the published Save the Monkey policy (`studio.codedrift@gmail.com`, Dehradun, Uttarakhand, India); neither is confirmed for Tank Mayhem. No postal address, grievance/data-protection officer or response-time commitment exists.
- Legal entity name and registration (only "CodeDrift Studio").
- Target audience / minimum age (DR-P13).
- Website URL where the policy will be hosted (needed for the Play Console).
- Retention settings in the Firebase Console.
- Who processes deletion requests and how.

## 7. Privacy implementation gaps (code/config, not changed by this task)

1. **Analytics and Crashlytics are on by default with no consent or opt-out UI** (also noted as DR-P13 in MANUAL_ACTIONS_REQUIRED). Regions with consent requirements, and child-directed audiences, would need this changed before release. The policy says honestly that it is on by default.
2. **No in-game way to see the anonymous UID, request deletion, or delete the anonymous account.** Deletion by email may be impossible to action because users cannot tell us their UID. Options: show the UID in a settings screen, add a "Delete my data" action (delete Firebase user + Firestore doc), or both. Policy text currently says we may not be able to find records.
3. **`AD_ID` permission and advertising-ID access come from Firebase Analytics even though the game has no ads.** If you do not want Advertising ID collection, it can be removed/disabled (manifest `tools:node="remove"` for `AD_ID`, and/or Analytics ad-ID settings). Needs a decision, because it changes the Data safety answers and affects any future ads.
4. **App Check debug provider (`firebase-appcheck-debug`) is linked into the release build** though App Check is not activated. Confirm it is intentional; if App Check is later enforced, a debug provider in production is a security concern.
5. **Firestore, App Check, Auth UI components** are linked but unused; each adds manifest components. Optional clean-up, not a policy blocker.
6. **Firestore security rules** are not in the repository; confirm the deployed rules deny access by default so an idle Firestore SDK exposes nothing.
7. `Plugins/IngameDebugConsole` is in the project; `Documentation/Release/MANUAL_ACTIONS_REQUIRED.md` already asks to confirm it is not in release builds. Out of scope here.
8. The `google-services` values (project `tank-mayhem-ff62d`) and API key are in the repository; Firebase API keys are not secrets by design, but restrict the key in Google Cloud Console (not checkable here).

## 8. Owner decisions required

1. **Audience / age (DR-P13):** 13+ general audience vs. any child-directed audience. This drives §8 of the policy, Play's Families policy, and whether current defaults are acceptable at all. Placeholder text is provided in the policy.
2. **Consent model:** keep default-on (current), or add a first-run consent / settings toggle. If any EEA/UK users are targeted, a consent flow is likely needed [verify with qualified advice].
3. **Advertising ID:** keep or remove `AD_ID` (§7.3).
4. **Deletion mechanism:** email-only (current policy wording), or build an in-game option (§7.2).
5. **Retention:** set and state Analytics retention; decide on any cleanup of anonymous accounts.
6. **"Do not sell" statement** and security-access statement.
7. **Contact details** and hosting URL.
8. Whether to keep Firestore/App Check in the release build.
9. Publication: the policy must be **public, reachable without login, and linked in both the Play listing and (ideally) the app**. There is no in-app link today.

## 9. Google Play Data safety — items to verify in the Console

Based on what this build does (answers are the owner's to make; this is a checklist, not legal advice):

| Play data type | Likely applies | Basis | Verify |
|---|---|---|---|
| Device or other IDs | Yes | Firebase UID, installation/app-instance ID, Advertising ID (AD_ID declared) | Purpose: Analytics, App functionality; collected, not user-optional |
| App activity → in-app actions | Yes | 6 custom events | Analytics |
| App info and performance → crash logs / diagnostics | Yes | Crashlytics | Analytics / App functionality |
| Approximate location | Possibly | IP-derived location in Analytics (DOCS) | Confirm in Firebase docs/Play guidance |
| Device/hardware info | Possibly | Automatic Firebase fields | Same |
| Personal info (name, email), financial, contacts, photos, audio | No | Not collected by code | — |

Other items to settle:
- **Data sharing:** Play treats transfer to a service provider acting on your behalf as not "sharing", but Google may use some Firebase data under its own terms. Check the current Play and Firebase guidance for the correct answer.
- **Encryption in transit:** Firebase SDK traffic is HTTPS (DOCS); confirm before answering "yes".
- **Data deletion:** Play asks whether users can request deletion and may require a deletion URL where accounts can be created. Anonymous accounts probably do not count as user-created accounts [UNVERIFIED]; confirm the requirement and what you will answer given §7.2.
- **Advertising ID declaration** (Play Console → App content): required because the manifest declares `AD_ID`; answer according to decision 3.
- **Ads:** answer "No ads" only if no ad SDK is added; current build has none (matches PLAY_CONSOLE_CHECKLIST).
- **Privacy policy URL, target audience and content declarations** must match the final policy.
- Re-check Data safety whenever an SDK is added or updated (Firebase SDK updates can change collected fields).

## 10. Is the policy ready to publish?

**No — owner approval and several decisions are required first.** The policy text matches the code as inspected, but it cannot be published while `[[...]]` placeholders remain (effective date, audience/age wording, contact details, retention, the sell/security statements). Items §7.1–7.3 are product decisions that may require code changes, and the policy will need updating if they are made. Rebuild `PRIVACY_POLICY.html` from the `.md` after edits (the converter is not committed; the two files must be kept in sync by hand otherwise).

## 11. Repository safety record

Only these three files were created. No source, scene, prefab, ProjectSettings, build-profile or Firebase configuration file was modified; nothing was committed, pushed or published. Git status before the task showed pre-existing user changes, which were left untouched (the `Documentation/Release/` folder was already untracked).

## 12. Publish version (2026-11-12)

`PRIVACY_POLICY.md` / `.html` were rewritten as a publish version at the owner's request: draft box and all `[[...]]` placeholders removed; sections for features not in use (Firestore, App Check, Unity Analytics note, "what is not in the game", future-change promises) removed; unverified external links removed; the "limit access to Firebase project" and "do not sell" statements removed because they could not be verified. Decisions applied on the owner's instruction, not verified by Claude: effective date 12 Nov 2026; contact `studio.codedrift@gmail.com`; audience 13+ (DR-P13). The location line was dropped. The implementation gaps in §7 (default-on collection, no in-game deletion/consent, AD_ID, linked-but-idle Firestore/App Check) are unchanged and the policy still describes them accurately. Sections were renumbered (now 12): the audit's §1-§11 refer to the earlier numbering.
