# WORKING.md — Shared Task Board

All agents MUST read this file at the start of each session.
Update your section when you start/complete a task.

## Current Sprint: Phase 1 — Foundation (0:00 - 1:30)

### architect
- Status: active
- Current task: Phase 1 orchestration — spawning builder-alpha and builder-beta
- Blocked by: -

### builder-alpha
- Status: active
- Current task: feat/player-movement (PlayerController + virtual joystick), then feat/camera-follow
- Blocked by: -

### builder-beta
- Status: active
- Current task: feat/scriptableobject-schemas (verify SO classes, create EditorScript to generate assets)
- Blocked by: -

### reviewer
- Status: idle
- Current task: -
- Blocked by: -

## Task Queue (Priority Order)

### In Progress
- [ ] [builder-alpha] feat/player-movement — PlayerController.cs, virtual joystick, Rigidbody2D movement
- [ ] [builder-alpha] feat/camera-follow — CameraFollow.cs, portrait mode 9:16
- [ ] [builder-beta] feat/scriptableobject-schemas — Verify SO classes, EditorScript to create EnemyData/WeaponData assets

### Pending (Phase 1 remaining)
- [ ] [architect] feat/generated-assets — ComfyUI pixel art generation (player, 5 enemies, bg tile)

### Pending (Phase 2+)
- [ ] [builder-alpha] feat/infinite-map
- [ ] [builder-beta] feat/object-pool
- [ ] [builder-beta] feat/enemy-base
- [ ] [builder-beta] feat/enemy-spawner
- [ ] [builder-alpha] feat/attack-system
- [ ] [builder-alpha] feat/attack-variants
- [ ] [builder-beta] feat/enemy-damage
- [ ] [builder-alpha] feat/levelup-system
- [ ] [builder-alpha] feat/levelup-ui
- [ ] [builder-alpha] feat/hud
- [ ] [builder-alpha] feat/result-screen
- [ ] [builder-beta] feat/wave-data
- [ ] [architect] feat/bgm
- [ ] [architect] feat/sound-effects
- [ ] [reviewer] feat/perf-tuning
- [ ] [builder-alpha] feat/final-build

### Completed
- [x] Core scripts: GameManager, PoolManager, EventBus, IPoolable
- [x] Data class definitions: EnemyData.cs, WeaponData.cs, SpawnTableData.cs

## Decisions Log

- [2026-02-22] Unity project located at `roguelike-survivor-game/` (URP 2D template, basic structure ready)
- [2026-02-22] Core scripts (GameManager, PoolManager, EventBus) already implemented — skip re-implementation
- [2026-02-22] Data class C# files already exist — builder-beta focuses on EditorScript to generate .asset files
- [2026-02-22] Phase 1 started: builder-alpha (player+camera), builder-beta (SO assets) running in parallel
- [2026-02-22] mcporter not in PATH — Unity MCP availability to be confirmed before SO asset generation
- [DECISION NEEDED] Virtual joystick: use Joystick Pack asset or custom implementation?

## Blockers / Issues

- mcporter CLI not found in PATH — need to verify Unity MCP server for scene/prefab ops
- ComfyUI asset generation blocked until mcporter/MCP servers confirmed available
