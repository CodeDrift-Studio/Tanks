# GIT WORKFLOW

Remote: `https://github.com/CodeDrift-Studio/Tanks.git` · Default branch: `main`.
Policy source: owner-approved Git workflow (2026-11-07). Summary also lives in `CLAUDE.md` §N.
Evidence labels as in `CLAUDE.md`. Repository setup facts below are VERIFIED (2026-11-07) unless marked otherwise.

---

## 1. Branches

| Branch | Role | Who commits | Rules |
|---|---|---|---|
| `main` | Stable / release | Nobody directly. Receives `dev` only via reviewed PR | No force-push, no reset, no rewriting published history. No unfinished R&D. Static inspection alone is not release proof. |
| `dev` | Active development (**default working branch**) | Claude Code / owner | Verify the active branch before any change. Logical, reviewable commits. No mixing unrelated changes. |
| `xyz-backup` | Archive of removed/deferred material | Only for archiving | Never develop here. Never merge into `dev`/`main`. Not a substitute for a full independent backup. No Library/Temp/Logs, secrets, personal data or build artifacts. |

Flow: `feature/*`, `fix/*`, `experiment/*` → `dev` → (PR) → `main`. `xyz-backup` stays separate.
Do not add other long-lived branches without owner approval.

### Naming
- `feature/<short-name>`, `fix/<short-name>`, `experiment/<short-name>` — lowercase, hyphenated, branched from `dev`, merged back into `dev` after review, then deleted.

## 2. Repository state after setup (VERIFIED 2026-11-07)

- Remote `main` was **not empty**: GitHub-generated commit `0d9ef83` ("Initial commit": `.gitignore` = GitHub Unity template, `README.md`). Local history was built on top of it; no unrelated histories, no force-push.
- `main` = `0d9ef83` (unchanged on purpose — it will receive the project only through the first `dev` → `main` PR).
- `dev` and `xyz-backup` = `784095d` (project baseline, child of `0d9ef83`).
- Baseline: 1047 files; no `Library/Temp/Logs/UserSettings/obj`, no `*.csproj`/`*.slnx`; every `Assets/**` file has its `.meta`.
- Baseline commit subject begins with an invisible UTF-8 BOM (shell quirk). Cosmetic; published history is not rewritten to fix it.
- Assets are **binary-serialized** at baseline (KI-003). Force Text re-serialization (approved, pending in M0) will produce one very large diff — commit it **alone**, on its own branch (`fix/force-text`), after a confirmed independent backup.

## 3. Commit conventions

- Small, logical units; one concern per commit. Never mix Unity asset changes with unrelated code or docs.
- Subject: `<type>: <imperative summary>` (≤ 72 chars). Types: `feat`, `fix`, `refactor`, `docs`, `chore`, `test`, `build`, `revert`.
- Body: what and why; list affected milestone (e.g. `M0`) and any KI/ADR ids; state what was **not** validated.
- Unity: commit an asset and its `.meta` together. Never delete `.meta` files, never regenerate GUIDs. Don't commit half-saved scenes/prefabs (Editor must be idle and assets saved).
- Commit only when authorized; do not push unless authorized for that session.
- Attribution trailer for Claude-made commits: `Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>`.
- On Windows PowerShell, pass messages with `git commit -F <file>` (write the file without BOM, e.g. with the Write tool) — piping a here-string to `-F -` failed in setup.

## 4. Pull request policy (`dev` → `main`)

Promote only when **all** hold: milestone/feature complete; relevant validation executed with evidence; blockers documented; no unapproved changes; full PR diff reviewed; owner approves.
- Base = `main`, compare = `dev`. Never merge automatically. No force-push.
- PR description lists: scope, milestone, validation performed, validation NOT performed, known issues.
- Feature branches → `dev` by PR or reviewed local merge; keep history readable.

## 5. Unity `.gitignore` policy

Base: GitHub Unity template (already on `main`) plus project additions: `.vscode/`, `.idea/`, OS junk (`.DS_Store`, `Thumbs.db`, `desktop.ini`), `*.keystore`, `*.jks`.
- **Ignored**: `Library/ Temp/ Obj/ Logs/ UserSettings/ Build/ Builds/ MemoryCaptures/ Recordings/`, IDE caches, generated `*.csproj`/`*.sln`/`*.slnx`, `*.apk`/`*.aab`.
- **Tracked**: `Assets/` (+ all `.meta`), `Packages/manifest.json`, `Packages/packages-lock.json`, `ProjectSettings/`, `Documentation/`, `CLAUDE.md`, `PROJECT_AUDIT.md`.
- Never commit keystores, passwords, tokens. If one is ever committed, stop and tell the owner (history cleanup needs owner decision; don't improvise).
- Git LFS: **PENDING DECISION** (M0). Largest tracked file at baseline < 20 MB; total ≈ 87 MB. Revisit when art/audio grows (GitHub hard limit 100 MB/file).

## 6. Archive procedure (`xyz-backup`)

1. Decide: obsolete or merely deferred?
2. Inspect dependencies: GUIDs, scene/prefab references, scripts, Build Settings. A text search finding nothing is **not** proof of "unused" (binary scenes/prefabs!).
3. Record original path, reason, date (append to the table below).
4. Preserve on `xyz-backup` (commit the material under `archive/<original-path>`; keep `.meta` files), push, verify with `git ls-remote --heads origin` / `git ls-tree`.
5. Only then remove from `dev` — with owner approval — in a separate commit.
6. Update docs to say where the material went.

Archive log (append-only):

| Date | Original path | Reason | Archived in commit | Removed from dev? |
|---|---|---|---|---|
| — | — | nothing archived yet | — | — |

## 7. Daily session protocol

**Start**
1. `git status --short --branch` — confirm branch (normally `dev`) and tree state.
2. `git fetch origin` when appropriate; check `origin/dev` divergence.
3. Never switch branches with uncommitted changes without protecting them (commit or `git stash`). Never discard user work.
4. Confirm task and milestone (`CLAUDE.md` §M), then start.

**End**
1. Review changed files and the full diff for unintended edits (scenes/prefabs/ProjectSettings especially).
2. Check for generated/unneeded files; run available validation.
3. Update milestone docs (`CLAUDE.md` §L).
4. Summarize changes and evidence.
5. Commit if authorized (logical groups); push only if authorized.

## 8. Recovery guidance

- Mistaken local commit, **not pushed**: `git reset --soft HEAD~1` (keeps changes). Don't use `--hard` without checking `git status`/`git diff` first.
- Mistaken commit **already pushed**: add a corrective commit or `git revert <sha>`. Never force-push or rewrite published history.
- Unity looks broken after a pull/switch: close Unity, check `git status` for missing `.meta`, reopen; delete only `Library/` if truly needed (it is regenerated — slow, but safe).
- Wrong branch: stash or commit, `git switch dev`, apply (`git stash pop` / `git cherry-pick`).
- Lost files: `git reflog`, `git fsck --lost-found`; for archived material see `xyz-backup`.
- Remote/local divergence: stop, report; no `--allow-unrelated-histories`, no force without owner approval.
- Full-folder copy outside the project directory remains the owner's real backup (git is not one).
