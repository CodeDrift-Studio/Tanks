# M6 — Optimization and Quality Assurance

**Status: NOT STARTED** · Complexity: L · Tags: `[Editor]` `[Build]` `[Device]` `[Approval]`

## 1. Objective
Meet defined performance/stability targets on Windows and Android; establish regression safety.

## 2. Current repository state (STATIC)
No tests; no profiling data; no budgets (ADR-009 pending). Known allocation sources: `TankAI` per-repath `NavMeshPath` + LINQ, `renderer.materials` in setup, `Instantiate/Destroy` for shells/explosions/pickup FX, string compares per FixedUpdate in `TankMovement`. 14 shaders + 4 ShaderGraphs; URP PC/Mobile assets.

## 3. Scope
Android + Windows profiling, memory, GC allocation, regression tests, device compatibility, input validation, stability.

## 4. Prerequisites
M4 accepted (M5 if content added). Approved budgets (target FPS, min device, memory, build size). Test devices.

## 5. Tasks
- [ ] `[Approval]` Set performance budgets (ADR-009).
- [ ] `[Device]` Profile Android with Unity Profiler (development build) in 2- and 4-tank matches; record frame time, GC, memory.
- [ ] `[Build]` Profile Windows similarly.
- [ ] Fix only measured hotspots (pooling via `ObjectPool<T>`, cache lookups, reduce AI allocations).
- [ ] Memory: 10 consecutive matches, check growth/leaks (PF-04).
- [ ] Add EditMode tests for pure logic (e.g., damage/charge math, winner determination if extracted) and PlayMode tests (round loop, AI soak) — asmdefs (ADR-005).
- [ ] Regression pass over the entire `TESTING_CHECKLIST.md` on both platforms.
- [ ] Device compatibility matrix (min 3 Android devices incl. low-end — owner supplies).
- [ ] Stability: long-session soak, background/resume, interruption handling.

## 6. Technical implementation notes
Measure before optimizing; one optimization per change with before/after numbers logged. Avoid architectural changes.

## 7. Files/systems likely affected
`Tank/TankAI.cs`, `Shell/*`, pooling helpers, graphics settings/quality tiers, texture/audio import settings, tests (new).

## 8. Risks
Device availability; perf regressions from content (M5); changing import settings affects many assets (approval).

## 9. Validation steps
Profiler captures attached to log entries; checklist runs; CI/test runner results (if available).

## 10. Acceptance criteria
Budgets met on defined devices; zero known P0/P1 stability issues; regression suite executed and results recorded; allocation hot spots resolved or accepted.

## 11. Definition of Done
Criteria met with evidence; docs updated.

## 12. Completion summary
_(empty)_
