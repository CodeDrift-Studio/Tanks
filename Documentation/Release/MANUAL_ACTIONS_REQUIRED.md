# Manual Actions Required — Tank Mayhem v1.0.0 Internal Testing

Ordered by priority. Claude cannot do these: they need the Unity Editor, console access, the keystore or a device.

## A. Unity Editor
1. **The `Release-Android™` build profile already exists and is active** (Editor-verified 2026-11-12: IL2CPP, Development off, Build App Bundle on, Desert-only scene list from the global list, package `com.cds.tankmayhem`). Leave `Dev-Android™` unchanged. Still to do in the Editor, by you:
   - **Version**: the profile currently has name `1.0.4` / code `4`; the Dev profile has `1.0.7` / `7`; the global settings `1.0.1` / `1`. Choose the production version name and a versionCode **higher than any code ever uploaded** (check the Play Console, including Dev uploads). **OWNER ACTION REQUIRED.** Not changed by Claude.
   - **Architectures**: the Editor reports `ARM64 + X86` (flag `6`). Decide whether to set **ARM64 only**. **OWNER DECISION.** Not changed.
   - **Signing**: the custom upload keystore is already configured. Enter the passwords in the Editor at build time. **Do not save them in the project.**
   - **Debug symbols**: the profile has a symbol level/format set; confirm in the built AAB / Crashlytics that native symbols are available. UNVERIFIED.
2. Android TV compatibility is already **off** and the Release profile is already landscape-only (Landscape Left/Right), both Editor-verified. The global Player Settings still allow portrait, so always build with `Release-Android™`.
3. Build. Record the AAB path and size. (Not done: no AAB exists yet.)
4. **Check the built AAB** (`bundletool dump manifest`, or Android Studio ▸ Analyze APK): confirm `INTERNET` permission (`ForceInternetPermission` is off, so it must come from the Firebase manifest merge), `targetSdk`, ABIs (`arm64-v8a`, and whether `x86` is present), versionCode and package. Confirm no debug console appears on device. The Desert scene no longer contains the console, so the `[ReleaseBuild] Removing…` log line is **not expected** to appear and is not evidence either way.
5. **Git hygiene before any commit**: `ProjectSettings/ProjectSettings.asset` and the build profile assets contain an absolute local keystore path and alias. Decide how to commit them (blank the path in the committed copy, or keep those files local). Do not commit the keystore. `Assets/google-services.json` is untracked: committing it is your decision, since it holds the project's API key, which is not a secret but is restricted to the app.

## B. Firebase Console (project `tank-mayhem-ff62d`)
1. Analytics ▸ DebugView: run a debug build with `adb shell setprop debug.firebase.analytics.app com.cds.tankmayhem`. Confirm `game_session_start`, `match_start`, `tank_selected`, `round_complete`, `match_complete` and `powerup_collected` appear.
2. Crashlytics: confirm the app appears. A test crash needs your approval.
3. Remote Config: check the published `match_rounds_to_win` (code default is now 3) and `powerup_respawn_seconds`. Do not publish a value of 5 for Desert. Note: no gameplay code reads either key today, so a published value has no effect on the game.
4. Authentication: Anonymous provider enabled (owner-reported working, not verified by Claude). Count the users after reinstalling.
5. Firestore: only if you want the diagnostic. Create the database, set rules allowing `diagnostics/{uid}` only for the signed-in owner, then run the **Development** build diagnostic. The release build does not contain it.
6. App Check: leave enforcement OFF for v1.0.

## C. Google Play Console
Follow `PLAY_CONSOLE_CHECKLIST.md`.

## D. Android device
Run `ANDROID_RELEASE_TEST_PLAN.md`. A second device and Android version is still pending from the earlier plan. Capture `adb logcat` for the startup frame skips.

## E. Decisions only the owner can make
- Version number and versionCode policy.
- ARM64-only versus ARM64 + X86.
- Whether to keep `StreamingAssets/google-services-desktop.json` (it ships in the Android build and is not needed on Android).
- Analytics and Crashlytics default-on with no consent (DR-P13).
- Whether to remove the unused App Check libraries (needs a dependency change approval). Firestore libraries are also linked but only an instance is acquired; no code writes automatically.
