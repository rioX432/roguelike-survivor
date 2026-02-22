# AI_DEV_RULES.md — OpenClaw Agent Development Rules

All agents MUST follow these rules. Violations will be caught by reviewer.

## Branch & PR Strategy

- **NEVER push directly to `main`**
- Create feature branches: `feat/<short-description>` (e.g., `feat/player-controller`)
- Create a PR via GitHub MCP when the feature is complete
- PR must pass all checks before merge (see Verification below)
- Keep PRs small and focused (< 200 lines diff preferred)

## Verification Checklist (MANDATORY for every PR)

Before creating a PR, run and confirm:

1. **Unity PlayMode**: Zero errors in Console (`Window → Console`)
2. **Script compilation**: Zero `error CS` in build output
3. **Android build**: `tools/android_smoke.sh` succeeds (when device available)
4. **Paste results in PR description**

If any check fails, fix before creating PR.

## Communication

- **Unclear requirements?** → Create a GitHub Issue using the `question` template
- **Blocked by another agent?** → Update WORKING.md blockers section + create Issue
- **Design decision needed?** → Document in WORKING.md Decisions Log, tag as `[DECISION NEEDED]`
- **Never guess** — ask via Issue if unsure about game design, UX, or architecture

## Generated Assets

All AI-generated assets MUST go to fixed paths:

| Type | Path |
|---|---|
| Sprites (ComfyUI) | `Assets/Art/Generated/Sprites/` |
| Backgrounds (ComfyUI) | `Assets/Art/Generated/Backgrounds/` |
| BGM (Suno) | `Assets/Audio/Generated/BGM/` |
| SE (Suno/generated) | `Assets/Audio/Generated/SE/` |

Never scatter generated files across arbitrary directories.

## Unity Binary Files

- **NEVER** create or edit `.prefab`, `.asset`, `.unity`, `.meta` files as raw text
- Use Unity MCP tools for scene/prefab operations
- Only edit `.cs` source files and text-based configs directly
- `.meta` files are auto-generated — never create manually

## Code Quality

- Every script MUST compile without errors before commit
- No `Debug.Log()` left in committed code (use conditional `#if UNITY_EDITOR`)
- Follow all rules in `CLAUDE.md`
- Namespace: `RoguelikeSurvivor` (always)

## Git Hygiene

- Commit messages: English, 1 line, imperative mood
- Always include `.meta` files with their corresponding assets
- Never commit: API keys, `.env`, `progress.txt`, `openclaw.json`
- Scene files: only builder-alpha edits (no exceptions)
