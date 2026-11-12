# Play Console Checklist — Internal Testing, v1.0.0

Nothing here has been done or verified by Claude. Current Play requirements can change, so confirm each against the Play Console and Google's help pages. The split between "before internal upload" and "before public release" is from general knowledge and **UNVERIFIED**.

## 1. Preparation
- [ ] Build the signed AAB with the `Release-Android™` profile (see `MANUAL_ACTIONS_REQUIRED.md` §A). No AAB has been built yet. The project currently holds three different version pairs (Release profile 1.0.4 / 4, Dev profile 1.0.7 / 7, global 1.0.1 / 1): the owner must pick the version and a code **higher than anything uploaded before** (check the Play Console history). OWNER ACTION REQUIRED.
- [ ] Confirm the target API meets Play's current minimum (the project targets 36).
- [ ] Keystore: use the upload key. Keep a backup outside the repo.
- [ ] Decide the testers: individual emails or a Google Group.

## 2. Create the app
- [ ] Play Console ▸ Create app: name "Tank Mayhem", default language, Game, Free.
- [ ] Package `com.cds.tankmayhem` is fixed by the first upload.
- [ ] Opt in to Play App Signing (default).

## 3. App content (the declarations in the dashboard)
Complete what the dashboard marks as required. Typical items:
- [ ] Privacy policy URL (needed since the app uses Firebase)
- [ ] Data safety form (Analytics, Crashlytics, Auth identifiers, device or app IDs; fill from the real SDK behaviour)
- [ ] Content rating questionnaire
- [ ] Target audience and age group (DR-P13). Choosing under 13 triggers extra Families requirements.
- [ ] Ads: **No ads** in v1.0
- [ ] App access: no login needed
- [ ] Other forms the dashboard lists (government app, financial features, health: expected No)

## 3b. Store listing
Internal testing normally does not require a complete public listing, but the dashboard may ask for basics. Prepare:
- [ ] Short and full description
- [ ] 512×512 icon
- [ ] 1024×500 feature graphic
- [ ] Phone screenshots (landscape)

## 4. Upload
- [ ] Testing ▸ Internal testing ▸ Create release, then upload the `.aab`
- [ ] Release name = the version name and code actually chosen for the uploaded AAB (not `1.0.0 (1)`), with the notes from `RELEASE_NOTES_v1.0.0.md` (edit the version text there to match)
- [ ] Review warnings (64-bit, target SDK, permissions, deobfuscation or native symbols)
- [ ] Save, review, roll out to internal testing

## 5. Testers
- [ ] Add the tester list, copy the opt-in link and send it
- [ ] Testers accept the invite, install from the Play Store, and play

## 6. Monitoring
- [ ] Play Console ▸ Android vitals (crashes and ANRs)
- [ ] Firebase Crashlytics and Analytics DebugView
- [ ] Collect feedback in one place

## 7. Update test
- [ ] Upload a build with a higher versionCode than the first and confirm that testers get the update, with the player ID preserved

## Production release
This checklist covers Internal Testing. A production release is the intended next stage; the declarations in section 3 (privacy policy URL, Data safety, content rating, target audience/age, ads = none) are expected to be required there too, and a full store listing (section 3b) will be. Production-track requirements are **UNVERIFIED**: confirm them in the Play Console. Nothing in this file has been completed or verified.

## Done when
Testers installed through Play, the run in `ANDROID_RELEASE_TEST_PLAN.md` is recorded with evidence, and no critical crash is open.
