# SERVICE INTEGRATION PLAN

Created 2026-11-08. **Planning document only.** Nothing described here is installed, configured, registered or coded. No packages were added; no accounts or console projects were created.

Owner-confirmed (see `DECISION_REGISTER.md`): **Firebase, Google Play Games Services, Google AdMob and Unity IAP / Google Play Billing are mandatory** for the Android first release (Free-to-Play, Ads + IAP). The **feature scope of each service is not assumed** and stays PENDING unless a row below says otherwise.

Evidence labels as in `CLAUDE.md`. Statements about SDK behavior, package names and policies below are **from general knowledge and UNVERIFIED for this project** (Unity 6000.3.6f1, URP 17.3, Android IL2CPP). Each M5/M6 starts with a "verify against current official documentation" task before any install. Policy statements (Google Play, AdMob, Firebase) change; re-check at implementation time.

## 0. Cross-cutting rules (apply to every service)
1. **Package ID first.** Firebase, GPGS, AdMob and Play Billing bind to the Android applicationId (and signing certificate SHA-1/SHA-256). Fix the package ID (DR-P2) and signing plan (DR-P16) before registration. Debug and release certificates differ; both need registering where required.
2. **Gameplay never waits on a service.** Every initialization is asynchronous, time-boxed and failure-tolerant; the game must be fully playable offline and with every service failing. No service call in `Update/FixedUpdate` paths of tanks, AI or match flow.
3. **One façade per service** (interface + null/offline implementation) so the game compiles and runs with a service disabled; feature flags per service; Editor and Windows (dev only) use the null implementation.
4. **Consent before data collection / personalized ads.** Consent state gates Analytics collection and ad personalization. Audience/age decision (DR-P13) comes first; if children are a target audience, the design changes materially (child-directed treatment, restricted SDK use) — **do not implement until DR-P13 is answered**.
5. **Test identifiers only until release.** Test ad units, license-tester accounts, Firebase debug devices. Real IDs only in the release build config, never in committed secrets beyond owner-approved config files.
6. **Secrets / config files** (`google-services.json`, AdMob app ID, Play licence keys): owner decides what is committed; keystores and passwords are never committed.
7. **Dependency resolution on Android** (Gradle / External Dependency Manager) is the main build-risk source; each integration is done in its own branch with a build check on a device (M5/M6 tasks) and a size/permission diff of the manifest.
8. **Order of integration**: Firebase base + Crashlytics → consent flow → Analytics events → GPGS sign-in → (Remote Config if approved) → AdMob (test ads) → IAP. Crash reporting first so later integrations are observable.
9. **Privacy artifacts** derive from this plan: privacy policy, Play Data Safety, ads declaration, content rating (M8).
10. **Android permissions**: record the merged manifest after each SDK is added (e.g. INTERNET, ACCESS_NETWORK_STATE, AD_ID/advertising ID) — UNVERIFIED which each SDK adds; verify from the actual merged manifest.

## 1. Firebase Analytics
| Item | Plan |
|---|---|
| Purpose | Understand sessions, retention, funnel (menu → match → result), feature usage, ad/IAP funnel, device mix. |
| Proposed functionality | Minimal event set designed in M5: `session/app open` (automatic), `match_start`, `match_end` (result, arena, duration, rounds), `arena_selected`, `powerup_collected` (type), `tutorial_step`, `ad_shown/ad_failed` (format, placement), `iap_started/iap_completed/iap_failed`, `settings_changed`. **Event list is a proposal; scope PENDING (DR-P5).** No personal data in parameters. |
| Required dependencies | Firebase project; Android app registration with final package ID; `google-services.json`; Firebase Unity SDK (Analytics + Firebase App); Android Gradle dependency resolution; Google Play services availability on device. |
| Accounts / consoles | Firebase console (project, Android app, SHA-1 optional for Analytics), link to Google Analytics property; Google account owned by studio (DR-P15). |
| Packages/SDK to investigate | Current Firebase Unity SDK distribution format (Unity Package Manager scoped registry/`.tgz` vs unitypackage), compatibility with Unity 6.3 and Android Gradle plugin used by Unity 6.3, External Dependency Manager (EDM4U) version, minSdk requirement vs project min SDK (currently 25, UNVERIFIED). |
| Data collected / processed | App instance ID, device/OS info, events and user properties, advertising ID (if enabled/linked), IP-derived coarse location. |
| Privacy / consent | Collection should be gated on consent where required by region/audience; policy text and Data Safety entries needed; ability to disable collection in settings (decision). |
| Init & failure handling | Async `CheckAndFixDependenciesAsync`-style init (name UNVERIFIED); on failure: disable analytics for session, log once, never block UI; events queued/dropped without throwing. |
| Offline behavior | SDK buffers and uploads later; no gameplay effect. |
| Testing | DebugView on a physical device (debug mode), event/parameter validation, offline then online upload, consent-off verification (no events), release build check, logcat has no exceptions. |
| Acceptance | Defined events appear in DebugView with correct parameters; game fully playable with Analytics init failing; no PII in events; consent-off sends nothing (as documented by the SDK); manifest/size diff recorded. |
| Pending decisions | DR-P5 (scope), DR-P13 (audience), DR-P15 (account), event list approval. |

## 2. Firebase Crashlytics
| Item | Plan |
|---|---|
| Purpose | Crash and non-fatal exception visibility for a live F2P game (the project has no crash reporting today). |
| Proposed functionality | Automatic crash capture; log custom keys (arena, match round, build id, active services); non-fatal logging for caught service/AI errors; breadcrumbs for key lifecycle events; optional user opt-out. |
| Required dependencies | Firebase base (§1); Crashlytics Unity SDK; build step to upload symbols (IL2CPP symbol files for native crashes); Gradle/Crashlytics plugin configuration. |
| Accounts / consoles | Firebase console → Crashlytics enabled for the Android app. |
| Packages/SDK to investigate | Crashlytics Unity package version for Unity 6.3; IL2CPP symbol upload (`il2cpp` symbols / `symbols.zip`) requirements for Release AAB; interaction with Play Console deobfuscation. |
| Data collected / processed | Stack traces, device model/OS, app version, custom keys/logs, crash-time state, installation ID. |
| Privacy / consent | Disclosed in privacy policy/Data Safety; consent handling per region/audience; do not log player-identifying data. |
| Init & failure handling | Init after Firebase base; if dependency check fails, run without it; never crash because Crashlytics failed. |
| Offline behavior | Reports are stored and sent on next launch. |
| Testing | Forced test crash and non-fatal in a **development** build on device; verify symbolicated trace in console (IL2CPP); verify not enabled in Editor; release-build check. |
| Acceptance | A deliberate non-fatal and crash appear, symbolicated, with custom keys; no impact on startup time beyond agreed budget (measured in M8). |
| Pending decisions | DR-P5, DR-P13, DR-P15; whether Crashlytics is the sole crash tool. |

## 3. Firebase Remote Config
| Item | Plan |
|---|---|
| Purpose | Tune balance/ad cadence/feature flags without a store release. **Mandatory-ness of this specific service is not established** — it is assessed here and integrated only if approved (DR-P5). |
| Proposed functionality (if approved) | Server-side defaults for: ad frequency caps/cooldowns, AI difficulty parameters, power-up spawn rates, kill-switch flags per service. Local in-app defaults always present; fetch with timeout and cache. |
| Required dependencies | Firebase base (§1); Remote Config Unity SDK; defaults defined in code/asset; parameter naming/versioning. |
| Accounts / consoles | Firebase console Remote Config; conditions (app version, country) if used. |
| Packages/SDK to investigate | Fetch/activate API semantics, minimum fetch interval, throttling in dev builds, real-time listener support on Unity (UNVERIFIED). |
| Data collected / processed | Instance/installation ID and device attributes needed for targeting. |
| Privacy / consent | Falls under Firebase installation ID disclosure; avoid user-level targeting without disclosure. |
| Init & failure handling | Defaults used at startup; fetch async with timeout; activation only at safe points (menu, never mid-round); malformed values validated and clamped; failure = defaults. |
| Offline behavior | Cached last-activated values or defaults. |
| Testing | Change a value in console, verify activation at next menu entry; offline launch uses cached/defaults; invalid value rejected; kill-switch disables a service in a dev build. |
| Acceptance | Gameplay values come from validated config with safe defaults; no mid-round changes; documented parameter list. |
| Pending decisions | **DR-P5 (include or not)**, which parameters may be remote-tuned, A/B testing yes/no. |

## 4. Google Play Games Services (GPGS)
| Item | Plan |
|---|---|
| Purpose | Player identity on Android, achievements, leaderboards, optionally cloud saves; discoverability on Play Games. |
| Proposed functionality | (a) **Sign-in** (automatic at startup + manual button in Settings; non-blocking). (b) **Achievements** — list designed in M5 (e.g. first win, win with each tank, win each arena, power-up usage) **PENDING DR-P6**. (c) **Leaderboards** — candidate: total wins / win streak vs AI **PENDING DR-P6**. (d) **Saved Games** only if cloud progression is approved (**DR-P7**; default no). |
| Required dependencies | Play Console app with Play Games Services configured (project, OAuth credentials with package ID + signing SHA-1, achievements/leaderboards defined, testers); GPGS Unity plugin; Google Play services on device. Firebase and GPGS may share a Google Cloud project (investigate). |
| Accounts / consoles | Google Play Console (Play Games Services section), Google Cloud project/OAuth consent screen; tester accounts. |
| Packages/SDK to investigate | Current official GPGS Unity plugin version/distribution and **Play Games Services v2 API behavior** (sign-in flow differences, server auth code needs), compatibility with Unity 6.3/Gradle, EDM4U overlap with Firebase, how plugin interacts with Unity IAP. |
| Data collected / processed | Play Games player ID, display name/avatar (as exposed by API), achievement/leaderboard progress, saved-game data if used. |
| Privacy / consent | Disclosed in policy/Data Safety; sign-in must be optional for play; no forced account wall. Play Games user data handling per Google policy. |
| Init & failure handling | Silent sign-in attempt after menu is shown; failure/cancel → play as guest, retry from Settings; never retry loops; handle sign-out, account switch, revoked access. Unlock/score calls queued locally or dropped safely when signed out. |
| Offline behavior | Game fully playable; progress submissions resent when possible (policy: define whether to queue — decision); cloud save conflicts handled only if DR-P7 = yes. |
| Testing | Debug and release signing (SHA-1) both tested; tester account on device; sign-in success/cancel/failure/offline; unlock achievement once (no duplicates); leaderboard submit and read; account switch; no regressions when Play services missing/outdated. |
| Acceptance | Sign-in optional and non-blocking; each approved achievement unlocks exactly once with correct trigger; leaderboard scores appear; failure modes handled without crash; documented ID table (IDs kept out of gameplay code via constants/asset). |
| Pending decisions | DR-P6, DR-P7, DR-P2 (package), DR-P15 (accounts), signing (DR-P16). |

## 5. Google AdMob
| Item | Plan |
|---|---|
| Purpose | Ad revenue for the F2P model. |
| Proposed functionality | Formats **assessed, not assumed (DR-P3)**: *Banner* — least intrusive but occupies screen in a touch-control game (likely menu/results only); *Interstitial* — natural breaks only (after match results, never mid-round) with cooldown and frequency cap; *Rewarded* — optional player-initiated rewards (e.g. continue/extra reward) — requires a defined, approved reward economy (none exists). Placement matrix and caps designed in M6 once approved. IAP "remove ads" interplay decided with DR-P4. |
| Required dependencies | AdMob account and app registration (final package ID); ad units per format; Google Mobile Ads Unity plugin (investigate current distribution/version for Unity 6.3); **User Messaging Platform (UMP) consent SDK / form** for consent; Google Play services; `app-ads.txt` if a developer website is used (UNVERIFIED need). |
| Accounts / consoles | AdMob console (app, ad units, payments, policy center), Play Console link for store listing, Firebase/AdMob linking optional. |
| Packages/SDK to investigate | Mobile Ads plugin version and Android dependency footprint; mediation not planned (owner specified AdMob); `AndroidManifest` AdMob App ID metadata requirement; ads and Unity IAP/Billing library version conflicts (Gradle); advertising ID / `AD_ID` permission handling. |
| Data collected / processed | Advertising ID, IP/device info, ad interaction data, consent signals; personalized vs non-personalized ads. |
| Privacy / consent | UMP consent flow before ad requests where required (EEA/UK etc.); child-directed / under-age-of-consent tagging decisions depend on **DR-P13**; ads declaration and Data Safety; ad content rating filters appropriate to audience. |
| Init & failure handling | Init after consent resolved; ad load asynchronous with timeout; ads **preloaded** at safe points; show only at approved placements; on no-fill/error → continue the flow with no ad, no blocking, bounded retry with backoff; dismissal and app-backgrounding handled (audio/timeScale restore); ad callbacks may arrive on non-main threads (marshal to main). |
| Offline behavior | No ads shown, no waiting, no error dialogs; rewarded buttons hidden/disabled with clear state. |
| Testing | **Test ads only** until release (test device IDs); each format load/show/close/click/no-fill/offline; frequency cap and cooldown timers; consent denied/granted paths; app pause/resume during an ad; rewarded grant exactly once; policy self-check (no accidental-click placements, no ads covering touch controls, no ads during active play); release build with real units on internal track only after owner approval. |
| Acceptance | Only approved placements show ads; caps/cooldowns obey config; consent respected; gameplay never blocked or frozen by ad state; rewarded grants are idempotent; no policy-violating placement in checklist review. |
| Pending decisions | DR-P3 (formats, placements, caps), DR-P13 (audience), DR-P4 (remove-ads IAP), DR-P15, rewards economy (if rewarded). |

## 6. Unity IAP / Google Play Billing
| Item | Plan |
|---|---|
| Purpose | In-app purchases for the F2P model (products undecided). |
| Proposed functionality | Product catalog (**none assumed; DR-P4**) with types consumable / non-consumable / (subscription not assumed). Purchase flow, entitlement grant, ownership re-query on startup/resume, restore (non-consumables) via re-query of owned purchases, handling of pending/cancelled/failed/refunded purchases, duplicate-grant prevention via transaction IDs persisted locally. Server-side receipt validation: **not planned** (no backend; Firebase Functions would be an additional service — decision needed if fraud risk is considered material). |
| Required dependencies | Unity IAP package (`com.unity.purchasing`, version/API generation to investigate for Unity 6.3 — the API differs between major versions; UNVERIFIED), Google Play Billing Library via the Unity IAP package, Play Console app with in-app products published/active, a signed build uploaded to a testing track (Billing needs a Play-distributed build), licence testers, final package ID. |
| Accounts / consoles | Play Console (merchant/payments profile, in-app products, licence testing, internal testing track); Unity Dashboard only if required by the chosen package version (investigate). |
| Packages/SDK to investigate | Current `com.unity.purchasing` version and initialization API; whether Unity services/Dashboard linkage is mandatory in the current version; Billing Library version required for Play target API; behavior with AdMob/GPGS/Firebase dependencies (Gradle conflicts); pending-purchase support for consumables. |
| Data collected / processed | Purchase tokens, product IDs, order IDs (processed by Google/Unity IAP); local entitlement state. |
| Privacy / consent | Policy mentions purchases; Data Safety "purchase history"; consumer-law display of prices from the store (localized price strings from store, never hard-coded). |
| Init & failure handling | Async store connect after menu; if unavailable (no Play services, offline, billing unsupported) → store UI disabled with clear message; purchase results always routed through one entitlement function; pending purchases shown as pending, grant only on completion; confirm/consume only after grant is persisted; retry on app start; guard against double-tap and re-entrancy; handle user cancel without error. |
| Offline behavior | Previously granted entitlements work offline from local storage; new purchases require connectivity; owned purchases re-queried on next connect (also handles refunds/revocation). |
| Testing | Licence-tester purchases on a real device from a Play testing track: success, cancel, pending (slow card), failure, already-owned, restore after reinstall, app killed mid-purchase, offline start, refund/revocation re-query, duplicate prevention, price localization; no purchase flow in Editor except mock. |
| Acceptance | Each approved product grants its entitlement exactly once; cancellation/failure leaves state unchanged; restore/re-query works after reinstall; refunds revoke non-consumable entitlement (policy decision documented); no blocked UI; entitlement persists across restarts. |
| Pending decisions | DR-P4 (catalog, prices), server validation yes/no, refund policy behavior, DR-P2, DR-P16. |

## 7. Player data (supports §4, §5, §6)
Local-first data model: versioned JSON (or PlayerPrefs for trivial flags — ADR-010 pending) with schema version, migration function, atomic write (temp + replace), corrupt-file recovery (fall back to defaults and keep a backup), and explicit ownership: **local** = settings, tutorial flags, entitlements cache; **cloud (only if DR-P7 = yes)** = progression; **never trust** client-stored entitlements as the only source (re-query IAP). Detailed tasks are in `Milestones/M5`.

## 8. Effort summary (developer working days; implementation + testing; excludes external waiting)
| Service | Implementation | Testing | Total | External waiting / dependencies |
|---|---|---|---|---|
| Firebase base + Analytics + Crashlytics + consent plumbing | 4–8 | 2–4 | 6–12 | Firebase project/accounts; SDK/Gradle surprises |
| Remote Config (if approved) | 2–4 | 1–2 | 3–6 | DR-P5 |
| GPGS (sign-in, achievements, leaderboards) | 5–9 | 3–5 | 8–14 | Play Console GPGS setup, tester propagation |
| Player data layer | 3–6 | 1–2 | 4–8 | DR-P7 for cloud (+3–6 if Saved Games) |
| AdMob + UMP consent | 6–11 | 4–7 | 10–18 | AdMob account/app review, policy review of placements |
| IAP / Billing | 6–11 | 4–7 | 10–18 | Play Console products, payment profile, a signed build on a test track |
| **Sum** | **26–49** | **15–27** | **41–76** | Add contingency for dependency conflicts; the first Android build with all four SDKs is the highest-risk integration point |

Estimates are order-of-magnitude, from similar integrations; not measured on this project. No release date is implied.

## 9. Out of scope for this plan
Ad mediation networks other than AdMob, push notifications, server-side backend, accounts system other than Play Games, attribution SDKs, social features, A/B-testing platforms beyond Remote Config (unless approved).
