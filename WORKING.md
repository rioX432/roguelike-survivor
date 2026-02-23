# WORKING.md — Shared Task Board

All agents MUST read this file at the start of each session.
Update your section when you start/complete a task.

## Current Sprint: Phase 1 — Foundation

### architect
- Status: monitoring
- Current task: Phase 1 review — awaiting PR merges, then Phase 2 setup
- Blocked by: Claude Code outdated (v1.0.68 → needs v1.0.88+), skipped via direct script writing

### builder-alpha
- Status: PR ready (awaiting review/merge)
- Current task: -
- Completed: feat/player-movement → PR #1

### builder-beta
- Status: PR ready (awaiting review/merge)
- Current task: -
- Completed: feat/scriptableobject-schemas → PR #2

### reviewer
- Status: idle
- Current task: Review PR #1 and PR #2
- Blocked by: -

## Task Queue (Priority Order)

### In Progress
- [ ] [human/architect] Review & merge PR #1 (player-movement + camera)
- [ ] [human/architect] Review & merge PR #2 (SO schemas editor script)
- [ ] [human] Run "GlitchClaw/Generate All Game Data" in Unity Editor after merging PR #2
- [ ] [human] Assign sprites to EnemyData/WeaponData in Inspector
- [ ] [human] Assign EnemyData refs to SpawnTable wave entries in Inspector
- [ ] [architect] feat/generated-assets — ComfyUI pixel art generation (player + 5 enemies + bg tile)

### Pending (Phase 2)
- [ ] [builder-alpha] feat/infinite-map — 3x3 chunk ring buffer
- [ ] [builder-beta] feat/object-pool — PoolManager pre-warm, IPoolable
- [ ] [builder-beta] feat/enemy-base — EnemyBase + chase AI + contact damage
- [ ] [builder-beta] feat/enemy-spawner — reads SpawnTableData, off-screen spawn circle

### Pending (Phase 3+)
- [ ] [builder-alpha] feat/attack-system — AttackModule base + RadialAttack
- [ ] [builder-alpha] feat/attack-variants — ConeAttack + HomingAttack
- [ ] [builder-beta] feat/enemy-damage — HP + TakeDamage + XP drop
- [ ] [builder-alpha] feat/levelup-system — XP magnet + level up + TimeScale=0 pause
- [ ] [builder-alpha] feat/levelup-ui — 3-card upgrade panel
- [ ] [builder-alpha] feat/hud — HP/XP bars + timer
- [ ] [builder-alpha] feat/result-screen — stats display + retry button
- [ ] [builder-beta] feat/wave-data — SpawnTableData 10-min progression
- [ ] [architect] feat/bgm — Suno BGM generation
- [ ] [architect] feat/sound-effects — SE integration
- [ ] [reviewer] feat/perf-tuning — profiler 60FPS check
- [ ] [builder-alpha] feat/final-build — Android APK + iOS Xcode

### Completed
- [x] Core scripts: GameManager, PoolManager, EventBus, IPoolable (pre-existing)
- [x] Data class definitions: EnemyData.cs, WeaponData.cs, SpawnTableData.cs (pre-existing)
- [x] feat/player-movement: PlayerController, PlayerStats, VirtualJoystick, CameraFollow → PR #1
- [x] feat/scriptableobject-schemas: GameDataGenerator EditorScript → PR #2

## Pull Requests

| PR | Branch | Title | Status |
|---|---|---|---|
| #1 | feat/player-movement | Add PlayerController, PlayerStats, VirtualJoystick, CameraFollow | Open |
| #2 | feat/scriptableobject-schemas | Add GameDataGenerator EditorScript | Open |

## Decisions Log

- [2026-02-22] Unity project at `roguelike-survivor-game/` (URP 2D template, basic structure ready)
- [2026-02-22] Core scripts (GameManager, PoolManager, EventBus) already implemented
- [2026-02-22] Data class C# files already exist; builder-beta focuses on EditorScript
- [2026-02-22] Claude Code v1.0.68 outdated, needs sudo to update; architect wrote scripts directly
- [2026-02-22] CameraFollow included in feat/player-movement PR (same logical unit)
- [2026-02-22] SpawnTable wave EnemyData refs must be assigned manually in Unity Inspector
- [DECISION NEEDED] Virtual joystick: using custom VirtualJoystick.cs (not Joystick Pack asset)
- [DECISION NEEDED] ComfyUI MCP: mcporter not in PATH — needs investigation

## Blockers / Issues

- **Claude Code v1.0.68**: Needs update to v1.0.88+. Requires sudo permissions: `sudo chown -R $USER:$(id -gn) $(npm -g config get prefix)` then `claude update`
- **mcporter CLI**: Not found in PATH. Verify Unity MCP and ComfyUI MCP servers before Phase 2 scene setup
- **SpawnTable refs**: EnemyData references in SpawnTableData must be linked manually in Unity Inspector after running GameDataGenerator
- **Sprites**: No pixel art assets yet — ComfyUI generation blocked on mcporter

## Notes for Next Session (Phase 2)

Before starting Phase 2:
1. Merge PRs #1 and #2
2. Run "GlitchClaw/Generate All Game Data" in Unity
3. Check if mcporter is available: try `~/.npm/bin/mcporter` or `npx mcporter`
4. Start ComfyUI for asset generation
5. Phase 2 assigns: builder-alpha (feat/infinite-map), builder-beta (feat/object-pool + feat/enemy-base)
