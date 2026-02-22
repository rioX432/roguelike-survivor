# BACKLOG.md — GlitchClaw MVP Issue Backlog

Each item = 1 feature branch + 1 PR. Acceptance criteria must ALL pass.

## Priority Legend
- **P0**: Must have (game won't work without it)
- **P1**: Should have (core experience)
- **P2**: Nice to have (polish)

---

## Phase 1: Foundation (0:00 - 1:30)

### 1. Player Movement [P0]
- **Branch**: `feat/player-movement`
- **Owner**: builder-alpha
- **Acceptance Criteria**:
  - [ ] Player moves with virtual joystick (New InputSystem)
  - [ ] Rigidbody2D.MovePosition() in FixedUpdate
  - [ ] Player cannot move off-screen
  - [ ] Compiles with zero errors

### 2. Camera Follow [P0]
- **Branch**: `feat/camera-follow`
- **Owner**: builder-alpha
- **Acceptance Criteria**:
  - [ ] Camera smoothly follows player (Vector3.Lerp or SmoothDamp)
  - [ ] Portrait mode (9:16 aspect ratio)
  - [ ] Orthographic camera with correct size

### 3. Pixel Art Assets Generation [P1]
- **Branch**: `feat/generated-assets`
- **Owner**: architect (via ComfyUI MCP)
- **Acceptance Criteria**:
  - [ ] Player sprite (32x32 or 64x64, GlitchClaw character)
  - [ ] 5 enemy sprites (Bit Drone, Glitch Bug, Rust Walker, Siege Core, Overlord AI)
  - [ ] Background tile (cyberpunk alley, tileable)
  - [ ] All placed in `Assets/Art/Generated/`

### 4. ScriptableObject Data Schemas [P0]
- **Branch**: `feat/scriptableobject-schemas`
- **Owner**: builder-beta
- **Acceptance Criteria**:
  - [ ] EnemyData SO with all fields from SYSTEM_DESIGN.md
  - [ ] WeaponData SO with AttackType enum
  - [ ] SpawnTableData SO with WaveEntry list
  - [ ] Create 5 EnemyData assets with values from CONCEPT.md
  - [ ] Create 3 WeaponData assets (Radial, Cone, Homing)

---

## Phase 2: Infinite Map + Enemy Spawn (1:30 - 3:30)

### 5. Infinite Map (Ring Buffer) [P0]
- **Branch**: `feat/infinite-map`
- **Owner**: builder-alpha
- **Acceptance Criteria**:
  - [ ] 3x3 chunk grid with SpriteRenderer
  - [ ] Chunks reposition when player crosses boundary
  - [ ] No Tilemap.SetTile() — Transform.position only
  - [ ] No visual seams between chunks
  - [ ] 60 FPS maintained

### 6. Object Pool System [P0]
- **Branch**: `feat/object-pool`
- **Owner**: builder-beta
- **Acceptance Criteria**:
  - [ ] PoolManager singleton with PreWarm/Spawn/Despawn
  - [ ] IPoolable interface (OnSpawn/OnDespawn)
  - [ ] Pre-warm 500 enemies + 200 projectiles at scene load
  - [ ] Zero runtime Instantiate() calls during gameplay

### 7. Enemy Base + AI [P0]
- **Branch**: `feat/enemy-base`
- **Owner**: builder-beta
- **Acceptance Criteria**:
  - [ ] EnemyBase : MonoBehaviour, IPoolable
  - [ ] Chase AI: move toward player position
  - [ ] Contact damage via OnTriggerEnter2D
  - [ ] Death → return to pool
  - [ ] Data-driven from EnemyData SO

### 8. Enemy Spawner [P0]
- **Branch**: `feat/enemy-spawner`
- **Owner**: builder-beta
- **Acceptance Criteria**:
  - [ ] Reads SpawnTableData for wave config
  - [ ] Spawns at random points on circle around player (off-screen)
  - [ ] Respects MaxActive cap per enemy type
  - [ ] Difficulty ramps over 10 minutes

---

## Phase 3: Combat (3:30 - 5:00)

### 9. Attack Module Base + Radial Attack [P0]
- **Branch**: `feat/attack-system`
- **Owner**: builder-alpha
- **Acceptance Criteria**:
  - [ ] AttackModule abstract base class
  - [ ] RadialAttack: 360° projectiles at interval
  - [ ] Projectiles are pooled
  - [ ] Damage enemies on contact
  - [ ] Data-driven from WeaponData SO

### 10. Cone Attack + Homing Attack [P1]
- **Branch**: `feat/attack-variants`
- **Owner**: builder-alpha
- **Acceptance Criteria**:
  - [ ] ConeAttack: forward-facing spread
  - [ ] HomingAttack: seeks nearest enemy
  - [ ] Both pooled and data-driven

### 11. Enemy Damage + Death [P0]
- **Branch**: `feat/enemy-damage`
- **Owner**: builder-beta
- **Acceptance Criteria**:
  - [ ] HP system on EnemyBase
  - [ ] TakeDamage() method
  - [ ] Death triggers EventBus.RaiseEnemyDeath()
  - [ ] Drop XP gem on death (pooled)

---

## Phase 4: Game Loop + UI (5:00 - 6:30)

### 12. XP + Level-Up System [P0]
- **Branch**: `feat/levelup-system`
- **Owner**: builder-alpha
- **Acceptance Criteria**:
  - [ ] XP gems auto-collect within magnetic radius
  - [ ] XP threshold: baseXP * level * 1.2
  - [ ] Level up → pause (TimeScale = 0)
  - [ ] Show 3 random upgrade options
  - [ ] Pick → resume

### 13. Level-Up UI Panel [P0]
- **Branch**: `feat/levelup-ui`
- **Owner**: builder-alpha
- **Acceptance Criteria**:
  - [ ] 3 upgrade cards with icon + description
  - [ ] On Canvas_Dynamic
  - [ ] Tap to select → resume game

### 14. HUD (HP, XP, Timer) [P0]
- **Branch**: `feat/hud`
- **Owner**: builder-alpha
- **Acceptance Criteria**:
  - [ ] HP bar (Canvas_Dynamic)
  - [ ] XP bar with level number (Canvas_Dynamic)
  - [ ] 10-minute countdown timer (Canvas_Static)
  - [ ] Canvas split: static vs dynamic elements

### 15. Result Screen [P1]
- **Branch**: `feat/result-screen`
- **Owner**: builder-alpha
- **Acceptance Criteria**:
  - [ ] Shows on game over or timer end
  - [ ] Displays: survival time, enemies killed, level reached
  - [ ] Retry button

### 16. Wave Table Data [P0]
- **Branch**: `feat/wave-data`
- **Owner**: builder-beta
- **Acceptance Criteria**:
  - [ ] SpawnTableData asset with 10-min wave progression
  - [ ] Bit Drone from 0:00, Glitch Bug from 1:00, etc.
  - [ ] Boss at 8:00
  - [ ] Balanced spawn rates

---

## Phase 5: Audio + Polish (6:30 - 8:00)

### 17. BGM Generation + Integration [P1]
- **Branch**: `feat/bgm`
- **Owner**: architect (via Suno MCP)
- **Acceptance Criteria**:
  - [ ] 1 BGM track (Celtic × Gagaku × J-Rock fusion)
  - [ ] Seamless loop (PyMusicLooper processed)
  - [ ] Placed in `Assets/Audio/Generated/BGM/`
  - [ ] AudioManager plays on game start

### 18. Sound Effects [P1]
- **Branch**: `feat/sound-effects`
- **Owner**: architect
- **Acceptance Criteria**:
  - [ ] attack_fire, enemy_hit, enemy_die, level_up, player_hit
  - [ ] Integrated via AudioManager
  - [ ] SE pool (4-8 concurrent AudioSources)

### 19. Performance Tuning [P2]
- **Branch**: `feat/perf-tuning`
- **Owner**: reviewer
- **Acceptance Criteria**:
  - [ ] 60 FPS with 500 enemies on mid-range device
  - [ ] GC alloc < 1KB/frame
  - [ ] Draw calls < 100 (SpriteAtlas)
  - [ ] Profiler screenshot attached to PR

### 20. Android + iOS Build [P0]
- **Branch**: `feat/final-build`
- **Owner**: builder-alpha
- **Acceptance Criteria**:
  - [ ] Android APK builds successfully
  - [ ] iOS Xcode project exports successfully
  - [ ] APK installs and runs on device (30s smoke test)
