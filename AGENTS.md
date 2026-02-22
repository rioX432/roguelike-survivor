# AGENTS.md — 8h Hackathon Roguelike Survivor

## Project

Unity 6 (URP 2D) portrait-mode survivors-like game. Android / iOS.
Source: `~/workspace/roguelike-survivor/` (Unity project TBD)

## Documents (ALWAYS read before acting)

- `CONCEPT.md` — Game concept, world, character design, art/audio direction
- `PLAN.md` — Timeline, MVP scope, task checklist
- `SYSTEM_DESIGN.md` — Technical spec, ScriptableObject schemas, system architecture
- `CLAUDE.md` — Coding conventions, architecture constraints, project structure

## Agent Roles

### architect (PM)
- Task decomposition and delegation to builder-alpha / builder-beta
- Asset generation via ComfyUI MCP (pixel art sprites, backgrounds)
- Progress tracking: update checkboxes in PLAN.md
- Scope guard: reject anything not in MVP scope
- Resolve conflicts between builders

### builder-alpha
- Player controller, camera follow, input system (virtual joystick)
- Attack modules (RadialAttack, ConeAttack, HomingAttack)
- UI implementation (Canvas_Static / Canvas_Dynamic split)
- Scene file ownership: ONLY builder-alpha edits .unity files

### builder-beta
- Enemy AI, EnemyBase, EnemySpawner
- ScriptableObject data creation (EnemyData, WeaponData, SpawnTableData)
- Wave/spawn system, difficulty curve
- Prefab creation for enemies and projectiles
- NEVER edit scene files — only Prefabs and ScriptableObjects

### reviewer
- Code review: GC alloc in hot paths, missing pool returns, null refs
- Performance: check for Update() allocations, excessive GetComponent calls
- Pattern compliance: verify CLAUDE.md conventions are followed
- Run profiler checks when possible

## Git Protocol

- Commit per feature, message in English, 1 line
- Always include .meta files
- Scene conflicts: builder-alpha has priority
- Never commit: API keys, .env files, progress.txt

## Communication

- Japanese for human-facing messages
- English for code, comments, commit messages
- Keep status updates concise

## Security (CRITICAL)

See SECURITY.md for full details. Key rules:
- NEVER log, echo, or expose API keys or tokens
- NEVER commit secrets to git
- NEVER run commands outside the project workspace without asking
- NEVER expose OpenClaw gateway to non-localhost
- All destructive operations (rm, git reset --hard) require human approval
