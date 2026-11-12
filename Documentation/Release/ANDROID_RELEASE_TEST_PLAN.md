# Android Release Test Plan — v1.0.0

Record the device model, Android version, build or versionCode and the date for every run. A row without evidence stays NOT RUN. All rows are **NOT RUN** today.

| # | Case | Steps | Expected | Result |
|---|---|---|---|---|
| 1 | Clean install | Uninstall, install the build from Play Internal Testing | Installs, icon and name correct | NOT RUN |
| 2 | Launch | Start the app, time it | Menu appears, no crash. Note the startup stutter | NOT RUN |
| 3 | Main menu | Navigate the menu | Single-player vs AI, Desert only | NOT RUN |
| 4 | Tank selection | Pick each of ATV, Shark, Medium, Heavy | Each spawns and plays | NOT RUN |
| 5 | Touch controls | Joystick moves camera-relative; clockwise/counterclockwise buttons rotate in place; fire independent; multi-touch | Move, rotate and shoot together; no stuck input | NOT RUN |
| 5b | Pause / resume controls | Pause mid-match while holding stick/fire, then resume | Gameplay controls hidden while paused, pause button stays usable, controls restored on resume, nothing stuck | NOT RUN |
| 6 | AI combat | Play a round | Bot moves, aims, fires | NOT RUN |
| 7 | Power-ups | Collect Speed, DamageReduction, ShootingBonus, Healing, Invincibility, DamageMultiplier | Effect applies and ends | NOT RUN |
| 8 | Round transition | Finish a round | Reset, scores right | NOT RUN |
| 9 | Match completion | Play to 3 round wins | Winner shown, back to menu | NOT RUN |
| 10 | Restart | Play two matches in a row | No leftover state | NOT RUN |
| 11 | Background / resume | Home button mid-match while holding stick/fire, return after 30 s | Game recovers, no stuck movement/rotation/fire (no auto-pause on background is a known gap, so note whether the match kept running) | NOT RUN |
| 12 | Force-close / relaunch | Swipe the app away, relaunch | Clean start, same Firebase UID | NOT RUN |
| 13 | Offline | Airplane mode, launch and play | Game fully playable | NOT RUN |
| 14 | Online after offline | Re-enable network | Auth signs in without a restart | NOT RUN |
| 15 | Firebase init | Dev build logcat: `adb logcat -s Unity \| findstr FIREBASE` | Init succeeded | NOT RUN |
| 16 | Anonymous Auth persistence | Note the UID, relaunch, reinstall | Same UID after relaunch. A new UID after reinstall is expected | NOT RUN |
| 17 | Analytics | DebugView during a full match | Six events with the right params | NOT RUN |
| 18 | Crashlytics | Console after a session (no forced crash without approval) | App registered | NOT RUN |
| 19 | Firestore diagnostic | Only if approved: Development build. The release build has none | Round-trip SUCCESS and document deleted | NOT RUN |
| 20 | Debug console absent | Release build (`Release-Android™`) on device. Note: the console was removed from the Desert scene itself (static check 2026-11-12); the build-time stripper is only a safety net and has never been shown to run | No console button or overlay | NOT RUN |
| 21 | Performance | 10-minute session, Android Studio profiler or `adb shell dumpsys gfxinfo` | Record fps, jank, heat | NOT RUN |
| 22 | Rotation / back button | Rotate (should stay landscape), press Back in menu and mid-match | Note behaviour (known gaps; Back handling was not verified) | NOT RUN |
| 23 | Update via Play | Install a build, then upload one with a higher versionCode | Update in place | NOT RUN |
| 25 | Package contents | Inspect the built AAB manifest | `INTERNET` present, package `com.cds.tankmayhem`, targetSdk, ABIs (note whether `x86` is included), versionCode | NOT RUN |
| 24 | Second device | Different model and Android version | Cases 1–12 | NOT RUN |
