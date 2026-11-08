# TESTING CHECKLIST — Android-First

Rewritten 2026-11-08 for the Android-only first release (ADR-012). Previous Windows/PC rows are **DEFERRED / out of first-release scope** (listed in §12; not release acceptance). The previous version is in git history (commit `6de4c4a`).

States: **NOT TESTED** · **PARTIAL** (some evidence, not enough for PASS) · **PASS** · **FAIL** · **N/A (deferred)**. A PASS requires evidence: date, build/commit id, tester, **device model + Android version** (or Editor/test runner), note. As of 2026-11-08 **every row is NOT TESTED except the rows marked PARTIAL below (B-04, A-01, A-02, AI-01, AI-07; since the 2026-11-08 baseline update also G-10 and P-01…P-07; A-10 is IN PROGRESS)**, which are owner-reported on a OnePlus 12 in the **Desert** arena (see `DEVELOPMENT_LOG.md` 2026-11-08). Claude has not executed anything on Android or in the Editor. Record failures in `KNOWN_ISSUES.md` and reference the ID.
**Arena scope (DR-C14, 2026-11-08): first release = Desert only.** Arena rows apply to Desert. Jungle and Moon are **DEFERRED and UNTESTED** (neither validated nor known broken; not release blockers); re-open their rows only when a content update is approved.
**M0 closure note (2026-11-08):** the owner reported that all remaining M0 validation tasks and acceptance checks passed, and M0 is recorded COMPLETE (`MILESTONE_STATUS.md`). That report carried **no per-test evidence** (no date/commit/device OS/logcat per row), so rows are **not** upgraded to PASS here: B-01, B-02, B-03, B-05, B-06 stay NOT TESTED and B-04, A-01 stay PARTIAL until the owner supplies the per-row evidence. This is a recording gap, not a failure. M1 is IN PROGRESS; A-02, AI-01, AI-07 stay PARTIAL.
**Evidence rule (DR-C21, 2026-11-08):** owner-reported results stay **PARTIAL** until build/commit id, device model + Android version and `logcat` (or equivalent) are recorded; none moves to PASS on report alone.
**ID note (DR-C22, 2026-11-08):** Google Play Games rows were GP-01…GP-06 and are now **PGS-01…PGS-06**, to avoid a collision with the gameplay finding IDs GP-01…GP-10 (round-reset fixes etc.), which are unchanged.
Primary test platform: **physical Android device(s)** (Editor simulation is not sufficient). Editor and automated tests support but do not replace device tests.
Milestone column = where the row first becomes required.

## 1. Build and Release
| ID | Steps | Expected | M | State | Evidence |
|---|---|---|---|---|---|
| B-01 | Open project in Unity 6000.3.6f1; check Console | No compile errors | M0 | NOT TESTED | |
| B-02 | Build Profiles: Desert at index 0, enabled; Jungle/Moon disabled or absent (later: final flow scenes, Desert only) | No missing scene; no Jungle/Moon in release build | M0 | NOT TESTED | KI-001. STATIC file read 2026-11-08 matches (latest read: only Desert listed, enabled; `Dev-Android` profile uses the global list); Editor check pending. |
| B-03 | Force Text applied; scenes/prefabs start with `%YAML`; no GUID/meta changes | Clean reserialization | M0 | NOT TESTED | KI-003 |
| B-04 | Android development build (IL2CPP, ARM64) succeeds | APK/AAB produced | M0 | PARTIAL | 2026-11-08, owner-reported: Development Build built and installed on OnePlus 12. IL2CPP/ARM64 and commit id not recorded. |
| B-05 | Fresh clone opens with no missing refs | Opens cleanly | M0 | NOT TESTED | |
| B-06 | Android Player Settings recorded (min/target SDK, orientation, arch, graphics API, id, version) | Values documented | M0 | NOT TESTED | KI-A06 |
| B-07 | Package ID/version/icon are non-placeholder | Final values | M7/M8 | NOT TESTED | KI-004 |
| B-08 | Release AAB signed, IL2CPP + ARM64 confirmed, installs from Play internal track | Installs/runs | M8 | NOT TESTED | |
| B-09 | Merged AndroidManifest permissions reviewed after each SDK | Only expected permissions | M5/M6/M8 | NOT TESTED | |

## 2. Android Device (primary)
| ID | Steps | Expected | M | State |
|---|---|---|---|---|
| A-01 | Install and launch on a real device | Menu appears, no crash; startup `logcat` clean | M0 | PARTIAL — 2026-11-08, owner-reported, OnePlus 12: installed, launched, ran smoothly. Startup `logcat` and Android version not recorded. |
| A-02 | Start 1 human vs 1 CPU | Match starts | M1 | PARTIAL — 2026-11-08, owner-reported, OnePlus 12, Desert: match ran with player and AI tank. Commit not recorded. |
| A-03 | Touch joystick + fire (multi-touch) | Tank moves/turns; charge/fire works | M1 | NOT TESTED |
| A-04 | Rotate / orientation (per DR-P9), notch/cutout/gesture bars | UI not clipped | M4 | NOT TESTED |
| A-05 | Background/resume app mid-match; incoming call/notification | Pauses safely, recovers | M3 | NOT TESTED |
| A-06 | Back button on every screen and during match | Defined behavior, no stuck state | M3 | NOT TESTED |
| A-07 | `adb logcat` during a full match | No unexplained exceptions | M1 | NOT TESTED |
| A-08 | Low/mid-tier device 10-min session | Stable FPS, no thermal crash (budget in ADR-009) | M8 | NOT TESTED |
| A-09 | Device matrix: small phone, tall/notched phone, tablet (if in scope) | Layout and perf acceptable | M4/M8 | NOT TESTED |
| A-10 | Additional-device Android testing (beyond OnePlus 12) | Report received; each device recorded with model, Android version, build/commit, result | M1/M8 | **IN PROGRESS — RESULTS PENDING** (2026-11-08; devices and versions not supplied; not complete) |
| A-10 | Kill app and relaunch; reinstall over existing | Settings persist; update works | M3/M8 | NOT TESTED |

## 3. Gameplay
| ID | Steps | Expected | M | State |
|---|---|---|---|---|
| G-01 | Movement forward/back/turn by joystick | Responsive, no jitter | M1 | NOT TESTED |
| G-02 | Tap vs full-charge shot | Range scales with charge; cooldown enforced | M1 | NOT TESTED |
| G-03 | Shell hit | Distance damage, knockback, FX/audio | M1 | NOT TESTED |
| G-04 | Tank dies | Explosion, removed, camera retargets | M1 | NOT TESTED |
| G-05 | Last tank standing | Round winner, score +1 | M1 | NOT TESTED |
| G-06 | Simultaneous death | Draw handled | M2 | NOT TESTED |
| G-07 | First to 5 | Match end state, correct flow after | M1/M3 | NOT TESTED |
| G-08 | Desert arena (Jungle and Moon DEFERRED, untested) | Loads, spawn correct, no missing refs/errors | M2 | NOT TESTED (Desert AI play observed in A-02/AI-01, not a G-08 pass) |
| G-09 | Camera framing with 2 tanks at edges, ultra-wide/tall aspect | Both tanks visible | M2/M4 | NOT TESTED |
| G-10 | Round reset: health, position, power-up state, shells | Clean state each round | M2 | PARTIAL — 2026-11-08, owner-reported: round-reset fixes GP-01…GP-04 verified in play. Build/commit/device OS not recorded. |

## 4. AI
| ID | Steps | Expected | M | State |
|---|---|---|---|---|
| AI-01 | 1 human vs 1 CPU, 10 rounds | CPU approaches, shoots, flees | M1 | PARTIAL — 2026-11-08, owner-reported, OnePlus 12, Desert, dev build with KI-P08 fix: AI moves and shoots (Desert). Round count (10) and flee behavior not recorded; 10 rounds NOT confirmed. |
| AI-02 | Soak: 100+ bot rounds in Desert (Jungle/Moon deferred) | No errors/NREs (KI-P01, KI-P08) | M2 | NOT TESTED |
| AI-03 | Target dies mid-chase | Retarget/idle without exception | M2 | NOT TESTED |
| AI-04 | CPU stuck on obstacle | Recovers in ~2–4 s | M2 | NOT TESTED |
| AI-05 | NavMesh coverage in Desert (Jungle/Moon deferred) | Paths found | M2 | NOT TESTED |
| AI-06 | Difficulty tiers (if approved) | Distinguishable behavior | M2 | NOT TESTED |
| AI-07 | Match with fewer than 4 tanks (default) | No exception from unassigned slots (KI-P08) | M1 | PARTIAL — 2026-11-08, owner-reported, OnePlus 12, Desert, dev build with fix: 1 human + 1 bot, `NullReferenceException` in `TankAI.SeekUpdate()` no longer reproduces. Only the 1v1 case; 3 and 4-tank cases NOT TESTED. Pre-fix failure was observed on the same device. |

## 5. Power-ups (code names)
| ID | Steps | Expected | M | State |
|---|---|---|---|---|
| P-01 | Speed pickup | Applies, HUD, expires | M2 | PARTIAL — 2026-11-08, owner-reported verified (all six power-ups; death/reset case for P-07). Build/commit/device not recorded. |
| P-02 | DamageReduction (shield) | Applies, expires | M2 | PARTIAL — 2026-11-08, owner-reported verified (all six power-ups; death/reset case for P-07). Build/commit/device not recorded. |
| P-03 | ShootingBonus | Applies, expires | M2 | PARTIAL — 2026-11-08, owner-reported verified (all six power-ups; death/reset case for P-07). Build/commit/device not recorded. |
| P-04 | Healing | Heals, capped | M2 | PARTIAL — 2026-11-08, owner-reported verified (all six power-ups; death/reset case for P-07). Build/commit/device not recorded. |
| P-05 | Invincibility | Applies, expires | M2 | PARTIAL — 2026-11-08, owner-reported verified (all six power-ups; death/reset case for P-07). Build/commit/device not recorded. |
| P-06 | DamageMultiplier (special shell) | Next shot boosted, flag cleared | M2 | PARTIAL — 2026-11-08, owner-reported verified (all six power-ups; death/reset case for P-07). Build/commit/device not recorded. |
| P-07 | Die/end round during each timed effect | Base stats and pickup ability restored next round (KI-P02, KI-P12) | M1 | PARTIAL — 2026-11-08, owner-reported verified (all six power-ups; death/reset case for P-07). Build/commit/device not recorded. |
| P-08 | Pickup while another effect active | Per design (currently blocked) | M2 | NOT TESTED |
| P-09 | Respawn after 20 s | New pickup spawns | M2 | NOT TESTED |

## 6. Game flow and settings
| ID | Steps | Expected | M | State |
|---|---|---|---|---|
| F-01 | Cold start -> menu -> setup -> Desert match -> results -> rematch/menu | Full flow, no build-index dependency | M3 | NOT TESTED |
| F-02 | ~~Arena selection picks the chosen scene~~ — **DEFERRED (DR-C14, Desert only)**; instead: match start loads Desert via the explicit scene reference | Desert loads | M3 | NOT TESTED |
| F-03 | Settings persist across kill/relaunch; corrupt data | Persist; safe fallback | M3 | NOT TESTED |
| F-04 | Pause/resume, quit confirmation, `timeScale` restored | No stuck pause | M3 | NOT TESTED |

## 7. UI/UX
| ID | Steps | Expected | M | State |
|---|---|---|---|---|
| U-01 | Main menu / setup / arena select on touch | Usable one-handed; target size per standard | M4 | NOT TESTED |
| U-02 | Round and match results screens | Correct data and actions | M4 | NOT TESTED |
| U-03 | HUD readability on small screens | Readable | M4 | NOT TESTED |
| U-04 | Settings UI | Applies and persists | M4 | NOT TESTED |
| U-05 | First-time instructions | New player completes a match unaided (observed) | M4 | NOT TESTED |
| U-06 | Audio/UI feedback present | Click, pickup, hit cues; volumes work | M4 | NOT TESTED |

## 8. Firebase (M5) — see SERVICE_INTEGRATION_PLAN §1–§3
| ID | Steps | Expected | State |
|---|---|---|---|
| FB-01 | Init with network / offline / consent denied | Game unaffected; consent respected | NOT TESTED |
| FB-02 | Analytics events in DebugView | Approved events/params only; no PII | NOT TESTED |
| FB-03 | Crashlytics forced crash + non-fatal (dev build) | Symbolicated report with custom keys | NOT TESTED |
| FB-04 | Remote Config (only if approved): change, fetch, activate at safe point; invalid value | Applied only at menu; invalid rejected; offline defaults | NOT TESTED |
| FB-05 | Manifest/size diff after SDK | Reviewed | NOT TESTED |

## 9. Google Play Games Services (M5) — §4
| ID | Steps | Expected | State |
|---|---|---|---|
| PGS-01 | Sign-in success / cancel / failure / offline | Optional, non-blocking, guest play | NOT TESTED |
| PGS-02 | Debug and release signing both authenticate | Works | NOT TESTED |
| PGS-03 | Each approved achievement unlocks once | No duplicates | NOT TESTED |
| PGS-04 | Leaderboard submit/read | Correct | NOT TESTED |
| PGS-05 | Account switch / sign-out / Play services missing | No crash | NOT TESTED |
| PGS-06 | Saved Games (only if approved) | Sync/conflict rules hold | NOT TESTED |

## 10. AdMob (M6) — §5
| ID | Steps | Expected | State |
|---|---|---|---|
| AD-01 | Each approved format with **test ads**: load/show/close/click/no-fill/offline | No block or crash | NOT TESTED |
| AD-02 | Cooldown and frequency caps | Obeyed | NOT TESTED |
| AD-03 | Consent granted/denied (UMP) | Respected | NOT TESTED |
| AD-04 | App backgrounded during ad; audio and `timeScale` after close | Restored | NOT TESTED |
| AD-05 | Rewarded grant (if approved) | Exactly once | NOT TESTED |
| AD-06 | Placement policy review | No ads mid-round/over controls | NOT TESTED |

## 11. IAP / Play Billing (M6) — §6
| ID | Steps | Expected | State |
|---|---|---|---|
| IAP-01 | Purchase success (licence tester) | Entitlement granted once | NOT TESTED |
| IAP-02 | Cancel / failure | State unchanged | NOT TESTED |
| IAP-03 | Pending purchase | Granted only on completion | NOT TESTED |
| IAP-04 | Kill app mid-purchase; relaunch | Resolved without duplicate | NOT TESTED |
| IAP-05 | Already owned / double-tap | No double grant | NOT TESTED |
| IAP-06 | Reinstall -> restore / re-query owned | Entitlement restored | NOT TESTED |
| IAP-07 | Refund / revocation re-query | Handled per policy | NOT TESTED |
| IAP-08 | Offline start; store unavailable | Local entitlements work; store disabled gracefully | NOT TESTED |

## 12. Performance, automated tests, compliance
| ID | Steps | Expected | M | State |
|---|---|---|---|---|
| PF-01 | Profile 1v1 match on low/mid device (Unity Profiler on device) | Meets ADR-009 budget (PENDING) | M8 | NOT TESTED |
| PF-02 | GC allocation per frame in match | Near zero steady state | M8 | NOT TESTED |
| PF-03 | Memory after 10 consecutive matches | No growth trend | M8 | NOT TESTED |
| PF-04 | Thermal: 20-min session | No throttle collapse | M8 | NOT TESTED |
| PF-05 | AAB/APK size and load times recorded | Within budget | M8 | NOT TESTED |
| T-01 | EditMode tests (power-up state, scoring, data migration, entitlement logic) | Executed, green | M2/M8 | NOT TESTED |
| T-02 | PlayMode smoke (spawn tanks, run a round) | Executed, green | M2/M8 | NOT TESTED |
| C-01 | Privacy policy, Data Safety, content rating, ads declaration match actual SDK behavior | Consistent | M8 | NOT TESTED |
| C-02 | Provenance table covers all shipped assets; zero unresolved | Complete | M7 | NOT TESTED |
| C-03 | Play internal test install; closed test where required | Passes | M8 | NOT TESTED |

## 13. DEFERRED — out of first-release scope (not acceptance)
Previous rows W-01…W-06 (Windows build, keyboard P1/P2, **PC gamepad W-04/KI-010**, PC resolutions, Esc/quit), M-01…M-03 (2–4 players, P1/P2 assignment, multi-tank start), M-04 (Android shared screen) are N/A for the first release. Retained in git history; reinstate only by written owner approval.
