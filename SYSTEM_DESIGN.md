# SYSTEM_DESIGN.md — Technical Specification

## Game Loop

```
[Start] → 10min Timer Begins
  ↓
  Player moves (virtual joystick)
  Enemies spawn (wave table)
  Auto-attacks fire (interval-based)
  Enemies take damage → die → drop XP gems
  Player collects XP → Level up → Choose upgrade
  ↓
[10min elapsed or Player HP = 0] → Result Screen
```

## Core Systems

### 1. GameManager (Singleton)

- Manages game state: `Playing`, `Paused`, `LevelUp`, `GameOver`
- Holds central timer (10 minutes countdown)
- TimeScale control for debug (10x speed testing)
- References to all manager singletons

### 2. PlayerController

- Movement: `Rigidbody2D.MovePosition()` in `FixedUpdate()`
- Input: New InputSystem, virtual joystick (`Joystick Pack` or custom)
- Stats: HP, Speed, AttackPower, AttackSpeed (modified by level-ups)
- Invincibility frames on hit (0.5s)

### 3. Attack System

Base class: `AttackModule`
```
AttackModule (abstract)
  ├── RadialAttack      — 360 degree projectiles at interval
  ├── ConeAttack        — Forward-facing spread
  └── HomingAttack      — Seeks nearest enemy
```

- Each module has: Damage, Interval, Range, ProjectileSpeed
- Player can hold multiple modules simultaneously
- Modules fire automatically, no player input needed
- Projectiles are pooled via PoolManager

### 4. Enemy System

Base class: `EnemyBase : MonoBehaviour, IPoolable`

| Type | Behavior | HP | Speed | Spawn Phase |
|---|---|---|---|---|
| Bit Drone | Hover toward player | Low | Slow | 0:00~ |
| Glitch Bug | Rush toward player (fast) | Low | Fast | 1:00~ |
| Rust Walker | Walk toward player | Medium | Medium | 3:00~ |
| Siege Core | Walk toward player, shielded | High | Slow | 5:00~ |
| Overlord AI | Float toward player, tentacles | Very High | Medium | 8:00~ |

- All enemies use simple chase AI: `(player.position - transform.position).normalized`
- Damage on contact with player (OnTriggerEnter2D)
- Death: play particle effect, drop XP gem, return to pool

### 5. Spawn System

`EnemySpawner` reads `SpawnTableData` (ScriptableObject):

```
SpawnTableData
  └── List<WaveEntry>
        ├── TimeStart (float, seconds)
        ├── TimeEnd (float, seconds)
        ├── EnemyData (SO reference)
        ├── SpawnRate (enemies per second)
        └── MaxActive (cap)
```

- Spawn position: random point on circle around player (screen edge + margin)
- Difficulty curve: increase spawn rate and enemy variety over 10 minutes

### 6. Level-Up System

- XP gems auto-collect within magnetic radius
- XP thresholds: `baseXP * level * 1.2` (exponential curve)
- On level up:
  - Pause game (TimeScale = 0)
  - Show 3 random upgrade options (new attack or stat boost)
  - Player picks one → resume

### 7. Infinite Map (Ring Buffer)

```
[C][C][C]     C = Chunk (SpriteRenderer with tiled sprite)
[C][P][C]     P = Player (always near center chunk)
[C][C][C]

When player crosses chunk boundary:
  → Move the farthest row/column to the opposite side
  → Update chunk positions (Transform.position only)
```

- 9 chunks total, each covering 1 screen + margin
- No Tilemap, no instantiation — just repositioning transforms

### 8. UI Layout (Portrait)

```
┌─────────────────┐
│ [Timer]  [Pause] │  ← Canvas_Static
├─────────────────┤
│                  │
│                  │
│   Game Area      │
│                  │
│                  │
├─────────────────┤
│ [HP Bar]         │  ← Canvas_Dynamic
│ [XP Bar]         │
│ [Level: N]       │
└─────────────────┘
```

### 9. Audio

- `AudioManager` singleton with separate AudioSources for BGM / SE
- BGM: seamless loop (PyMusicLooper processed)
- SE pool: 4-8 concurrent SE AudioSources to avoid clipping
- SE list: attack_fire, enemy_hit, enemy_die, level_up, player_hit

## ScriptableObject Schema

### EnemyData
```csharp
[CreateAssetMenu(menuName = "Data/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public Sprite sprite;
    public float maxHP;
    public float moveSpeed;
    public float contactDamage;
    public float xpDrop;
    public float scale;
}
```

### WeaponData
```csharp
[CreateAssetMenu(menuName = "Data/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Sprite icon;
    public Sprite projectileSprite;
    public float damage;
    public float interval;
    public float speed;
    public float range;
    public int projectileCount;
    public AttackType attackType; // Radial, Cone, Homing
}
```

### SpawnTableData
```csharp
[CreateAssetMenu(menuName = "Data/SpawnTableData")]
public class SpawnTableData : ScriptableObject
{
    public List<WaveEntry> waves;
}

[System.Serializable]
public class WaveEntry
{
    public float timeStart;
    public float timeEnd;
    public EnemyData enemyData;
    public float spawnRate;
    public int maxActive;
}
```

## Event Bus (Decoupled Communication)

```csharp
public static class EventBus
{
    public static event Action<int> OnXPCollected;
    public static event Action<int> OnLevelUp;
    public static event Action OnPlayerDeath;
    public static event Action OnGameOver;
    public static event Action<EnemyBase> OnEnemyDeath;
}
```

Prefer EventBus over direct references between systems to reduce coupling.
