# M7 — Commercial Release Preparation

**Status: NOT STARTED** · Complexity: L · Tags: `[Editor]` `[Build]` `[Device]` `[Approval]` `[Legal]`

## 1. Objective
A release candidate with cleared licensing, branding, store materials and correct build configuration.

## 2. Current repository state (STATIC)
Placeholder company/product/bundle id (KI-004); default icon (UNVERIFIED); Asset Store EULA governs the sample with separate third-party licences (KI-007); pre-release/AI packages (KI-009); Unity Analytics module present by default; no keystore configuration; business model, final name and pricing undecided.

## 3. Scope
Asset licensing review, branding, store assets, versioning, build configurations, privacy review, release candidate, distribution preparation.

## 4. Prerequisites
M6 accepted; owner decisions: final name, pricing, business model, target stores (Steam/itch/Google Play/etc. — PENDING).

## 5. Tasks
- [ ] `[Legal][Approval]` Provenance + licence table for **all** assets and packages (Asset Store EULA terms, Unity Companion License applicability, OFL/UFL/CC0 notices). Claude records facts; the owner/legal decides. No legal conclusions by Claude.
- [ ] Ship required licence notices/credits in-game and in store listings.
- [ ] Mark any unresolved licensing item as **release blocker**; replace content (M5) or obtain permission.
- [ ] Branding: name, icon, splash, store art, screenshots (owner/art).
- [ ] Versioning scheme (e.g., semantic version + Android `versionCode` increments).
- [ ] Configure release identity: company, product, package id; Android keystore handled by owner (never in repo); ARM64; target SDK per store requirements (verify current requirements — UNKNOWN today).
- [ ] Windows release build config (architecture, compression, installer/zip).
- [ ] Privacy review: Unity Analytics/AI packages/permissions; decide removal of unused packages (ADR-007); privacy policy if required by store.
- [ ] `[Device]` Release candidate build test on Windows + Android devices; full checklist run.
- [ ] Distribution prep: store listings, age rating, build upload.

## 6. Technical implementation notes
Use Release (non-development) builds for final QA. Strip debug/log noise. Verify no editor-only code leaks (`ProjectWideActionsSetup` is editor-only). Keep secrets out of the repo.

## 7. Files/systems likely affected
`ProjectSettings/*`, `Packages/manifest.json` (approved removals only), splash/icon assets, docs (credits/licences), build scripts (if added).

## 8. Risks
Licensing blockers; store policy changes; rejection for privacy/permissions; keystore loss.

## 9. Validation steps
Clean-machine install tests; Play Store pre-launch report (if used); checklist section 1 (Build/Release) fully executed.

## 10. Acceptance criteria
- Licensing review complete; no unresolved blocker.
- Release identity/branding applied; non-placeholder IDs.
- RC builds for Windows and Android pass the full checklist.
- Privacy review done; store assets ready.

## 11. Definition of Done
Criteria met with evidence and owner sign-off on legal/business items; docs updated.

## 12. Completion summary
_(empty)_
