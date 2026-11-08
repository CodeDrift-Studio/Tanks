> **SUPERSEDED 2026-11-08 — historical reference only.** This is the original Windows+Android MVP milestone from 2026-11-06. The authoritative plan is `MILESTONE_ROADMAP.md` (M0–M8, Android-first). Do not execute tasks from this file; its content is kept unchanged below for traceability.

# M5 — Original Content and Differentiation

**Status: NOT STARTED** · Complexity: L–XL · Tags: `[Editor]` `[Build]` `[Device]` `[Approval]`

## 1. Objective
Make the game original and legally clean, and give it its own character — without silently expanding MVP.

## 2. Current repository state (STATIC)
All art/audio/prefabs derive from the Unity sample (Asset Store EULA + listed third-party licences). Existing variants: tanks `Demo_Tank` + ATV/Heavy/Medium/Shark; 15 tank FBX models in `Art/Models/Tanks`; six power-up types; three arenas. No original content has been identified. Licence applicability UNKNOWN (KI-007).

## 3. Scope
**Mandatory for MVP (minimum)**: whatever replacement/transformation of sample-derived content the licensing review (KI-007) requires, plus original branding assets (with M7).
**OPTIONAL (needs approval per item)**: original tank variations; new mechanics; additional weapon/power-up variations; original arena content (new arenas are out of MVP); replayability features.

## 4. Prerequisites
M3 accepted; licensing review outcome or at least a list of questions (KI-007); art direction decision (PENDING).

## 5. Tasks
- [ ] `[Approval]` Build asset provenance table (every imported asset: source, licence, replaceable?) — see M7 task; store in `Documentation/` (new file, owner OK).
- [ ] `[Approval]` Decide, per asset class (models, textures, audio, UI, code), keep/replace based on licensing review.
- [ ] Define the differentiation list (max N items; owner picks) — do not implement unlisted ideas.
- [ ] **OPTIONAL** Tank variations via prefab variants + data (stats only first).
- [ ] **OPTIONAL** One new mechanic/weapon/power-up variation prototype; evaluate in playtest before keeping.
- [ ] **OPTIONAL** Original arena dressing (re-skin within the 3 arenas; new arenas out of MVP).
- [ ] Update tests/checklists for every new content item.

## 6. Technical implementation notes
Prefab variants over duplication; data-driven stats (ADR-006); keep power-up system extensible via `PowerUpType` enum + `PowerUpDetector` (extend, don't rewrite); respect existing layer ("Players") and NavMesh setups when editing arenas (rebake needed after geometry change — binary data in `Demo_NavMeshes`).

## 7. Files/systems likely affected
`Art/*`, `Prefabs/*`, `PowerUp/*`, `Tank/*`, arena scenes + NavMesh/lighting data.

## 8. Risks
Scope expansion; art cost; breaking lighting/NavMesh bakes; unresolved licensing driving large rework; balance regressions.

## 9. Validation steps
Per-item playtests; AI works in modified arenas; build-size and performance check; regression run of checklists.

## 10. Acceptance criteria
- Licensing-driven replacements completed (or owner-documented acceptance of remaining items).
- Approved differentiation items implemented and playtested.
- No MVP scope creep (diff against the approved list).

## 11. Definition of Done
Criteria met with evidence; provenance table updated; docs updated.

## 12. Completion summary
_(empty)_
