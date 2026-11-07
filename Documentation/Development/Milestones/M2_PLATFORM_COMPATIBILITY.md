# M2 — Platform Input and Compatibility

**Status: NOT STARTED** · Complexity: M–L · Tags: `[Editor]` `[Build]` `[Device]` `[Approval]`

## 1. Objective
Reliable controls and UI on Windows and Android.

## 2. Current repository state (STATIC)
- Keyboard: `KeyboardLeft` (WASD+Space) and `KeyboardRight` (arrows+Enter) implemented in `Tank_Actions`.
- Gamepad: scheme/bindings exist but desktop code path doesn't activate them (KI-010).
- Android: `MobileUIControl`/`UIVirtualJoystick` and `MobileControlCanvas.prefab` exist; not found in scenes (KI-008). P2 hidden on mobile.
- Project-wide actions persistence (KI-006); `Keyboard.current` null risk (KI-P03).
- UI scaling: `GameUIHandler.Update` toggles `matchWidthOrHeight`; no safe-area handling.

## 3. Scope
PC keyboard, PC gamepad, Android touch, UI responsiveness, resolutions/aspect ratios, platform validation. Not in scope: Android shared-screen multiplayer (PENDING DECISION).

## 4. Prerequisites
M0 accepted; M1 findings available (input-related). Physical Android device and a gamepad (UNKNOWN availability).

## 5. Tasks
- [ ] `[Editor][Build]` Verify keyboard P1/P2 simultaneous play (checklist W-02/W-03).
- [ ] `[Editor][Device]` Test a gamepad (W-04); confirm/deny KI-010.
- [ ] If KI-010 confirmed: `[Approval]` design minimal device-assignment extension (ADR-008) — e.g. per-slot gamepad option or auto-assign unused gamepad; keep keyboard behavior unchanged.
- [ ] `[Editor]` Verify/add `MobileControlCanvas` in the three arena scenes (KI-008); verify `MobileUIControl.AutoDisableOnNonMobilePlatform` behavior in Editor with device simulator.
- [ ] `[Device]` Android: joystick + fire work; charge and release behave; pause button works (A-03).
- [ ] Guard `Keyboard.current` null in `TankInputUser.Awake` (KI-P03) after confirmation.
- [ ] Ensure `InputSystem.actions` available in players (KI-006).
- [ ] `[Editor][Device]` UI scaling: test aspect matrix (W-05, A-04); add safe-area handling if clipping observed.
- [ ] Record per-platform results in `TESTING_CHECKLIST.md`.

## 6. Technical implementation notes
Use the per-tank cloned action asset; don't introduce `UnityEngine.Input`. Keep string names stable. Use `InputUser.PerformPairingWithDevice` for explicit pairing. Avoid rewriting `TankMovement` control schemes — extend.

## 7. Files/systems likely affected
`Tank/TankInputUser.cs`, `Tank/TankMovement.cs`, `UI/GameUIHandler.cs`, `UI/StartMenuSlot.cs`, `UI/MobileControl/*`, `Settings/Tank_Actions.inputactions`, scenes (touch canvas), `ProjectSettings` (project-wide actions).

## 8. Risks
Device pairing edge cases (hot-plug); on-screen controls vs. UI navigation conflicts; scene (binary→text) edits; limited device coverage.

## 9. Validation steps
Execute W-02..W-06, A-01..A-06; verify no NREs in player logs; test hot-plug gamepad.

## 10. Acceptance criteria
- PC: 2 humans on keyboard; gamepad works for at least one player.
- Android: single human with touch controls vs AI completes a match on a real device.
- UI usable on the aspect-ratio matrix; no clipped critical UI.
- No input-related errors in player logs.

## 11. Definition of Done
All criteria met with evidence; if device testing unavailable → REVIEW REQUIRED, not COMPLETED.

## 12. Completion summary
_(empty)_
