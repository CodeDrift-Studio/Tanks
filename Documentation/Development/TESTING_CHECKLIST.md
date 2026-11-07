# TESTING CHECKLIST

States: **NOT TESTED** · **PASS** · **FAIL**. A PASS requires evidence: date, build/commit id, tester, device/Editor, note. Initial state of every row: NOT TESTED (nothing has been executed as of 2026-11-06).
Record a failure as an issue in `KNOWN_ISSUES.md` and reference its ID.

Columns: ID | Steps | Expected | State | Evidence / Issue

## 1. Build and Release
| ID | Steps | Expected | State | Evidence |
|---|---|---|---|---|
| B-01 | Open project in Unity 6000.3.6f1; check Console | No compile errors | NOT TESTED | |
| B-02 | Build Settings lists valid scenes; index 0 correct | No missing scene | NOT TESTED | KI-001 |
| B-03 | Build Windows x64 (development) | Build succeeds | NOT TESTED | |
| B-04 | Build Android APK/AAB (development, IL2CPP, ARM64) | Build succeeds | NOT TESTED | |
| B-05 | Fresh-clone check: clone repo, open, no missing refs | Opens cleanly | NOT TESTED | needs git (KI-002) |
| B-06 | Player log of Windows build after a full match | No errors/NREs | NOT TESTED | KI-006 |
| B-07 | Package id/version/icon set (release config) | Non-placeholder | NOT TESTED | KI-004 |

## 2. Windows PC
| ID | Steps | Expected | State |
|---|---|---|---|
| W-01 | Launch build | Menu appears, no errors | NOT TESTED |
| W-02 | P1 WASD+Space vs CPU | Moves, turns, fires | NOT TESTED |
| W-03 | P1 WASD + P2 arrows/Enter (2 humans) | Independent control, no cross-trigger | NOT TESTED |
| W-04 | Gamepad as P1 | Moves/fires (KI-010) | NOT TESTED |
| W-05 | 1920×1080, 2560×1080 (21:9), 1280×800 (16:10), 1024×768 (4:3), windowed/fullscreen | UI readable, camera frames all tanks | NOT TESTED |
| W-06 | Esc pause/resume; quit button | Pauses, resumes, quit works in build | NOT TESTED |

## 3. Android
| ID | Steps | Expected | State |
|---|---|---|---|
| A-01 | Install and launch on a real device | Menu, no crash | NOT TESTED |
| A-02 | Select 1 human + CPU | P2 button hidden; match starts | NOT TESTED |
| A-03 | Touch joystick + fire | Controls tank, charge/fire works (KI-008) | NOT TESTED |
| A-04 | Rotate/orientation, notch/safe-area | UI not clipped | NOT TESTED |
| A-05 | Background/resume app mid-match | Recovers | NOT TESTED |
| A-06 | `adb logcat` during a full match | No exceptions | NOT TESTED |
| A-07 | Low/mid-tier device 10-min session | Stable frame rate, no thermal crash | NOT TESTED |

## 4. Gameplay
| ID | Steps | Expected | State |
|---|---|---|---|
| G-01 | Movement forward/back/turn | Responsive, no jitter | NOT TESTED |
| G-02 | Tap vs full-charge shot | Range scales with charge; cooldown enforced | NOT TESTED |
| G-03 | Shell hit | Damage by distance, knockback, explosion FX/audio | NOT TESTED |
| G-04 | Tank dies | Explosion, removed from play, camera retargets | NOT TESTED |
| G-05 | Last tank standing | Round winner message, score +1 | NOT TESTED |
| G-06 | Simultaneous death | "DRAW!" | NOT TESTED |
| G-07 | First to 5 round wins | "WINS THE GAME", scene restarts to menu | NOT TESTED |
| G-08 | Each arena (Desert, Jungle, Moon) | Loads, spawns correct, no missing refs/console errors | NOT TESTED |
| G-09 | Camera with 2/3/4 tanks and edge positions | All active tanks visible | NOT TESTED |
| G-10 | Pause → "select tank" | Returns to menu, timeScale = 1 (KI-P06) | NOT TESTED |

## 5. AI
| ID | Steps | Expected | State |
|---|---|---|---|
| AI-01 | 1 human vs 1 CPU, 10 rounds | CPU approaches, shoots, flees | NOT TESTED |
| AI-02 | 4 CPU soak (auto), 30+ rounds per arena | No console errors/NREs (KI-P01) | NOT TESTED |
| AI-03 | Target dies mid-chase | CPU retargets/idles without exception | NOT TESTED |
| AI-04 | CPU stuck on obstacle | Recovers within ~2–4 s | NOT TESTED |
| AI-05 | NavMesh present in each arena | Paths found | NOT TESTED |

## 6. Multiplayer (local)
| ID | Steps | Expected | State |
|---|---|---|---|
| M-01 | 2 humans + 2 CPU on PC | 4 tanks, correct colours/controls | NOT TESTED |
| M-02 | Assigning P1/P2 across slots | Duplicate assignment demotes other slot to CPU | NOT TESTED |
| M-03 | Start requires ≥ 2 tanks | Button disabled/“2 Tanks required” | NOT TESTED |
| M-04 | Android shared screen | **Not supported — do not test unless approved** | N/A |

## 7. Power-ups
| ID | Steps | Expected | State |
|---|---|---|---|
| P-01 … P-06 | Collect Speed / DamageReduction / ShootingBonus / Healing / Invincibility / DamageMultiplier | Effect applies, HUD shows, effect ends | NOT TESTED |
| P-07 | Die or end round while a timed effect is active | Stats back to baseline next round (KI-P02) | NOT TESTED |
| P-08 | Pickup while another effect active | Not consumed / per design | NOT TESTED |
| P-09 | Respawn after 20 s | New random pickup spawns | NOT TESTED |

## 8. UI
| ID | Steps | Expected | State |
|---|---|---|---|
| U-01 | Menu slot add/remove/control buttons | State consistent, preview colour matches slot | NOT TESTED |
| U-02 | Round/Win text, scores | Readable, correct | NOT TESTED |
| U-03 | Health + aim sliders per tank | Update correctly | NOT TESTED |
| U-04 | Controls screen from pause | Opens/back works | NOT TESTED |
| U-05 | Settings menu (after M3/M4) | Persists across restart | NOT TESTED |

## 9. Performance
| ID | Steps | Expected | State |
|---|---|---|---|
| PF-01 | Windows: profile 4-tank match | Budget PENDING (ADR-009) | NOT TESTED |
| PF-02 | Android: profile 2-tank match | Budget PENDING | NOT TESTED |
| PF-03 | GC allocations/frame in match | Near zero in steady state | NOT TESTED |
| PF-04 | Memory after 10 consecutive matches | No growth trend | NOT TESTED |
| PF-05 | Build size | Recorded | NOT TESTED |
