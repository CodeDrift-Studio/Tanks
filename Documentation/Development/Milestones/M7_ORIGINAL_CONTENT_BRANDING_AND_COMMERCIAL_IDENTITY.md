# M7 — Original Content, Branding and Commercial Identity

**Status: NOT STARTED** (licensing evidence collection is planned to start in M0/M1; this milestone is the final clearance and production gate) · Effort: 16–78 days · Tags: `[Legal]` `[Approval]` `[Editor]` `[Content]`
References: KI-007, KI-004, KI-009; `RELEASE_GAP_ASSESSMENT.md` §9, LEG-01, ID-01, BRAND-01, STORE-01. Claude records facts; the owner/legal decides. **No legal conclusions by Claude.**

## 1. Objective
Every shipped asset is original or has a recorded, owner-accepted licence; the game has a final name and a coherent commercial identity.

## 2. Scope
- Sample asset licensing review (Asset Store EULA / Unity sample terms / `Documentation.txt`) — **early investigation tasks live in M0/M1**; final decision and execution here.
- Third-party asset and font licensing (OFL fonts, CC0 Blue Noise, audio, models, textures, packages).
- Replacement of uncleared assets (models, textures, audio/music, UI, fonts, VFX as required).
- Final game name (DR-P1); visual identity; logo, app icon, splash; menu and **Desert** arena presentation (the only first-release arena, DR-C14); audio and music.
- Store screenshots, feature graphic, promo materials; store listing copy.
- In-game credits/notices required by licences.

## 3. Out of scope
New gameplay mechanics, additional arenas/tanks (unless approved; Jungle and Moon are deferred to a future update, DR-C14), engine/package upgrades, legal advice.

## 4. Prerequisites
Provenance table started (M0/M1). Owner/legal review capacity. M4 UI done (to brand). DR-P11 outcome, DR-P17 budget/owner for replacement work.

## 5. Tasks
- [ ] (Early, M0/M1) Provenance inventory: every asset group (models 28 FBX, 42 textures, audio, fonts, shaders/materials, packages) with source, licence document, redistribution notes. Facts only.
- [ ] `[Legal][Approval]` Owner/legal reviews EULA, third-party notice, Unity terms; records outcome per asset group (cleared / needs permission / replace).
- [ ] Replacement plan and budget (DR-P17): scope, sourcing (create/commission/purchase with clear licences), schedule.
- [ ] Produce/replace assets in priority order (tank models and the Desert arena visible in screenshots first).
- [ ] Final name, package/product naming consistency check (DR-P1, with DR-P2 already fixed in M5).
- [ ] Logo, icon (adaptive icon), splash; menu and arena presentation pass.
- [ ] Audio and music cleared/replaced; mix check.
- [ ] Store assets: screenshots from final builds, feature graphic, short/long description, category, tags; localization if scoped.
- [ ] Credits/licence notices screen and file.
- [ ] Final scan: all assets in builds are in the provenance table with cleared status; zero unresolved. Scope: assets that ship in the Desert-only release build. Jungle/Moon-only assets are not shipped in the first release, but remain in the repository; record them in the provenance table as DEFERRED and re-clear them before the content update (not a first-release blocker, DR-C14). Which assets are exclusive to Jungle/Moon is UNKNOWN (not analysed).

## 6. Dependencies
Owner/legal; art/audio resources; M4 (UI), M5/M6 (store listing must reflect ads/IAP/data use); DR-P1, DR-P11, DR-P17.

## 7. Risks
Largest cost uncertainty in the project (replacement 0–40 days or more); late discovery of uncleared assets; branding changes force UI rework; naming/trademark conflicts (not assessed here; owner/legal).

## 8. Required approvals
Licensing decisions (owner/legal), final name, brand direction, asset deletion/replacement (CLAUDE.md §K), commissioning budget.

## 9. Validation procedure
Provenance table vs actual build contents (asset reports); visual/audio review on device; store-asset checklist; licence notice check.

## 10. Acceptance criteria
- Provenance table complete; every shipped asset cleared or replaced; no unresolved licensing item.
- Final name/identity applied in-game, icon, splash, store assets.
- Required notices/credits present.

## 11. Evidence to record
Provenance table, owner/legal decision records (dates, who), before/after asset lists, build asset report, store asset files list.

## 12. Exit gate
Owner confirms zero unresolved licensing items and approves branding. Unresolved licensing = **release blocker**. Next: M8.

## Completion summary
_(empty)_
