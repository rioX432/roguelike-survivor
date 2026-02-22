# CONCEPT.md — Game Concept & Art Direction

## Core Concept

サイバーパンク世界で AI に追いやられた小さな生き物たちが生き延びるサバイバーズライク。
ストーリーは不要。ゲームプレイと雰囲気で語る。

## World

- **舞台**: 崩壊したネオンシティ。AI が支配し、有機生命体は隅に追いやられている
- **雰囲気**: 暗いサイバーパンクの背景に、小さくてかわいいキャラが必死に戦うギャップ
- **背景要素**: ネオン看板、壊れたホログラム、暗い路地、光るケーブル、雨に濡れた地面
- **カラーパレット**:
  - 背景: ダークブルー、パープル、ネオンピンク、ネオングリーン（サイバーパンク定番）
  - キャラ: パステル系の明るい色（背景とのコントラストで視認性確保）

## Player Character

**コンセプト**: スティッチ × ちいかわ × ハムスター

- 小さくて丸い体型（ハムスター的）
- 大きな耳（スティッチ的、ただし丸め）
- つぶらな目、小さい口（ちいかわ的な表情）
- 手足は短くてデフォルメ
- 色: パステルブルーまたはラベンダー（サイバーパンク背景に映える）
- 表情: 基本は不安げだが頑張っている感じ（ちいかわ的）

## Enemies (AI Machines)

敵は全て AI / 機械 / ドローン系。世界観に沿ったデザイン。

| ID | Name | Concept | Behavior |
|---|---|---|---|
| 1 | Bit Drone | 小さな浮遊ドローン、目が赤く光る | 低HP、低速、序盤の雑魚 |
| 2 | Glitch Bug | 虫型ナノマシン、紫に発光 | 低HP、高速、すばしっこい |
| 3 | Rust Walker | 二足歩行の錆びたロボ | 中HP、中速、中盤から |
| 4 | Siege Core | 大型四足歩行マシン、シールド持ち | 高HP、低速、エリート |
| 5 | Overlord AI | 巨大な浮遊 AI コア、触手ケーブル付き | 超高HP、中速、ボス |

## Art Style

- **ピクセルアート**（32x32 or 64x64 キャラ）
- キャラ: かわいい、丸い、パステルカラー、2〜3フレームアニメ
- 敵: 機械的、角ばった、ネオン発光、赤・紫・オレンジ系
- 背景: ダークトーン、ネオンアクセント、タイルベースで繰り返し可能
- **コントラスト重視**: 暗い背景 × 明るいキャラで視認性を確保（ゲーム性に直結）

### ComfyUI Prompt Keywords

**Player character**:
```
pixel art, tiny cute creature, round body, big round ears, small hamster,
stitch-like, chibi, pastel blue, large eyes, worried expression,
cyberpunk setting, 32x32 sprite sheet, transparent background,
top-down view, game asset
```

**Enemies**:
```
pixel art, cyberpunk enemy, mechanical drone/robot/AI core,
neon glow, red eyes, dark metal, purple energy,
32x32 sprite, transparent background, top-down view, game asset
```

**Background tile**:
```
pixel art, cyberpunk alley, dark neon city, wet ground reflection,
neon signs, broken hologram, cable wires, tileable,
top-down perspective, dark blue purple palette, game background tile
```

## Audio Direction

### BGM

**コンセプト**: ケルト音楽 × 雅楽 × 和ロック の融合

- **ケルト要素**: フィドル、ティンホイッスル、アイリッシュフルート、弾むリズム
  - リファレンス: 塔の上のラプンツェル「王国でダンス」、メリダ系のケルティック曲
- **雅楽要素**: 笙、篳篥、龍笛のような音色、雅な間
- **和ロック要素**: エレキギター、ドラム、力強いビート
- **テンポ**: BPM 140-160（アクションゲームに合う疾走感）
- **雰囲気**: 哀愁がありつつも力強い。小さな生き物が必死に戦う緊張感と高揚感

### Suno Prompt Keywords
```
Celtic rock fusion, gagaku instruments, Japanese rock, fiddle,
tin whistle, sho, electric guitar, fast tempo, BPM 150,
epic battle music, cyberpunk atmosphere, energetic, emotional,
game soundtrack, loopable
```

### SE (Sound Effects)

| SE | Description |
|---|---|
| attack_fire | 軽いエネルギー音（ピュンッ） |
| enemy_hit | 金属にヒットする音（キンッ） |
| enemy_die | 機械が壊れる音（ガシャン + 電子音） |
| level_up | キラキラした上昇音 |
| player_hit | 柔らかいダメージ音（ちいかわ的な「ワッ」に近い音） |

## Game Name Candidates

| Name | Concept |
|---|---|
| **NeonPaw** | ネオン + 肉球、サイバーパンク×かわいいの象徴 |
| **ByteBiter** | バイト（データ）を噛む小さな生き物 |
| **Glitch Survivors** | グリッチ（バグ）の世界のサバイバー |
| **Neon Critters** | ネオン街の小さな生き物たち |
| **CyberPaws** | サイバー + 肉球 |
