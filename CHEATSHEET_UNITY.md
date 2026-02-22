# Unity Cheat Sheet

## Paths

| Item | Path |
|---|---|
| Unity Editor | `/Users/hackathon/workspace/6000.3.9f1/Unity.app` |
| Unity Binary | `/Users/hackathon/workspace/6000.3.9f1/Unity.app/Contents/MacOS/Unity` |
| Project | `/Users/hackathon/workspace/roguelike-survivor/roguelike-survivor-game/` |
| Scripts | `roguelike-survivor-game/Assets/Scripts/` |
| Package Manifest | `roguelike-survivor-game/Packages/manifest.json` |

## Unity Hub

### Editor re-register (after moving to workspace)
1. Open Unity Hub
2. Installs tab -> "Locate" button
3. Select: `/Users/hackathon/workspace/6000.3.9f1/Unity.app`

### Open project
1. Unity Hub -> Projects -> Add -> Select `roguelike-survivor-game/` folder
2. Or double-click the project in Unity Hub

## mcp-unity Package Setup

### Option A: Package Manager GUI
1. Unity -> Window -> Package Manager
2. "+" button -> "Add package from git URL..."
3. URL: `https://github.com/CoderGamester/mcp-unity.git`

### Option B: Edit manifest.json directly
Add to `roguelike-survivor-game/Packages/manifest.json` dependencies:
```json
"com.gamelovers.mcp-unity": "https://github.com/CoderGamester/mcp-unity.git"
```

### Verify mcp-unity
1. Unity -> Window -> Package Manager -> "In Project" filter
2. "MCP Unity" should appear in the list
3. mcp-unity server can now communicate with the editor

## CLI Build Commands

```bash
UNITY="/Users/hackathon/workspace/6000.3.9f1/Unity.app/Contents/MacOS/Unity"
PROJECT="/Users/hackathon/workspace/roguelike-survivor/roguelike-survivor-game"

# Android APK
$UNITY -batchmode -projectPath "$PROJECT" -executeMethod BuildScript.BuildAndroid -quit -logFile -

# iOS Xcode project
$UNITY -batchmode -projectPath "$PROJECT" -executeMethod BuildScript.BuildiOS -quit -logFile -

# Run tests
$UNITY -batchmode -projectPath "$PROJECT" -runTests -testResults results.xml -quit -logFile -
```

## Project Structure (Target)

```
roguelike-survivor-game/Assets/
  Scripts/
    Core/           # GameManager, PoolManager, EventBus, IPoolable
    Player/         # PlayerController, PlayerStats
    Camera/         # CameraFollow
    Attack/         # AttackModule, RadialAttack, ConeAttack, HomingAttack
    Enemy/          # EnemyBase, EnemyAI, EnemySpawner
    Data/           # ScriptableObject definitions
    UI/             # UIManager, HPBar, XPBar, TimerUI, LevelUpPanel
    Map/            # InfiniteMapController, ChunkManager
    Audio/          # AudioManager, BGMController
  ScriptableObjects/
    Enemies/        # EnemyData assets
    Weapons/        # WeaponData assets
    Waves/          # SpawnTableData, WaveData assets
  Prefabs/
    Player/ | Enemies/ | Projectiles/ | Effects/ | UI/
  Art/
    Sprites/ | Backgrounds/
  Audio/
    BGM/ | SE/
  Scenes/
    MainGame.unity | Result.unity
```

## Current Scripts (Template)

| File | Description |
|---|---|
| `Core/GameManager.cs` | Singleton, GameState enum, 10-min timer |
| `Core/EventBus.cs` | Static events (OnXPCollected, OnLevelUp, etc.) |
| `Core/IPoolable.cs` | Interface: OnSpawn(), OnDespawn() |
| `Core/PoolManager.cs` | Singleton pool with PreWarm/Spawn/Despawn |
| `Core/BuildScript.cs` | Editor-only build automation (Android/iOS) |
| `Data/EnemyData.cs` | ScriptableObject: HP, speed, damage, XP, sprite |
| `Data/WeaponData.cs` | ScriptableObject: type, damage, cooldown, range |
| `Data/SpawnTableData.cs` | ScriptableObject: wave entries with timing |

## Key Conventions (from CLAUDE.md)

- Namespace: `RoguelikeSurvivor`
- `[SerializeField] private` (not public fields)
- `TryGetComponent<T>()` over `GetComponent<T>()`
- No `Find()` / `FindObjectOfType()` at runtime
- No allocations in `Update()` / `FixedUpdate()`
- `CompareTag("Enemy")` not `gameObject.tag == "Enemy"`
- Pool everything: implement `IPoolable`

## URP 2D Setup Checklist

- [ ] Rendering: URP 2D Renderer (already in project template)
- [ ] Camera: Orthographic, Size for portrait (9:16)
- [ ] Sorting Layers: Background, Map, Enemies, Player, Projectiles, UI
- [ ] Physics2D: Layer-based collision matrix
- [ ] Quality: Mobile preset (no MSAA, no shadows)

## Input System

- Package: `com.unity.inputsystem` (already installed: 1.18.0)
- Create `PlayerInputActions.inputactions` asset
- Actions: Move (Vector2), Attack (Button)
- Virtual joystick for mobile

## Performance Targets

| Metric | Target |
|---|---|
| FPS | 60 (mid-range mobile) |
| Active enemies | 500 max (pooled) |
| Active projectiles | 200 max (pooled) |
| GC alloc/frame | < 1KB |
| Draw calls | < 100 (SpriteAtlas) |

## Troubleshooting

### Library/Bee cache has old paths
Unity auto-regenerates on next open. Safe to delete Library/ if needed.

### .meta files missing
Open project in Unity editor -> auto-generated for all assets.

### Script compilation errors
Check Unity console (Window -> Console) after opening project.

### Build failures
- Android: Ensure Android SDK/NDK paths set (Preferences -> External Tools)
- iOS: Ensure Xcode installed (macOS only)
- Check `BuildScript.cs` for correct scenes list
