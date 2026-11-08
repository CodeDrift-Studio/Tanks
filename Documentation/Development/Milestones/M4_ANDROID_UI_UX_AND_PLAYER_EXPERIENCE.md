# M4 — Android UI/UX and Player Experience

**Status: NOT STARTED** · Effort: 22–40 days (impl 18–32, test 4–8) · Tags: `[Editor]` `[Device]` `[Approval]`
References: `RELEASE_GAP_ASSESSMENT.md` §6.2, §7, UI-01…UI-05, AND-03, AUD-01; KI-P07.

## 1. Objective
A touch-first, readable, polished interface for phones (and tablets if approved) with clear first-time guidance.

## 2. Scope
- Touch-first UI system and style (uGUI + TMP, existing stack).
- Main menu and match setup. (Arena selection screen is **not** a first-release requirement: Desert only, DR-C14; keep layouts able to add one later.)
- Gameplay HUD (screen-space health, scores, power-up timers, round text).
- Pause menu and settings screen (audio volumes, controls options, other approved settings; persistence from M3).
- Round results and match results screens (rematch / menu; no next-arena action in the first release).
- Safe area (cutouts), orientation lock/behavior, aspect-ratio support, canvas scaling.
- Touch target sizing; joystick/fire layout, dead zones, size/position options; control tuning.
- First-time player instructions / lightweight onboarding.
- Audio feedback (UI sounds, music/SFX mixing hooks) and visual feedback (hit/pickup cues), transitions.
- Placeholder slots (not implementation) for ad/IAP/GPGS UI entry points to avoid rework in M5/M6.

## 3. Out of scope
Final brand art and identity (M7); SDK behavior (M5/M6); new gameplay; localization beyond approved launch languages (DR-P20).

## 4. Prerequisites
M3 exit gate; DR-P9 (orientation, tablet), DR-P20 (localization), DR-P11 licensing direction before large art spend.

## 5. Tasks
- [ ] `[Approval]` UI style guide and screen list; wireframes approved by owner.
- [ ] Main menu, setup (UI-01). ~~Arena selection (UI-02)~~ deferred with Jungle/Moon (DR-C14).
- [ ] Gameplay HUD for phones (UI-05); readability on small screens.
- [ ] Pause + settings (UI-04); results screens (UI-03).
- [ ] Safe area + orientation + aspect handling (AND-02 remainder).
- [ ] Touch ergonomics: target sizes (agreed minimum), joystick/fire tuning (AND-03), optional left-handed/size options.
- [ ] First-time instructions/onboarding.
- [ ] Audio and UI feedback (AUD-01 part): UI click sounds, mixer volume controls wired to settings.
- [ ] `[Device]` Playtests on the device matrix (small phone, mid phone, tall/notched phone, tablet if in scope).
- [ ] Update `TESTING_CHECKLIST.md` UI rows with evidence.

## 6. Dependencies
M3 flow and settings layer; Editor; devices; art assets (placeholder allowed until M7).

## 7. Risks
Art rework when licensing/branding changes (M7); device diversity; UI built on desktop-oriented prefabs; time sink in polish.

## 8. Required approvals
Wireframes/style; scene and prefab changes; any package (e.g. new UI framework) — none planned.

## 9. Validation procedure
Device screenshots on the matrix (portrait/landscape as supported); one-handed/thumb reach checks; ADB `logcat` review; non-developer playtest where possible.

## 10. Acceptance criteria
- All required screens exist, are connected to the real flow and usable by touch.
- No clipped or overlapped UI on any device in the matrix (notch/cutout/gesture bars included).
- Touch targets meet the agreed minimum; controls do not overlap HUD or ad placeholders.
- A first-time player can start and finish a match without external instructions (observed playtest).
- Settings changes apply and persist.

## 11. Evidence to record
Screenshots per device/screen, playtest notes (tester, date, build), checklist results, issues filed.

## 12. Exit gate
Acceptance met; owner signs off on UX. Next: M5.

## Completion summary
_(empty)_
