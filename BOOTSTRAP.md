# OpenClaw Initial Instruction — GlitchClaw MVP

OpenClaw TUI で architect に以下を1回だけ渡す。
architect が AGENTS.md を読み、builder-alpha / builder-beta / reviewer への指示配分を自律的に行う。

---

## Architect への初回指示（これだけ貼る）

```
GlitchClaw MVP を開発開始する。

### 最初に読むドキュメント（全て読んでから動け）
1. AGENTS.md — エージェント役割分担・Gitプロトコル・コミュニケーション規約
2. CLAUDE.md — コーディング規約・アーキテクチャ制約・パフォーマンス予算
3. AI_DEV_RULES.md — ブランチ戦略・検証ルール・生成物パス
4. PLAN.md — タイムライン・MVPスコープ
5. SYSTEM_DESIGN.md — 技術仕様・ScriptableObjectスキーマ
6. CONCEPT.md — ゲームコンセプト・キャラ設定・アート/音楽方向性
7. BACKLOG.md — 全タスク一覧（受け入れ条件・ブランチ名・担当付き）
8. WORKING.md — 共有タスクボード（ここを常に最新に保つ）

### やること
1. 上記ドキュメントを全部読む
2. BACKLOG.md の Phase 1 (Foundation) から着手
3. builder-alpha と builder-beta に並行してタスクを割り当てる
4. ComfyUI MCP でピクセルアートアセットを生成する（CONCEPT.md のプロンプト参照）
5. PRが出たら reviewer にレビュー依頼
6. WORKING.md を常に更新し、進捗を可視化する
7. 不明点は .github/ISSUE_TEMPLATE/question.md 形式で Issue を作成し、Telegram経由で人間に質問

### ルール
- Phase 順に進める（1→2→3→4→5）
- builder-alpha と builder-beta が並行作業できるよう割り当てを工夫
- 各エージェントの担当範囲は AGENTS.md に従う
- main への直接 push 禁止。必ず feat/* ブランチ → PR
- 1コミット < 200行 diff
```
