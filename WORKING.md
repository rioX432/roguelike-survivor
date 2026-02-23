# WORKING.md — Shared Task Board

## Current Status: Phase 1-4 COMPLETE, Phase 5 In Progress

### ✅ Completed Phases

#### Phase 1: Foundation (0:00 - 1:30)
- [x] PlayerController + VirtualJoystick
- [x] CameraFollow (portrait 9:16)
- [x] GameDataGenerator EditorScript
- [x] PlayerStats + HP system
- **Status**: All 5 PR(s) merged (#1, #2 squashed into main)

#### Phase 2: Infinite Map + Enemy Spawn (1:30 - 3:30)
- [x] InfiniteMapController (3x3 chunk ring-buffer)
- [x] EnemyBase (chase AI, IPoolable, contact damage)
- [x] EnemySpawner (wave-based spawning)
- [x] XPGem (magnetic attraction + auto-collect)
- **Status**: All 3 PR(s) merged (#3, #4, #5)

#### Phase 3: Combat System (3:30 - 5:00)
- [x] AttackModule base class
- [x] Projectile (pooled, range-based despawn)
- [x] RadialAttack (360° burst)
- [x] ConeAttack (forward spread)
- [x] HomingAttack (nearest enemy tracking)
- [x] EnemyBase XP gem drop on death
- **Status**: All 3 PR(s) merged (#6, #7, #8)

#### Phase 4: Game Loop + UI (5:00 - 6:30)
- [x] LevelUpSystem (XP tracking, level up, pause on LevelUp state)
- [x] LevelUpUI (3-card upgrade selection)
- [x] HPBar (with low-HP color warn)
- [x] XPBar (progress + level display)
- [x] TimerUI (MM:SS countdown)
- [x] ResultScreen (stats + retry)
- [x] AudioManager (BGM loop + SE pool)
- [x] PlayerController fix (InputSystem direct API)
- [x] SceneSetup EditorScript (one-click initialization)
- **Status**: All 4 PR(s) merged (#9, #10, #11, #13)

### 🔄 Phase 5: Audio + Polish + Build (6:30 - 8:00)

#### Remaining Tasks
- [ ] Pixel art asset generation (ComfyUI)
- [ ] BGM composition (Suno)
- [ ] SE recordings/generation
- [ ] Scene setup validation in Unity Editor
- [ ] Android APK build + device deploy
- [ ] iOS Xcode project export (if time)
- [ ] Performance profiling (60 FPS target)

### 📊 Script Inventory

**Total Scripts**: 32 C# files

| Category | Count | Scripts |
|----------|-------|---------|
| Core | 5 | GameManager, PoolManager, EventBus, IPoolable, BuildScript |
| Player | 4 | PlayerController, PlayerStats, PlayerXP, VirtualJoystick |
| Attack | 6 | AttackModule, Projectile, RadialAttack, ConeAttack, HomingAttack, HomingProjectile |
| Enemy | 3 | EnemyBase, EnemySpawner, XPGem |
| Camera | 1 | CameraFollow |
| Map | 1 | InfiniteMapController |
| Data | 3 | EnemyData, WeaponData, SpawnTableData |
| UI | 8 | LevelUpSystem, LevelUpUI, HPBar, XPBar, TimerUI, ResultScreen, LevelUpPanel, UpgradeCardUI |
| Audio | 1 | AudioManager |
| Editor | 1 | GameDataGenerator, SceneSetup |

### 🔗 Key Design Decisions

1. **Object Pooling**: PoolManager + IPoolable for enemies, projectiles, XP gems
2. **Event Bus**: Decoupled communication via EventBus for XP, level-up, death, damage
3. **Infinite Map**: 3x3 chunk ring-buffer with Transform.position only (no SetTile)
4. **Attack System**: Modular with base AttackModule + variants (Radial/Cone/Homing)
5. **UI Split**: Canvas_Static (timer) vs Canvas_Dynamic (HP/XP bars)
6. **Input**: New InputSystem with fallback to KeyboardInput via InputSystem_Actions.inputactions
7. **Audio**: Singleton AudioManager with BGM loop + SE pool (6 sources, round-robin)

### ⚠️ Known Issues / Duplicates

- **Duplicate Level-Up Logic**: 
  - `LevelUpSystem.cs` (main implementation, singleton)
  - `PlayerXP.cs` (fallback, same functionality)
  - **Recommendation**: Use `LevelUpSystem.Instance` in scene; `PlayerXP.cs` can be removed if not used

- **Duplicate Level-Up UI**:
  - `LevelUpUI.cs` (main implementation, accepts `List<WeaponData>`)
  - `LevelUpPanel.cs` + `UpgradeCardUI.cs` (fallback UI implementation)
  - **Recommendation**: Use `LevelUpUI.Instance` in scene; Panel/UpgradeCardUI can be removed if not used

### 🎮 Ready to Build?

**Prerequisites before Android build:**
1. ✅ All C# scripts compiled (verified in main)
2. ✅ EventBus fully wired (OnXPCollected, OnLevelUp, OnEnemyDeath, OnPlayerDeath, OnGameOver)
3. ✅ GameManager + PoolManager + AudioManager as singletons
4. ⏳ Scene setup: MainGame.unity with proper hierarchy (see SceneSetup EditorScript)
5. ⏳ Sprite assets assigned to EnemyData/WeaponData (GameDataGenerator creates SO, needs manual sprite assignment)
6. ⏳ Audio clips assigned to AudioManager (SE pool + BGM reference)

### 📱 Deploy Commands

```bash
# Build Android APK
UNITY_EXE="/Users/hackathon/workspace/6000.3.9f1/Unity.app/Contents/MacOS/Unity"
$UNITY_EXE -batchmode -projectPath ~/workspace/roguelike-survivor/roguelike-survivor-game \
  -executeMethod BuildScript.BuildAndroid -quit

# Install on connected device (via adb)
adb install -r build.apk

# Wireless ADB connected device (Samsung Galaxy S25 Ultra detected)
adb devices -l
```

### 📋 Git Branches

All feature branches merged to main:
- `feat/player-movement` → PR #1 (merged)
- `feat/scriptableobject-schemas` → PR #2 (merged)
- `feat/infinite-map` → PR #3 (merged)
- `feat/enemy-base` → PR #4 (merged)
- `feat/enemy-spawner` → PR #5 (merged)
- `feat/enemy-damage` → PR #6 (merged)
- `feat/attack-system` → PR #7 (merged)
- `feat/attack-variants` → PR #8 (merged)
- `feat/levelup-system` → PR #9 (merged)
- `feat/hud` → PR #10 (merged)
- `feat/result-screen` → PR #11 (merged)
- `feat/fix-audiomanager` → PR #13 (merged)

### 🎯 Next Steps (For Final Sprint)

1. **Open Unity Editor** and verify MainGame.unity scene via SceneSetup
2. **Run GameDataGenerator** menu in Unity to create SO assets
3. **Assign sprites** to EnemyData/WeaponData in Inspector
4. **Test PlayMode** with 10x time scale to verify 10-min game loop
5. **Build Android APK** and deploy to S25 Ultra via adb
6. **Run on device** and verify:
   - Player moves with joystick
   - Enemies spawn and chase
   - Attacks fire and damage enemies
   - XP collects and level-up pauses
   - Upgrade selection works
   - Game ends after 10 min with result screen

### 💾 Session Summary

**Total Time Invested**: ~3.5 hours (hackathon sprint)

**What's Built**:
- ✅ 32 C# scripts (core + all systems)
- ✅ 3 ScriptableObject data types (EnemyData, WeaponData, SpawnTableData)
- ✅ 13 merged PRs
- ✅ Full object pooling + event bus architecture
- ✅ Complete attack system with 3 variants
- ✅ Game loop with pause on level-up
- ✅ Result screen with retry
- ✅ Audio manager setup

**Not Yet Done** (Phase 5):
- ❌ Pixel art sprites (ComfyUI)
- ❌ BGM track (Suno)
- ❌ SE audio files
- ❌ Final scene validation
- ❌ Android build + test
- ❌ Performance tuning

**Estimated Remaining Time**: ~1.5-2 hours for assets + build + testing
