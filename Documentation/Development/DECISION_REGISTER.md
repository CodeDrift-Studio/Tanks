# DECISION REGISTER (Product Decision Checkpoint)

Created 2026-11-08. One page for **product/business decisions**. Technical architecture decisions live in `TECHNICAL_DECISIONS.md` (ADR log) and point here. Do not invent answers: a PENDING row stays PENDING until the owner states it in writing, then it moves to CONFIRMED with the date and source.

Statuses: **CONFIRMED** (owner) · **PENDING** (owner decision needed) · **ASSUMED-PROPOSAL** (suggested default, not a decision).
"Needed by" = the latest point at which the decision blocks work (milestone gate).

## A. Confirmed decisions (owner, recorded 2026-11-08)

| ID | Decision | Source | Consequences |
|---|---|---|---|
| DR-C1 | Android is the only first-release platform | Owner instruction 2026-11-08 | Windows/PC and iOS are out of first-release scope; code and assets are kept |
| DR-C2 | Initial gameplay is single-player human vs AI | Owner | Touch controls mandatory; P2/multi-human paths deferred |
| DR-C3 | Touch controls are mandatory | Owner | M1/M4 |
| DR-C4 | Online multiplayer excluded | Owner | — |
| DR-C5 | Shared-screen Android multiplayer excluded from current release scope | Owner | May be revisited later in writing |
| DR-C6 | Monetization: Free-to-Play with Ads + IAP | Owner | Drives M6, privacy/consent, age-group decision, store declarations |
| DR-C7 | Google AdMob is mandatory | Owner | M6; formats/placements still PENDING |
| DR-C8 | Unity IAP / Google Play Billing is mandatory | Owner | M6; catalog/pricing PENDING |
| DR-C9 | Firebase services are mandatory | Owner | M5; exact services PENDING (do not assume every SDK feature) |
| DR-C10 | Google Play Games Services (GPGS) is mandatory | Owner | M5; feature scope PENDING |
| DR-C11 | ~~Existing Desert, Jungle and Moon arenas are the proposed first-release content~~ **SUPERSEDED by DR-C14 (2026-11-08)** | Owner ("proposed"), superseded | See DR-C14 |
| DR-C12 | Git roles: `main` stable/release-approved, `dev` active development/integration, `xyz-backup` archived/deferred material, feature branches from `dev` merged through review | Owner (consistent with `GIT_WORKFLOW.md`, 2026-11-07) | See `CLAUDE.md` §N |
| DR-C14 | **The first commercial Android release launches with ONE arena only: Desert.** Jungle and Moon are deferred to a future content update (deferred content, **not release blockers**). Arena architecture/content should stay extensible for future arenas. | Owner instruction 2026-11-08 | No arena-selection requirement for first release; arena validation, presentation, store assets and performance work scoped to Desert; Jungle/Moon scenes and assets stay in the repo (nothing deleted), excluded from the release build; closes DR-P12 |
| DR-C13 | Release licensing: unresolved licensing is a release blocker; no legal conclusions are made by Claude | Existing (KI-007, CLAUDE.md §K) | Investigation starts early (M0/M1), clearance gate in M7 |
| DR-C15 | **Initial four-tank balance pass accepted as the gameplay baseline** (ATV / Shark / Medium / Heavy = menu slots 1–4). Approved stats: ATV health 150, speed 14, turn 180, cooldown 0.45, max damage 60, radius 3, knockback 10; Shark 225 / 12 / 180 / 1.0 / 60 / 6 / 40; Medium 200 / 10 / 150 / 0.85 / 80 / 3 / 15; Heavy 260 / 7 / 120 / 1.25 / 80 / 4 / 25. Min/max shot force (15/30) and charge time (0.75 s) unchanged. | Owner 2026-11-08 (audit and proposal by Claude, static; owner reports all tanks perform much better) | Values live in the four variant prefabs under `Assets/_Tanks/Tutorial_Demo/Demo_Prefabs/Demo_Tanks/`. **Statistically balanced win rates across the six matchups are NOT established** (no playtest data recorded); win-rate validation stays an open M2 task and is frozen until DR-C16 lifts |
| DR-C16 | **Temporary gameplay freeze** from 2026-11-08 until the additional-device Android testing report arrives and the owner lifts it in writing. Until then: no further tank stat adjustments; no gameplay-mechanic changes; no refactoring of stable gameplay systems; no new gameplay features; already verified gameplay fixes (GP-01…GP-04, GP-08, GP-10 guards) are not reopened without a reproducible issue. If a real critical defect is found: document it in `KNOWN_ISSUES.md`, assess severity, and obtain owner approval before changing the frozen baseline. **This is a gameplay freeze, not a project freeze** (services, docs, tooling, non-gameplay work continue) | Owner 2026-11-08 | Purpose: avoid gameplay churn while Android compatibility is being tested. Android input guards (AND-01) are **exempt** from the freeze (DR-C19), which does not authorize unrelated gameplay changes |
| DR-C17 | **Next development focus: Firebase and Google Play Games Services integration (M5)** may proceed while additional-device Android testing continues. AdMob and Unity IAP stay in the later monetization phase (M6). | Owner 2026-11-08 | **Not started.** Exact Firebase/GPGS feature scope stays PENDING (DR-P5/P6/P7); still gated by DR-P2 (package ID), DR-P13, DR-P15 and package-install approval. Ordering versus M2–M4 is resolved by DR-C18 (overlap allowed; M2–M4 are not automatic blockers) |
| DR-C18 | **M5 (Firebase + GPGS) may overlap M2–M4.** M2–M4 stay in the roadmap (NOT STARTED, not dropped) but are **not automatic blockers** for M5. | Owner 2026-11-08 (resolves `NEXT_SESSION.md` conflict 8) | M5's own gates still apply (DR-P2, DR-P5/P6/P7, DR-P13, DR-P15, DR-P16, package-install approval). M6 still follows M5's consent foundation. Other milestone overlaps still need owner approval |
| DR-C19 | **AND-01 Android input guards are exempt from the gameplay freeze (DR-C16).** This does not authorize unrelated gameplay changes. | Owner 2026-11-08 (resolves conflict 9) | The freeze no longer blocks AND-01. The normal per-fix plan/approval step for code changes (M1 §8, CLAUDE.md §K) is unchanged. Any other fix, including one prompted by the multi-device report, is still a freeze question |
| DR-C20 | **Force Text stays enabled; no additional reserialization is to be triggered.** Documented discrepancy (observed read-only 2026-11-08): the working tree has `EditorSettings.asset` `m_SerializationMode: 2`, and the Desert scene, `Demo_Tank.prefab` and `ProjectSettings.asset` are YAML there, while `HEAD` holds them as binary (git shows them as 'Bin'). About 260 modified tracked files are scenes/prefabs/materials/.asset. The reserialized files are **uncommitted**. | Owner 2026-11-08 (resolves conflict 12) | `CLAUDE.md` §G and KI-003 still describe the assets as binary; that wording reflects `HEAD` and is not rewritten here. KI-003 is not moved to VERIFIED FIXED (nothing committed or fully verified). Committing the reserialization stays a separate, reviewed decision (`GIT_WORKFLOW.md`) |
| DR-C21 | **Owner-reported validation stays PARTIAL** until sufficient supporting evidence is recorded: date, build/commit id, device model + Android version, and `logcat` or equivalent. No row moves to PASS on report alone. | Owner 2026-11-08 (resolves conflict 13) | Applies to G-10, P-01…P-07, A-01, A-02, AI-01, AI-07, B-04 and the M0 closure items |
| DR-C22 | **Checklist ID convention:** Google Play Games Services test rows are renamed GP-01…GP-06 -> **PGS-01…PGS-06** (`TESTING_CHECKLIST.md`). Gameplay finding IDs GP-01…GP-10 (assessment/known-issue tracking) are preserved unchanged. | Owner 2026-11-08 (resolves conflict 10) | Removes the ID collision; older text saying 'GP-0x' means a gameplay finding unless it is in a checklist row, where it now reads PGS |

## B. Pending decisions

| ID | Decision needed | Why it matters | Needed by | Related |
|---|---|---|---|---|
| DR-P1 | Final game name | Branding, store listing, package ID | Before M5 console registration (name may be provisional; package ID may not) | M7, KI-004 |
| DR-P2 | Final **package ID** (applicationId) | Firebase, GPGS, AdMob, IAP and Play Console all bind to it; changing later is costly | **Before M5 starts** | KI-004 |
| DR-P3 | Ad formats (banner / interstitial / rewarded), placements, frequency caps, cooldowns | Player experience, policy compliance, M6 scope | Before M6 design | SERVICE_INTEGRATION_PLAN §AdMob |
| DR-P4 | IAP product catalog and pricing (e.g. remove-ads, cosmetic, consumable — none assumed) | M6 scope, entitlements, save data | Before M6 design | §IAP |
| DR-P5 | Exact Firebase service scope (Analytics, Crashlytics are expected candidates; Remote Config pending; others not assumed) | Privacy, SDK size, M5 scope | Before M5 design | §Firebase |
| DR-P6 | GPGS scope: sign-in only vs achievements vs leaderboards; auto vs manual sign-in | M5 scope, content design | Before M5 design | §GPGS |
| DR-P7 | Cloud save (GPGS Saved Games) yes/no | Data schema, conflict handling, privacy | Before M5 data design | §Player data |
| DR-P8 | Minimum Android version and supported device class | API level, performance budget, test matrix | Before M8 budgets (prefer before M1 device test planning) | ADR-009 |
| DR-P9 | Orientation (landscape-only?) and tablet support | UI layout, safe area, store screenshots | Before M4 | — |
| DR-P10 | Performance budgets (FPS target, memory, APK/AAB size, load time) | M8 acceptance | Before M8 (ideally M3) | ADR-009 |
| DR-P11 | Sample-asset licensing outcome (reuse / replace / partially replace) | M7 scope and the largest cost uncertainty | Investigation M0/M1; outcome before M4 art investment | KI-007 |
| ~~DR-P12~~ | ~~Final arena count for release (3 proposed)~~ **RESOLVED 2026-11-08: Desert only (DR-C14).** Remaining open sub-question: when/if Jungle and Moon ship in a content update (not scheduled, not a release blocker) | Content scope | — | DR-C14 |
| DR-P13 | Target audience and age group (including whether children are a target) | AdMob child-directed handling, consent flow, Play Families policy, Data Safety, ad content rating | **Before M5 consent design and M6** | privacy |
| DR-P14 | Google Play release channel / plan (internal → closed → production; countries; pricing model is F2P) | M8 testing plan | Before M8 | — |
| DR-P15 | Developer accounts: who owns Play Console, AdMob, Firebase, GPGS projects | External setup, signing | Before M5 | — |
| DR-P16 | Signing: Play App Signing enrollment, upload key custody | Release | Before M8 | CLAUDE.md §G |
| DR-P17 | Asset replacement budget/owner (art, audio, fonts) if licensing requires replacement | Effort and schedule | After DR-P11 | M7 |
| DR-P18 | AI difficulty tiers and touch aiming aid (if any) | Gameplay design | Before M2 balancing | — |
| DR-P19 | Pre-release/unused package cleanup (ADR-007), Git LFS (M0) | Size, privacy | M0 / before M8 | KI-009 |
| DR-P20 | Localization (languages at launch) | UI text approach | Before M4 | — |
| DR-P21 | Commit grouping and branch/commit plan for the pending work (scripts incl. the `TankAI.cs` KI-P08 + GP-03 split, tank prefabs, reserialized assets, documentation) | Reviewability; `GIT_WORKFLOW.md` | **DEFERRED by owner 2026-11-08** (conflict 11). No staging, commits, push or merge until the owner decides | CLAUDE.md §N |

## C. Assumed proposals (NOT decisions)
- Landscape-only phone-first layout (DR-P9).
- Settings stored locally first; cloud only if DR-P7 = yes.
- No rewarded/interstitial during active rounds (typical policy-safe default; to confirm in DR-P3).

## D. Change log
- 2026-11-08: register created from owner instruction; C1–C12 confirmed, P1–P20 opened.
- 2026-11-08 (later): owner confirmed first-release arena scope = **Desert only** (DR-C14). DR-C11 superseded, DR-P12 resolved. Jungle and Moon recorded as deferred content for a future update, not release blockers.
- 2026-11-08 (baseline freeze): DR-C15 (initial tank balance accepted as baseline), DR-C16 (temporary gameplay freeze), DR-C17 (next focus Firebase + GPGS) recorded from owner instruction. No pending decision was resolved or invented.
- 2026-11-08 (conflict resolution): owner decisions recorded as DR-C18…DR-C22 and DR-P21 (deferred). DR-C16/DR-C17 wording updated to point at them. No other decision changed.
