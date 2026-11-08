# M6 — Monetization: AdMob + IAP

**Status: NOT STARTED** · Effort: 19–35 days (impl 13–24, test 6–11) + external review waiting · Tags: `[Editor]` `[Device]` `[Approval]` `[Console]` `[Legal]`
Detail: `SERVICE_INTEGRATION_PLAN.md` §5–§6. References: `RELEASE_GAP_ASSESSMENT.md` §9 (monetization undecided there; now Ads + IAP confirmed).

## 1. Objective
Policy-compliant AdMob ads and Google Play purchases that never block or freeze gameplay and handle every failure state.

## 2. Scope
**AdMob**: account and app registration; ad unit setup; banner/interstitial/rewarded **assessment** and approved placements (DR-P3); cooldowns and frequency caps; consent (UMP) and privacy; test ads; failure/no-fill handling; app pause/resume around ads.
**Unity IAP / Google Play Billing**: product catalog (DR-P4); purchase lifecycle (success, cancellation, pending, failure); entitlement grants; duplicate prevention; restore / re-query owned purchases; refund and revocation handling.
**Interplay**: remove-ads entitlement (if that is a product), rewarded-reward economy (if rewarded ads approved), analytics events for ad/IAP funnel (M5).

## 3. Out of scope
Ad mediation networks beyond AdMob; subscriptions (unless catalog says so); server-side validation backend (unless approved as an added service); battle pass / live ops; PC/iOS stores.

## 4. Prerequisites
M5 exit gate (consent foundation, Firebase/Crashlytics for observability). DR-P2 (package ID), DR-P3, DR-P4, DR-P13, DR-P15, DR-P16 decided. A signed build uploadable to a Play internal-testing track (Billing requires a Play-distributed build). Owner controls AdMob and Play Console accounts.

## 5. Tasks
- [ ] `[Approval]` Verify current SDK/package versions and policies; record choices.
- [ ] `[Console]` Owner: AdMob app + ad units (test ad units first); Play Console in-app products, payment profile, licence testers.
- [ ] `[Approval]` Install Mobile Ads plugin and `com.unity.purchasing` (package change); resolve Gradle dependencies; device build check with manifest/size diff.
- [ ] Design and approve placement matrix: where/when each format may appear (never mid-round, never over controls); cooldown and cap parameters (Remote Config only if M5 approved it).
- [ ] Consent (UMP) integration and audience handling per DR-P13.
- [ ] Ads façade: preload, show, close, error, no-fill, timeouts; marshal callbacks to the main thread; restore audio/timeScale after ads.
- [ ] Rewarded flow (only if approved): idempotent reward grant.
- [ ] IAP façade: store connect, product fetch, purchase, pending, cancel, failure; single entitlement grant function; transaction-ID duplicate guard; persisted entitlements; consume/confirm only after grant is stored.
- [ ] Restore / re-query owned purchases on start/resume; refund/revocation handling per policy decision.
- [ ] Store UI hooks (M4 placeholders) wired; price text from store (never hard-coded).
- [ ] `[Device]` Tests with test ads and licence-tester purchases (see §9); offline; consent variants; interrupted purchase; reinstall restore.
- [ ] Policy self-review checklist (accidental clicks, placement, child-directed settings per DR-P13).

## 6. Dependencies
M5; Play Console and AdMob accounts; a Play testing track build; DR-P3/P4/P13.

## 7. Risks
Ad/IAP library dependency conflicts with Firebase/GPGS; policy rejection (placements, audience); revenue-affecting bugs (double grant, lost purchase); Billing cannot be tested without Play distribution; ad SDK crashes (monitor via Crashlytics); real ad unit IDs used in testing (account risk).

## 8. Required approvals
Package installs; placements and caps; product catalog and prices; consent/audience handling; refund-behavior policy; whether server-side validation is required.

## 9. Validation procedure
Test ads only until release; every format: load, show, close, click, no-fill, offline, backgrounded during ad. Purchases (licence tester): success, cancel, pending, failure, already-owned, kill app mid-purchase, offline start, reinstall + restore, refund/revocation re-query. `logcat` + Crashlytics review.

## 10. Acceptance criteria
- Ads appear only at approved placements; caps/cooldowns obeyed; consent honored; gameplay unaffected by ad failure.
- Each approved product grants its entitlement exactly once; cancel/failure leaves state unchanged; restore works after reinstall; revocation handled as specified.
- No ANR/crash attributable to ads or billing in the test matrix.
- Policy self-review completed and recorded.

## 11. Evidence to record
Versions, console configuration (no secrets), device test table, `logcat` excerpts, policy checklist, entitlement state transitions, screenshots of test ads/purchases.

## 12. Exit gate
Acceptance met with device evidence; owner approves placements and catalog for release. Next: M7 (final clearance) / M8.

## Completion summary
_(empty)_
