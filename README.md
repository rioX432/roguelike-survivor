# GlitchClaw

サイバーパンク世界で AI に追いやられた小さな生き物が生き延びる、縦画面サバイバーズライクゲーム。

**全工程を AI エージェントが開発。人間はディレクターとして指示を出すのみ。**

## Game Concept

- **ジャンル**: 縦画面 2D サバイバーズライク（ダダサバイバー系）
- **世界観**: AI が支配するネオンシティ。プレイヤーは「グリッチ」として生まれた小さな生き物
- **キャラクター**: スティッチ × ちいかわ × ハムスター — パステルカラーの小さくてかわいい生き物が、暗いサイバーパンク世界で必死に戦う
- **ゲームループ**: 10分間のウェーブ制 → 自動攻撃で AI マシンを撃退 → レベルアップで強化 → リザルト
- **BGM**: ケルト音楽 × 雅楽 × 和ロック の融合サウンド

## AI-Driven Development

本プロジェクトは **OpenClaw** によるマルチエージェント AI オーケストレーションで開発されています。
人間（ディレクター）はゲームの方向性を決め、AI エージェントがコード・アセット・音楽を生成します。

### Agent Architecture

```
Human Director (You)
  │
  ├── Telegram Bot (@glitchclaw_agent_bot) ── 質問・通知チャンネル
  │
  ▼
OpenClaw Gateway (ws://127.0.0.1:18789)
  │
  ├── architect (Claude Sonnet) ─── PM・タスク分解・アセット生成指示
  ├── builder-alpha (Claude Sonnet) ── Player・UI・攻撃・シーン
  ├── builder-beta (Claude Sonnet) ── Enemy・データ・スポーン・Prefab
  └── reviewer (Claude Haiku) ──── コードレビュー・パフォーマンス監視
```

### MCP (Model Context Protocol) Servers

| Server | Purpose | Tools |
|---|---|---|
| **Unity MCP** | Unity Editor 操作（シーン・Prefab・スクリプト・ビルド） | 30 tools |
| **ComfyUI MCP** | AI 画像生成（ピクセルアートスプライト・背景タイル） | 17 tools |
| **GitHub MCP** | Issue / PR / レビュー / マージ自動化 | 38 tools |
| **Suno MCP** | AI 音楽生成（BGM・SE） | Playwright 経由 |

### Communication

エージェントと人間ディレクターの双方向コミュニケーションは **Telegram Bot** 経由で行う。
エージェントが判断に迷った場合、Telegram で即座に質問が届き、スマホから回答可能。

### Workflow

```
1. Human: BACKLOG.md にタスクを定義（受け入れ条件付き）
2. Human: BOOTSTRAP.md の指示を architect に渡して開発開始
3. Architect: タスクを分解し、Builder に割り当て
4. Builder: feat/* ブランチで実装 → PR 作成
5. Reviewer: コードレビュー + パフォーマンスチェック
6. Agent → Human: 不明点は Telegram で質問（GitHub Issue にも記録）
7. Human: PR を確認 → マージ承認
8. (optional) android_smoke.sh で実機スモークテスト
```

## Tech Stack

| Component | Technology |
|---|---|
| Engine | Unity 6 (6000.3.9f1) — URP 2D |
| Platform | Android / iOS |
| Language | C# (.NET) |
| AI Orchestration | OpenClaw 2026.2.21 |
| Communication | Telegram Bot (agent ↔ human) |
| Image Generation | ComfyUI (local) |
| Music Generation | Suno (via MCP) |
| Loop Processing | PyMusicLooper |
| Version Control | Git + GitHub |

## Project Structure

```
roguelike-survivor/
├── CONCEPT.md              # Game concept, world, character, art/audio direction
├── PLAN.md                 # Timeline, MVP scope, task checklist
├── SYSTEM_DESIGN.md        # Technical spec, ScriptableObject schemas
├── CLAUDE.md               # Coding conventions, architecture constraints
├── AI_DEV_RULES.md         # Rules for AI agents (branch strategy, verification)
├── BACKLOG.md              # Issue-level task backlog with acceptance criteria
├── WORKING.md              # Shared task board (agent status tracking)
├── BOOTSTRAP.md              # OpenClaw initial instruction (paste to architect)
├── AGENTS.md               # Agent roles, git protocol, communication
├── SECURITY.md             # API key management, security policy
├── CHEATSHEET_*.md         # Quick reference for OpenClaw, MCP, Unity
├── tools/
│   └── android_smoke.sh    # Automated Android build + smoke test
├── .github/
│   ├── pull_request_template.md
│   └── ISSUE_TEMPLATE/
│       └── question.md     # Agent → Human question template
└── roguelike-survivor-game/  # Unity project
    ├── Assets/
    │   ├── Scripts/        # C# source code
    │   ├── Art/Generated/  # ComfyUI generated assets
    │   └── Audio/Generated/# Suno generated audio
    ├── Packages/
    └── ProjectSettings/
```

## Development Setup

### Requirements

- macOS (Apple Silicon)
- Unity 6 (6000.3.9f1) with Android + iOS build support
- Node.js 22+ (via nvm)
- Python 3.11+ (via pyenv)
- Docker Desktop (for GitHub MCP)
- OpenClaw CLI
- ComfyUI.app

### Quick Start

```bash
# 1. OpenClaw status check
openclaw status

# 2. MCP servers check
mcporter list

# 3. Open TUI and start directing
openclaw tui
```

See `CHEATSHEET_OPENCLAW.md`, `CHEATSHEET_MCP.md`, `CHEATSHEET_UNITY.md` for detailed commands.
